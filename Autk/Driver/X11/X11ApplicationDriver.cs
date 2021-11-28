/*
 * Copyright (c) 2021 Martin Mills <daggerbot@gmail.com>
 *
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */

namespace Autk.Driver.X11;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

using Autk.Native;
using Autk.Native.IO;
using Autk.Native.X11;

internal class X11ApplicationDriver : ApplicationDriver
{
    //==============================================================================
    // Fields
    //==============================================================================

    public readonly X11Atoms Atoms;

    private IntPtr _xlib;
    private IntPtr _xcb;
    private int _xfd = -1;
    private Xcb.xcb_setup_t _setup;
    private Xcb.xcb_screen_t _defaultScreen;
    private Xcb.xcb_depth_t _defaultDepth;
    private Xcb.xcb_visualtype_t _defaultVisual;
    private Dictionary<uint, X11WindowDriver> _windowMap = new Dictionary<uint, X11WindowDriver>();
    private PosixPipeReader _messagePipeReader;
    private PosixPipeWriter _messagePipeWriter;
    private ConcurrentQueue<Action> _actionQueue = new ConcurrentQueue<Action>();

    //==============================================================================
    // Constructors
    //==============================================================================

    public X11ApplicationDriver(string? name = null)
    {
        _xlib = Xlib.XOpenDisplay(name);

        if (_xlib == IntPtr.Zero)
            throw new X11Exception("Can't open X11 display connection");

        try
        {
            Xlib.XSetEventQueueOwner(_xlib, Xlib.XEventQueueOwner.XCBOwnsEventQueue);
            _xcb = Xlib.XGetXCBConnection(_xlib);

            if (_xcb == IntPtr.Zero)
                throw new X11Exception("Can't get XCB connection");

            _xfd = Xcb.xcb_get_file_descriptor(_xcb);
            InitSetup();
            Atoms = InternAtoms();

            PosixPipe.Open(out _messagePipeReader, out _messagePipeWriter);
        }
        catch
        {
            if (_messagePipeReader != null)
                _messagePipeReader.Dispose();
            if (_messagePipeWriter != null)
                _messagePipeWriter.Dispose();

            Xlib.XCloseDisplay(_xlib);
            throw;
        }
    }

    //==============================================================================
    // Properties
    //==============================================================================

    public Xcb.xcb_depth_t DefaultDepth => _defaultDepth;

    public Xcb.xcb_screen_t DefaultScreen => _defaultScreen;

    public Xcb.xcb_visualtype_t DefaultVisual => _defaultVisual;

    public override bool IsDisposed => _xlib == IntPtr.Zero;

    internal IntPtr XcbConnectionPointer => _xcb;

    internal IntPtr XlibDisplayPointer => _xlib;

    //==============================================================================
    // Methods
    //==============================================================================

    private void CheckConnectionError()
    {
        if (IsDisposed)
            return;

        int errorCode = Xcb.xcb_connection_has_error(_xcb);

        switch (errorCode)
        {
            case 0:
                break;
            case Xcb.XCB_CONN_ERROR:
                throw new X11Exception("XCB I/O error");
            case Xcb.XCB_CONN_CLOSED_EXT_NOTSUPPORTED:
                throw new X11Exception("Unsupported extension");
            case Xcb.XCB_CONN_CLOSED_MEM_INSUFFICIENT:
                throw new X11Exception("Out of memory");
            case Xcb.XCB_CONN_CLOSED_REQ_LEN_EXCEED:
                throw new X11Exception("Request length out of range");
            case Xcb.XCB_CONN_CLOSED_PARSE_ERR:
                throw new X11Exception("Invalid display name");
            case Xcb.XCB_CONN_CLOSED_INVALID_SCREEN:
                throw new X11Exception("Invalid screen");
            case Xcb.XCB_CONN_CLOSED_FDPASSING_FAILED:
                throw new X11Exception("Failed to pass file descriptor");
            default:
                throw new X11Exception($"XCB connection error code {errorCode}");
        }
    }

    public override WindowDriver CreateWindow(WindowType type, Size size)
    {
        return new X11WindowDriver(this, type, size);
    }

    public override void Dispose()
    {
        if (_xlib != IntPtr.Zero)
        {
            Xlib.XCloseDisplay(_xlib);
            _xlib = IntPtr.Zero;
        }

        _messagePipeReader.Dispose();
        _messagePipeWriter.Dispose();

        OnDisposed();
    }

