// Copyright (c) 2024 Martin Mills <daggerbot@gmail.com>
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace Autk.Native.X11;

using System;
using System.Runtime.InteropServices;

internal static class Xlib
{
    //
    // Constants
    //

    private const string LibX11 = "libX11.so.6";
    private const string LibX11Xcb = "libX11-xcb.so.1";

    //
    // X11 methods
    //

    [DllImport(LibX11)]
    public static extern void XCloseDisplay(
        IntPtr displayPtr);

    [DllImport(LibX11)]
    public static extern int XDefaultScreen(
        IntPtr displayPtr);

    [DllImport(LibX11)]
    public static extern IntPtr XOpenDisplay(
        [MarshalAs(UnmanagedType.LPStr)]
        string? name);

    //
    // X11-xcb methods
    //

    [DllImport(LibX11Xcb)]
    public static extern IntPtr XGetXCBConnection(
        IntPtr displayPtr);

    [DllImport(LibX11Xcb)]
    public static extern void XSetEventQueueOwner(
        IntPtr displayPtr,
        XEventQueueOwner owner);

    //
    // Types
    //

    public enum XEventQueueOwner
    {
        XlibOwnsEventQueue = 0,
        XCBOwnsEventQueue = 1,
    }
}
