/*
 * Copyright (c) 2021 Martin Mills <daggerbot@gmail.com>
 *
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */

namespace Autk.Native.X11;

using System;
using System.Runtime.InteropServices;

internal static partial class Xlib
{
    //==============================================================================
    // Constants
    //==============================================================================

    private const string LibX11 = "libX11.so.6";

    //==============================================================================
    // Methods
    //==============================================================================

    [DllImport(LibX11)]
    public static extern void XCloseDisplay(
        IntPtr display);

    [DllImport(LibX11)]
    public static extern int XDefaultScreen(
        IntPtr display);

    [DllImport(LibX11)]
    public static extern IntPtr XOpenDisplay(
        [MarshalAs(UnmanagedType.LPStr)]
        string? name);
}