    public X11WindowDriver? GetWindow(uint xid)
    {
        return _windowMap.GetValueOrDefault(xid);
    }

    private bool HandleClientMessage(Xcb.xcb_client_message_event_t clientMessage)
    {
        if (clientMessage.type == Atoms.WM_PROTOCOLS && clientMessage.format == 32)
        {
            if (clientMessage.data.data0 == Atoms.WM_DELETE_WINDOW)
            {
                var window = GetWindow(clientMessage.window);

                if (window != null)
                    window.OnWindowEventReceived(new WindowEventArgs { EventType = WindowEventType.CloseRequest });
            }
        }

        return true;
    }

    // Returns false if the main loop should terminate.
    private bool HandleEvent(IntPtr eventPtr)
    {
        var genericEvent = Marshal.PtrToStructure<Xcb.xcb_generic_event_t>(eventPtr);

        switch (genericEvent.response_type & ~0x80)
        {
            case Xcb.XCB_CLIENT_MESSAGE:
                return HandleClientMessage(Marshal.PtrToStructure<Xcb.xcb_client_message_event_t>(eventPtr));

            case Xcb.XCB_DESTROY_NOTIFY:
                {
                    var destroyEvent = Marshal.PtrToStructure<Xcb.xcb_destroy_notify_event_t>(eventPtr);
                    var window = GetWindow(destroyEvent.window);
                    _windowMap.Remove(destroyEvent.window);

                    if (window != null)
                        window.Expire();
                }
                return true;

            default:
                return true;
        }
    }

    private void InitSetup()
    {
        //
        // _setup
        //

        var setupPtr = Xcb.xcb_get_setup(_xcb);
        _setup = Marshal.PtrToStructure<Xcb.xcb_setup_t>(setupPtr);

        //
        // _defaultScreen
        //

        int defaultScreenNum = Xlib.XDefaultScreen(_xlib);
        var screenIter = Xcb.xcb_setup_roots_iterator(setupPtr);
        IntPtr screenPtr;

        for (int i = 0; ; ++i)
        {
            if (screenIter.rem < 1)
                throw new X11Exception($"Invalid default screen: {defaultScreenNum}");
            else if (i == defaultScreenNum)
            {
                screenPtr = screenIter.data;
                break;
            }
            else
                Xcb.xcb_screen_next(ref screenIter);
        }

        _defaultScreen = Marshal.PtrToStructure<Xcb.xcb_screen_t>(screenPtr);

        //
        // _defaultDepth
        //

        var depthIter = Xcb.xcb_screen_allowed_depths_iterator(screenPtr);
        IntPtr depthPtr;
        Xcb.xcb_depth_t depth;

        for (; ; )
        {
            if (depthIter.rem < 1)
                throw new X11Exception($"Invalid default root depth: {_defaultScreen.root_depth}");

            depthPtr = depthIter.data;
            depth = Marshal.PtrToStructure<Xcb.xcb_depth_t>(depthPtr);

            if (depth.depth == _defaultScreen.root_depth)
                break;
            else
                Xcb.xcb_depth_next(ref depthIter);
        }

        _defaultDepth = depth;

        //
        // _defaultVisual
        //

        var visualIter = Xcb.xcb_depth_visuals_iterator(depthPtr);
        IntPtr visualPtr;
        Xcb.xcb_visualtype_t visual;

        for (; ; )
        {
            if (visualIter.rem < 1)
                throw new X11Exception($"Invalid default root visual: {_defaultScreen.root_visual}");

            visualPtr = visualIter.data;
            visual = Marshal.PtrToStructure<Xcb.xcb_visualtype_t>(visualPtr);

            if (visual.visual_id == _defaultScreen.root_visual)
                break;
            else
                Xcb.xcb_visualtype_next(ref visualIter);
        }

        if ((Xcb.xcb_visual_class_t)visual.class_ != Xcb.xcb_visual_class_t.XCB_VISUAL_CLASS_TRUE_COLOR)
            throw new X11Exception("Default root visual is not true-color");

        _defaultVisual = visual;
    }

