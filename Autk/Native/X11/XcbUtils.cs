// Copyright (c) 2024 Martin Mills <daggerbot@gmail.com>
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace Autk.Native.X11;

using System;
using System.Runtime.InteropServices;

internal static class XcbUtils
{
    //
    // Constants
    //

    private const int ClientMessageDataOffset = 12;

    //
    // Methods
    //

    public static sbyte GetClientMessageInt8(IntPtr eventPtr, int index)
    {
        var value = GetClientMessageUInt8(eventPtr, index);
        return unchecked((sbyte)value);
    }

    public static short GetClientMessageInt16(IntPtr eventPtr, int index)
    {
        if (index < 0 || index >= 10)
            throw new ArgumentOutOfRangeException(nameof(index));

        return Marshal.ReadInt16(eventPtr + ClientMessageDataOffset + index * 2);
    }

    public static int GetClientMessageInt32(IntPtr eventPtr, int index)
    {
        if (index < 0 || index >= 5)
            throw new ArgumentOutOfRangeException(nameof(index));

        return Marshal.ReadInt16(eventPtr + ClientMessageDataOffset + index * 4);
    }

    public static byte GetClientMessageUInt8(IntPtr eventPtr, int index)
    {
        if (index < 0 || index >= 20)
            throw new ArgumentOutOfRangeException(nameof(index));

        return Marshal.ReadByte(eventPtr + ClientMessageDataOffset + index);
    }

    public static ushort GetClientMessageUInt16(IntPtr eventPtr, int index)
    {
        var value = GetClientMessageInt16(eventPtr, index);
        return unchecked((ushort)value);
    }

    public static uint GetClientMessageUInt32(IntPtr eventPtr, int index)
    {
        var value = GetClientMessageInt32(eventPtr, index);
        return unchecked((uint)value);
    }
}
