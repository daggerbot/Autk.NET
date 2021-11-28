/*
 * Copyright (c) 2021 Martin Mills <daggerbot@gmail.com>
 *
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */

namespace Autk.Native;

using System;
using System.Runtime.InteropServices;

internal static class Libc
{
    //==============================================================================
    // Methods
    //==============================================================================

    [DllImport("c")]
    public static extern void free(
        IntPtr ptr);

    [DllImport("c")]
    [return: MarshalAs(UnmanagedType.LPStr)]
    public static extern string strerror(
        int errorCode);
}
