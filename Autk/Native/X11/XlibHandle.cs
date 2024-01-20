// Copyright (c) 2024 Martin Mills <daggerbot@gmail.com>
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace Autk.Native.X11;

using System;
using System.Runtime.InteropServices;

internal class XlibHandle : SafeHandle
{
    //
    // Constructors
    //

    public XlibHandle()
        : base(IntPtr.Zero, true)
    {
    }

    public XlibHandle(IntPtr displayPtr)
        : this()
    {
        SetHandle(displayPtr);
    }

    //
    // Properties
    //

    public override bool IsInvalid => handle == IntPtr.Zero;

    public IntPtr XlibDisplayPtr => handle;

    //
    // Methods
    //

    protected override bool ReleaseHandle()
    {
        if (handle != IntPtr.Zero)
        {
            Xlib.XCloseDisplay(handle);
            handle = IntPtr.Zero;
        }

        return true;
    }
}
