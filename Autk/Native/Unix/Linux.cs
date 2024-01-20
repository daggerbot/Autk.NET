// Copyright (c) 2024 Martin Mills <daggerbot@gmail.com>
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace Autk.Native.Unix;

using System;
using System.Runtime.InteropServices;

internal static class Linux
{
    //
    // Constants
    //

    private const string Libc = "libc.so.6";

    //
    // Methods
    //

    [DllImport(Libc, SetLastError = true)]
    public static extern int poll(
        [MarshalAs(UnmanagedType.LPArray), In, Out]
        Posix.pollfd[] fds,
        UIntPtr nfds,
        int timeout);
}
