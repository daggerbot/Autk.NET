// Copyright (c) 2024 Martin Mills <daggerbot@gmail.com>
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#pragma warning disable CS8981

namespace Autk.Native.Unix;

using System;
using System.Runtime.InteropServices;

internal static class Posix
{
    //
    // Constants
    //

    private const string Libc = "c";

    // poll() flags. Some values seem to be consistent across platforms, but not all. Let's only include the consistent
    // ones.
    public const short POLLIN = 0x0001;
    public const short POLLPRI = 0x0002;
    public const short POLLOUT = 0x0004;
    public const short POLLERR = 0x0008;
    public const short POLLHUP = 0x0010;
    public const short POLLNVAL = 0x0020;

    //
    // Methods
    //

    [DllImport(Libc, SetLastError = true)]
    public static extern int close(
        int fd);

    [DllImport(Libc, SetLastError = true)]
    public static extern void free(
        IntPtr block);

    [DllImport(Libc, SetLastError = true)]
    public static extern int pipe(
        [MarshalAs(UnmanagedType.LPArray, SizeConst = 2), Out]
        int[] fds);

    [DllImport(Libc, SetLastError = true)]
    public static extern IntPtr read(
        int fd,
        IntPtr buffer,
        UIntPtr count);

    [DllImport(Libc, SetLastError = true)]
    public static extern int strerror_r(
        int errorCode,
        [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2), Out]
        byte[] buffer,
        UIntPtr bufferSize);

    [DllImport(Libc, SetLastError = true)]
    public static extern IntPtr write(
        int fd,
        IntPtr buffer,
        UIntPtr count);

    //
    // Types
    //

    [StructLayout(LayoutKind.Sequential)]
    public struct pollfd
    {
        public int fd;
        public short events;
        public short revents;
    }
}
