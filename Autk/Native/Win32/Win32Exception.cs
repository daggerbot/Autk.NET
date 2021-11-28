/*
 * Copyright (c) 2021 Martin Mills <daggerbot@gmail.com>
 *
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */

namespace Autk.Native.Win32;

using System;
using System.Runtime.InteropServices;

public class Win32Exception : NativeRuntimeException
{
    //==============================================================================
    // Constructors
    //==============================================================================

    internal Win32Exception(string message, Exception? innerException = null)
        : base(message, innerException)
    {
    }

    internal Win32Exception(string message, int errorCode, Exception? innerException = null)
        : this($"{message}: {GetErrorMessage(errorCode)}", innerException)
    {
    }

    //==============================================================================
    // Methods
    //==============================================================================

    internal static string GetErrorMessage(int errorCode)
    {
        Winapi.FormatMessageW(
            dwFlags: Winapi.FORMAT_MESSAGE_ALLOCATE_BUFFER | Winapi.FORMAT_MESSAGE_FROM_SYSTEM,
            lpSource: IntPtr.Zero,
            dwMessageId: unchecked((uint)errorCode),
            dwLanguageId: 0,
            lpBuffer: out var messagePtr,
            nSize: 0,
            Arguments: IntPtr.Zero);

        if (messagePtr == IntPtr.Zero)
            return $"Win32 error code {errorCode}";
        else
        {
            try
            {
                return Marshal.PtrToStringUni(messagePtr)!;
            }
            finally
            {
                Winapi.LocalFree(messagePtr);
            }
        }
    }
}
