// Copyright (c) 2024 Martin Mills <daggerbot@gmail.com>
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace Autk.Display.X11;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;

using Autk.Native.Unix;
using Autk.Native.X11;

internal class X11DisplayProvider : DisplayProvider, IFileDescriptor
{
    //
    // Fields
    //

    public readonly X11Atoms Atoms;

    private XlibHandle _xlibHandle;
    private IntPtr _xcb;
    private int _fd;
    private int _defaultScreenIndex;
    private Xcb.xcb_screen_t[] _screens;
    private Dictionary<uint, X11WindowProvider> _windowMap = new();
    private X11MessageQueue _messageQueue = new();

    //
    // Constructors
    //

    public X11DisplayProvider(string? name = null)
    {
        // Open Xlib display connection.
        var xlib = Xlib.XOpenDisplay(name);

        if (xlib == IntPtr.Zero)
            throw new X11Exception($"XOpenDisplay failed: {name}");

        _xlibHandle = new XlibHandle(xlib);

        try
        {
            // Use XCB connection over Xlib.
            Xlib.XSetEventQueueOwner(xlib, Xlib.XEventQueueOwner.XCBOwnsEventQueue);
            _xcb = Xlib.XGetXCBConnection(xlib);

            if (_xcb == IntPtr.Zero)
                throw new X11Exception("Can't get XCB connection from Xlib connection");

            // Initialize misc.
            Atoms = X11Atoms.InternAtoms(this);
            _fd = Xcb.xcb_get_file_descriptor(_xcb);
            _defaultScreenIndex = Xlib.XDefaultScreen(xlib);

            // Initialize screens.
            var setupPtr = Xcb.xcb_get_setup(_xcb);
            var screenIter = Xcb.xcb_setup_roots_iterator(setupPtr);

            if (screenIter.rem < 1)
                throw new X11Exception("No X11 screens available");

            _screens = new Xcb.xcb_screen_t[screenIter.rem];

            for (int i = 0; i < _screens.Length; i++)
            {
                _screens[i] = Marshal.PtrToStructure<Xcb.xcb_screen_t>(screenIter.data);
                Xcb.xcb_screen_next(ref screenIter);
            }
        }
        catch
        {
            Dispose();
            throw;
        }
    }

    //
    // Properties
    //

    public Xcb.xcb_screen_t DefaultScreen => _screens[_defaultScreenIndex];

    public int DefaultScreenIndex => _defaultScreenIndex;

    public int FileDescriptorValue => _fd;

    public override bool IsDisposed => _xlibHandle.IsInvalid;

    internal IntPtr XcbConnectionPtr => _xcb;

    internal IntPtr XlibDisplayPtr => _xlibHandle.XlibDisplayPtr;

    //
    // Methods
    //

    private void CheckConnectionError()
    {
        ThrowIfDisposed();

        int errorCode = Xcb.xcb_connection_has_error(_xcb);

        switch (errorCode)
        {
            case 0:
                break;
            case Xcb.XCB_CONN_ERROR:
                throw new X11Exception("XCB I/O error");
            case Xcb.XCB_CONN_CLOSED_EXT_NOTSUPPORTED:
                throw new X11Exception("X11 extension not supported");
            case Xcb.XCB_CONN_CLOSED_MEM_INSUFFICIENT:
                throw new X11Exception("XCB: Out of memory");
            case Xcb.XCB_CONN_CLOSED_REQ_LEN_EXCEED:
                throw new X11Exception("Invalid X11 request length");
            case Xcb.XCB_CONN_CLOSED_PARSE_ERR:
                throw new X11Exception("Failed to parse X11 display name");
            case Xcb.XCB_CONN_CLOSED_INVALID_SCREEN:
                throw new X11Exception("Invalid X11 screen");
            case Xcb.XCB_CONN_CLOSED_FDPASSING_FAILED:
                throw new X11Exception("X11 file descriptor passing failed");
            default:
                throw new X11Exception($"XCB error code {errorCode}");
        }
    }

    public override WindowProvider CreateWindow(Window owner, WindowStyle style, Size size)
    {
        var window = new X11WindowProvider(owner, this, size);

        try
        {
            window.Disposing += WindowDisposing;
            _windowMap.Add(window.Xid, window);
        }
        catch
        {
            window.Dispose();
            throw;
        }

        return window;
    }

