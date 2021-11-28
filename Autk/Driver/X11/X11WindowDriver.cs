/*
 * Copyright (c) 2021 Martin Mills <daggerbot@gmail.com>
 *
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */

namespace Autk.Driver.X11;

using System;
using System.Drawing;
using System.Text;

using Autk.Native.X11;

internal class X11WindowDriver : WindowDriver
{
    //==============================================================================
    // Constants
    //==============================================================================

    private const int MaxDimension = 0xFFFF;
    private const int MaxLocation = 0x7FFF;
    private const int MinLocation = -0x8000;

    //==============================================================================
    // Fields
    //==============================================================================

    private X11ApplicationDriver _app;
    private IntPtr _xcb;
    private uint _xid;
    private string? _title;
    private bool _visible;
    private Size _size;
    private Point _location;

    //==============================================================================
    // Constructors
    //==============================================================================

    public X11WindowDriver(X11ApplicationDriver app, WindowType type, Size size)
    {
        size.Width = Math.Clamp(size.Width, 1, MaxDimension);
        size.Height = Math.Clamp(size.Height, 1, MaxDimension);

        app.ThrowIfDisposed();
        app.Disposed += App_Disposed;
        _app = app;
        _xcb = app.XcbConnectionPointer;
        _xid = Xcb.xcb_generate_id(_xcb);

        uint valueMask = Xcb.XCB_CW_EVENT_MASK;
        var values = new uint[]
        {
            Xcb.XCB_EVENT_MASK_STRUCTURE_NOTIFY,
        };

        Xcb.xcb_create_window(
            connection: _xcb,
            depth: app.DefaultDepth.depth,
            id: _xid,
            parent: app.DefaultScreen.root,
            x: 0,
            y: 0,
            width: (ushort)size.Width,
            height: (ushort)size.Height,
            borderWidth: 0,
            class_: (ushort)Xcb.xcb_window_class_t.XCB_WINDOW_CLASS_INPUT_OUTPUT,
            visual: app.DefaultVisual.visual_id,
            valueMask: valueMask,
            values: values);

        try
        {
            SetProtocols();
            SetWindowType(type);
            app.RegisterWindow(this);
        }
        catch
        {
            Dispose();
        }
    }

    //==============================================================================
    // Properties
    //==============================================================================

    public override bool IsDisposed => _xid == 0;

    public override bool IsVisible
    {
        get => _visible;

        set
        {
            if (value)
            {
                ThrowIfDisposed();
                Xcb.xcb_map_window(_xcb, _xid);
            }
            else if (!IsDisposed)
                Xcb.xcb_unmap_window(_xcb, _xid);
        }
    }

    public override Point Location
    {
        get => _location;

        set
        {
            if (IsDisposed)
                return;

            Xcb.xcb_configure_window(
                connection: _xcb,
                window: _xid,
                valueMask: Xcb.XCB_CONFIG_WINDOW_X | Xcb.XCB_CONFIG_WINDOW_Y,
                values: new uint[]
                {
                    unchecked((uint)Math.Clamp(value.X, MinLocation, MaxLocation)),
                    unchecked((uint)Math.Clamp(value.Y, MinLocation, MaxLocation)),
                });
        }
    }

    public override Size Size
    {
        get => _size;

        set
        {
            if (IsDisposed)
                return;

            Xcb.xcb_configure_window(
                connection: _xcb,
                window: _xid,
                valueMask: Xcb.XCB_CONFIG_WINDOW_WIDTH | Xcb.XCB_CONFIG_WINDOW_HEIGHT,
                values: new uint[]
                {
                    unchecked((uint)Math.Clamp(value.Width, 1, MaxDimension)),
                    unchecked((uint)Math.Clamp(value.Height, 1, MaxDimension)),
                });
        }
    }

    public override string? Title
    {
        get => _title;

        set
        {
            if (IsDisposed)
                return;

            byte[]? bytes = null;

            if (value != null)
                bytes = Encoding.UTF8.GetBytes(value);

            SetProperty(Xcb.XCB_ATOM_WM_NAME, Xcb.XCB_ATOM_STRING, bytes);
            SetProperty(Xcb.XCB_ATOM_WM_ICON_NAME, Xcb.XCB_ATOM_STRING, bytes);
            SetProperty(_app.Atoms._NET_WM_NAME, _app.Atoms.UTF8_STRING, bytes);
            SetProperty(_app.Atoms._NET_WM_ICON_NAME, _app.Atoms.UTF8_STRING, bytes);
            _title = value;
        }
    }

    public IntPtr XcbConnectionPointer => _xcb;

    public uint Xid => _xid;

    //==============================================================================
    // Methods
    //==============================================================================

    public override void Dispose()
    {
        if (_xcb != IntPtr.Zero && _xid != 0)
            Xcb.xcb_destroy_window(_xcb, _xid);

        Expire();
    }

    public void Expire()
    {
        _app.Disposed -= App_Disposed;
        _xcb = IntPtr.Zero;
        _xid = 0;
        _title = null;
        _visible = false;
        _size = Size.Empty;
        _location = Point.Empty;

        OnDisposed();
    }

    internal override void OnWindowEventReceived(WindowEventArgs e)
    {
        switch (e.EventType)
        {
            case WindowEventType.Hide:
                _visible = false;
                break;

            case WindowEventType.Move:
                _location = e.Location!.Value;
                break;

            case WindowEventType.Resize:
                _size = e.Size!.Value;
                break;

            case WindowEventType.Show:
                _visible = true;
                break;

            default:
                break;
        }

        base.OnWindowEventReceived(e);
    }

    private void SetProperty(uint property, uint type, byte[]? value)
    {
        ThrowIfDisposed();

        int length = 0;

        if (value != null)
            length = value.Length;

        Xcb.xcb_change_property(
            connection: _xcb,
            mode: (byte)Xcb.xcb_prop_mode_t.XCB_PROP_MODE_REPLACE,
            window: _xid,
            property: property,
            type: type,
            format: 8,
            dataLength: checked((uint)length),
            data: value);
    }

    private void SetProperty(uint property, uint type, uint[]? value)
    {
        ThrowIfDisposed();

        int length = 0;

        if (value != null)
            length = value.Length;

        Xcb.xcb_change_property(
            connection: _xcb,
            mode: (byte)Xcb.xcb_prop_mode_t.XCB_PROP_MODE_REPLACE,
            window: _xid,
            property: property,
            type: type,
            format: 32,
            dataLength: checked((uint)length),
            data: value);
    }

    private void SetProtocols()
    {
        SetProperty(_app.Atoms.WM_PROTOCOLS, Xcb.XCB_ATOM_ATOM, new uint[]
        {
            _app.Atoms.WM_DELETE_WINDOW,
        });
    }

    private void SetWindowType(WindowType type)
    {
        uint windowTypeAtom;

        switch (type)
        {
            case WindowType.Normal:
                windowTypeAtom = _app.Atoms._NET_WM_WINDOW_TYPE_NORMAL;
                break;
            default:
                throw new ArgumentException($"Invalid or unimplemented window type: {type}");
        }

        SetProperty(_app.Atoms._NET_WM_WINDOW_TYPE, Xcb.XCB_ATOM_ATOM, new uint[] { windowTypeAtom });
    }

    private void App_Disposed(object? sender, EventArgs e)
    {
        Expire();
    }
}
