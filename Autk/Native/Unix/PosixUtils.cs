// Copyright (c) 2024 Martin Mills <daggerbot@gmail.com>
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace Autk.Native.Unix;

using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

internal static class PosixUtils
{
    //
    // Methods
    //

    public static string GetErrorString(int errorCode)
    {
        var buffer = new byte[256];
        Posix.strerror_r(errorCode, buffer, new UIntPtr(unchecked((uint)buffer.Length)));
        int length = 0;

        while (length < buffer.Length && buffer[length] != 0)
            length++;

        if (length == 0)
            return $"POSIX error code {errorCode}";

        return Encoding.Default.GetString(buffer, 0, length);
    }

    public static int Poll(PollItem[] items, int timeout)
    {
        var fds = items.Select(item => new Posix.pollfd
        {
            fd = item.FileDescriptor.FileDescriptorValue,
            events = item.EventMask,
            revents = 0,
        }).ToArray();

        int result;

        // The signature for poll() varies by platform with the definition of nfds_t.
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            result = Linux.poll(fds, new UIntPtr(unchecked((uint)fds.Length)), timeout);
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            result = FreeBSD.poll(fds, unchecked((uint)fds.Length), timeout);
        else
            throw new PlatformNotSupportedException();

        if (result < 0)
            throw new PosixException("poll", Marshal.GetLastSystemError());

        for (int i = 0; i < items.Length; i++)
        {
            items[i].ReturnEventMask = fds[i].revents;
        }

        return result;
    }

    //
    // Types
    //

    public struct PollItem
    {
        public IFileDescriptor FileDescriptor;
        public short EventMask;
        public short ReturnEventMask;
    }
}