    public override void Dispose()
    {
        if (IsDisposed)
            return;

        Xlib.XCloseDisplay(XlibDisplayPtr);

        _xlibHandle.Dispose();
        _xcb = IntPtr.Zero;
        _messageQueue.Dispose();

        OnDisposed();
    }

    private void HandleEvent(IntPtr eventPtr)
    {
        var generic = Marshal.PtrToStructure<Xcb.xcb_generic_event_t>(eventPtr);

        switch (generic.response_type & ~0x80)
        {
            case Xcb.XCB_CLIENT_MESSAGE:
            {
                var ev = Marshal.PtrToStructure<Xcb.xcb_client_message_event_t>(eventPtr);

                if (ev.type == Atoms.WM_PROTOCOLS)
                {
                    var protocol = XcbUtils.GetClientMessageUInt32(eventPtr, 0);

                    if (protocol == Atoms.WM_DELETE_WINDOW)
                    {
                        if (_windowMap.TryGetValue(ev.window, out var window))
                            window.OnCloseRequested();
                    }
                }

                break;
            }

            case Xcb.XCB_DESTROY_NOTIFY:
            {
                var ev = Marshal.PtrToStructure<Xcb.xcb_destroy_notify_event_t>(eventPtr);

                if (_windowMap.TryGetValue(ev.window, out var window))
                    window.Invalidate();

                break;
            }

            case Xcb.XCB_MAP_NOTIFY:
            {
                var ev = Marshal.PtrToStructure<Xcb.xcb_map_notify_event_t>(eventPtr);

                if (_windowMap.TryGetValue(ev.window, out var window))
                    window.OnVisibilityChanged(true);

                break;
            }

            case Xcb.XCB_UNMAP_NOTIFY:
            {
                var ev = Marshal.PtrToStructure<Xcb.xcb_unmap_notify_event_t>(eventPtr);

                if (_windowMap.TryGetValue(ev.window, out var window))
                    window.OnVisibilityChanged(false);

                break;
            }

            default:
                break;
        }
    }

    private void HandlePendingEvents()
    {
        while (true)
        {
            var eventPtr = Xcb.xcb_poll_for_event(_xcb);

            if (eventPtr == IntPtr.Zero)
                break;

            try
            {
                HandleEvent(eventPtr);
            }
            finally
            {
                Posix.free(eventPtr);
            }
        }
    }

    public override void InvokeLater(Action action)
    {
        _messageQueue.PostAction(action);
    }

    public override void PostQuitMessage()
    {
        _messageQueue.PostQuitMessage();
    }

    public override void Run()
    {
        var pollItems = new PosixUtils.PollItem[]
        {
            new() { FileDescriptor = this, EventMask = Posix.POLLIN },
            new() { FileDescriptor = _messageQueue.ReadFileDescriptor, EventMask = Posix.POLLIN },
        };

        while (true)
        {
            _ = Xcb.xcb_flush(_xcb);
            CheckConnectionError();
            PosixUtils.Poll(pollItems, -1);

            if ((pollItems[0].ReturnEventMask & Posix.POLLERR) != 0)
                throw new X11Exception("Polling X11 connection returned error status");
            else if ((pollItems[0].ReturnEventMask & Posix.POLLHUP) != 0)
                throw new X11Exception("Disconnected from X11 server");
            else if ((pollItems[1].ReturnEventMask & Posix.POLLERR) != 0)
                throw new X11Exception("Polling message queue returned error status");
            else if ((pollItems[1].ReturnEventMask & Posix.POLLHUP) != 0)
                throw new X11Exception("Disconnected from message queue");

            if ((pollItems[0].ReturnEventMask & Posix.POLLIN) != 0)
                HandlePendingEvents();

            if ((pollItems[1].ReturnEventMask & Posix.POLLIN) != 0)
                if (!_messageQueue.HandleNextMessage())
                    break;
        }
    }

    //
    // Event handler methods
    //

    private void WindowDisposing(object? sender, EventArgs e)
    {
        if (sender != null)
        {
            var window = (X11WindowProvider)sender;
            _windowMap.Remove(window.Xid);
            window.Disposing -= WindowDisposing;
        }
    }
}
