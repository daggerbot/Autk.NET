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

internal static class Posix
{
    //==============================================================================
    // Constants
    //==============================================================================

    // Poll event masks
    public const short POLLIN = 0x0001;
    public const short POLLPRI = 0x0002;
    public const short POLLOUT = 0x0004;
    public const short POLLERR = 0x0008;
    public const short POLLHUP = 0x0010;
    public const short POLLNVAL = 0x0020;

    //==============================================================================
    // Methods
    //==============================================================================

    [DllImport("c", SetLastError = true)]
    public static extern int close(
        int fd);

    [DllImport("c", SetLastError = true)]
    public static extern int pipe(
        [MarshalAs(UnmanagedType.LPArray, SizeConst = 2)]
        [Out]
        int[] fds);

    [DllImport("c", SetLastError = true)]
    public static extern int poll(
        [In]
        [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)]
        [Out]
        pollfd[] pollfds,
        UIntPtr nfds,
        int timeout);

    [DllImport("c", SetLastError = true)]
    public static extern IntPtr read(
        int fd,
        [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)]
        [Out]
        byte[] data,
        UIntPtr length);

    [DllImport("c", SetLastError = true)]
    public static extern IntPtr write(
        int fd,
        [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)]
        byte[] data,
        UIntPtr length);

    //==============================================================================
    // Types
    //==============================================================================

    [StructLayout(LayoutKind.Sequential)]
    public struct pollfd
    {
        public int fd;
        public short events;
        public short revents;
    }
}
