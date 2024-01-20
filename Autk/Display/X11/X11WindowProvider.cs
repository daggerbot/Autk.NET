// Copyright (c) 2024 Martin Mills <daggerbot@gmail.com>
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace Autk.Display.X11;

using System;
using System.Drawing;

using Autk.Native.X11;

internal class X11WindowProvider : WindowProvider
{
    //
    // Fields
    //

    private X11DisplayProvider _display;
    private uint _xid;

    private bool _visible;

    //
    // Constructors
    //

    public X11WindowProvider(Window owner, X11DisplayProvider display, Size size)
        : base(owner)
    {
        display.ThrowIfDisposed();
        _display = display;

        var xcb = display.XcbConnectionPtr;
        var screen = display.DefaultScreen;
        _xid = Xcb.xcb_generate_id(xcb);

        Xcb.xcb_create_window(
            connectionPtr: xcb,
            depth: screen.root_depth,
            window: _xid,
            parentId: screen.root,
            x: 0,
            y: 0,
            width: ClampSize(size.Width),
            height: ClampSize(size.Height),
            borderWidth: 0,
            class_: (ushort)Xcb.xcb_window_class_t.XCB_WINDOW_CLASS_INPUT_OUTPUT,
            screen.root_visual,
            Xcb.XCB_CW_EVENT_MASK,
            new int[]
            {
                Xcb.XCB_EVENT_MASK_STRUCTURE_NOTIFY,
            });

        try
        {
            SetWMProtocols();
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

    public override bool IsDisposed => _xid == 0;

    public override bool IsVisible
    {
        get => _visible;
        set
        {
            if (value)
                Xcb.xcb_map_window(_display.XcbConnectionPtr, _xid);
            else
                Xcb.xcb_unmap_window(_display.XcbConnectionPtr, _xid);
        }
    }

    internal uint Xid => _xid;

    //
    // Methods
    //

    public static ushort ClampSize(int size)
    {
        return (ushort)Math.Clamp(size, 1, 0xFFFF);
    }

    public override void Dispose()
    {
        if (IsDisposed)
            return;

        Xcb.xcb_destroy_window(_display.XcbConnectionPtr, _xid);
        Invalidate();
    }

    public void Invalidate()
    {
        if (IsDisposed)
            return;

        OnDisposing();

        _xid = 0;
        _visible = false;

        OnDisposed();
    }

    internal void OnCloseRequested()
    {
        Owner.CloseRequested();
    }

    internal void OnDisposing()
    {
        Disposing?.Invoke(this, EventArgs.Empty);
    }

    internal void OnVisibilityChanged(bool visible)
    {
        if (visible != this._visible)
        {
            this._visible = visible;
            Owner.OnVisibilityChanged(visible);
        }
    }

    private void SetProperty(uint property, uint type, uint[] data)
    {
        ThrowIfDisposed();

        Xcb.xcb_change_property(
            connectionPtr: _display.XcbConnectionPtr,
            mode: (byte)Xcb.xcb_prop_mode_t.XCB_PROP_MODE_REPLACE,
            window: _xid,
            property: property,
            type: type,
            format: 32,
            dataLength: unchecked((uint)data.Length),
            data: data);
    }

    private void SetWMProtocols()
    {
        SetProperty(
            property: _display.Atoms.WM_PROTOCOLS,
            type: Xcb.XCB_ATOM_ATOM,
            data: new uint[] { _display.Atoms.WM_DELETE_WINDOW });
    }

    //
    // Event handler methods
    //

    private void DisplayDisposed(object? sender, EventArgs e)
    {
        _display.Disposed -= DisplayDisposed;
        Invalidate();
    }

    //
    // Events
    //

    // For handlers that need the window's ID before it is disposed.
    public event EventHandler<EventArgs>? Disposing;
}
