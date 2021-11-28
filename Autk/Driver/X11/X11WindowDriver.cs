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
using System.Runtime.InteropServices;

using Autk.Native.X11;

internal class X11WindowDriver : WindowDriver
{
    //==============================================================================
    // Fields
    //==============================================================================

    private X11ApplicationDriver _app;
    private IntPtr _xcb;
    private uint _xid;
    private bool _visible;

    //==============================================================================
    // Constructors
    //==============================================================================

    public X11WindowDriver(X11ApplicationDriver app, WindowType type, Size size)
    {
        size.Width = Math.Clamp(size.Width, 1, 0xFFFF);
        size.Height = Math.Clamp(size.Height, 1, 0xFFFF);

        app.ThrowIfDisposed();
        app.Disposed += App_Disposed;
        _app = app;
        _xcb = app.XcbConnectionPointer;
        _xid = Xcb.xcb_generate_id(_xcb);

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
            valueMask: 0,
            values: null);

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
        _visible = false;

        OnDisposed();
    }

    private void SetAtomProperty(uint property, uint[]? value)
    {
        if (IsDisposed)
            return;

        int length = 0;
        var dataPtr = Marshal.AllocHGlobal(sizeof(uint));

        try
        {
            if (value != null)
            {
                length = value.Length;

                for (int i = 0; i < length; ++i)
                    Marshal.WriteInt32(dataPtr + i * sizeof(uint), unchecked((int)value[i]));
            }

            Xcb.xcb_change_property(
                connection: _xcb,
                mode: (byte)Xcb.xcb_prop_mode_t.XCB_PROP_MODE_REPLACE,
                window: _xid,
                property: property,
                type: Xcb.XCB_ATOM_ATOM,
                format: 32,
                dataLength: checked((uint)length),
                data: dataPtr);
        }
        finally
        {
            Marshal.FreeHGlobal(dataPtr);
        }
    }

    private void SetProtocols()
    {
        SetAtomProperty(_app.Atoms.WM_PROTOCOLS, new uint[]
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

        SetAtomProperty(_app.Atoms._NET_WM_WINDOW_TYPE, new uint[] { windowTypeAtom });
    }

    private void App_Disposed(object? sender, EventArgs e)
    {
        Expire();
    }
}