    // Interns all atoms declared as fields in the X11Atoms struct.
    // Batch sends requests first then batch receives replies to prevent excessive I/O blocking.
    private X11Atoms InternAtoms()
    {
        var atoms = (object)new X11Atoms();
        var fields = typeof(X11Atoms).GetFields().ToArray();
        var cookies = new Xcb.xcb_intern_atom_cookie_t[fields.Length];

        for (int i = 0; i < fields.Length; ++i)
        {
            var nameBytes = Encoding.UTF8.GetBytes(fields[i].Name);
            cookies[i] = Xcb.xcb_intern_atom(_xcb, onlyIfExists: false, checked((ushort)nameBytes.Length), nameBytes);
        }

        for (int i = 0; i < fields.Length; ++i)
        {
            var errorPtr = IntPtr.Zero;
            var replyPtr = Xcb.xcb_intern_atom_reply(_xcb, cookies[i], ref errorPtr);

            try
            {
                if (errorPtr != IntPtr.Zero)
                    throw new X11Exception(Marshal.PtrToStructure<Xcb.xcb_generic_error_t>(errorPtr));
                else if (replyPtr == IntPtr.Zero)
                    throw new X11Exception("Failed to intern atom");
                else
                {
                    var reply = Marshal.PtrToStructure<Xcb.xcb_intern_atom_reply_t>(replyPtr);
                    fields[i].SetValue(atoms, reply.atom);
                }
            }
            finally
            {
                if (errorPtr != IntPtr.Zero)
                    Libc.free(errorPtr);
                if (replyPtr != IntPtr.Zero)
                    Libc.free(replyPtr);
            }
        }

        return (X11Atoms)atoms;
    }

    // Returns false if the main loop should terminate.
    private bool ReadAndExecuteMessage()
    {
        var buffer = new byte[1];

        if (_messagePipeReader.Read(buffer, 0, 1) == 0)
            throw new EndOfStreamException();

        switch ((Opcode)buffer[0])
        {
            case Opcode.Action:
                if (_actionQueue.TryDequeue(out var action))
                    action.Invoke();
                return true;

            case Opcode.Quit:
                return false;

            default:
                throw new IOException("Invalid application message opcode");
        }
    }

    public void RegisterWindow(X11WindowDriver window)
    {
        if (IsDisposed || window.IsDisposed || window.XcbConnectionPointer != _xcb)
            throw new InvalidOperationException();

        _windowMap[window.Xid] = window;
    }

    public override void Quit()
    {
        lock (this)
        {
            WriteOpcode(Opcode.Quit);
        }
    }

    public override void Run()
    {
        ThrowIfDisposed();

        var pollfds = new Posix.pollfd[]
        {
            new Posix.pollfd { fd = _xfd, events = Posix.POLLIN },
            new Posix.pollfd { fd = _messagePipeReader.FileDescriptor, events = Posix.POLLIN },
        };

        for (; ; )
        {
            if (IsDisposed)
                break;
            if (Xcb.xcb_flush(_xcb) <= 0)
                throw new X11Exception("Flushing requests failed");

            CheckConnectionError();

            int pollResult = Posix.poll(pollfds, new UIntPtr((uint)pollfds.Length), -1);

            if (pollResult < 0)
                throw new IOException("poll(): " + Libc.strerror(Marshal.GetLastSystemError()));
            else if (pollResult == 0)
                continue;

            // Poll for X11 events.
            if (pollfds[0].revents != 0)
            {
                if (pollfds[0].revents != Posix.POLLIN)
                    throw new IOException($"poll(): revents = {pollfds[0].revents}");

                for (; ; )
                {
                    var eventPtr = Xcb.xcb_poll_for_event(_xcb);

                    if (eventPtr == IntPtr.Zero)
                        break;

                    try
                    {
                        if (!HandleEvent(eventPtr))
                            return;
                    }
                    finally
                    {
                        Libc.free(eventPtr);
                    }
                }
            }

            // Poll for application messages.
            if (pollfds[1].revents != 0)
            {
                if (pollfds[1].revents != Posix.POLLIN)
                    throw new IOException($"poll(): revents = {pollfds[1].revents}");

                if (!ReadAndExecuteMessage())
                    return;
            }
        }
    }

    private void WriteOpcode(Opcode opcode)
    {
        _messagePipeWriter.Write(new byte[] { (byte)opcode }, 0, 1);
    }

    //==============================================================================
    // Types
    //==============================================================================

    private enum Opcode
    {
        Action = 1,
        Quit = 2,
    }
}
