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

    private const string LibX11Xcb = "libX11-xcb.so.1";

    //==============================================================================
    // Methods
    //==============================================================================

    [DllImport(LibX11Xcb)]
    public static extern IntPtr XGetXCBConnection(
        IntPtr display);

    [DllImport(LibX11Xcb)]
    public static extern void XSetEventQueueOwner(
        IntPtr display,
        XEventQueueOwner owner);
}
