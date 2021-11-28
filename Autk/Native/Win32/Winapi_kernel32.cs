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

internal static partial class Winapi
{
    //==============================================================================
    // Constants
    //==============================================================================

    private const string Kernel32 = "kernel32";

    //==============================================================================
    // Methods
    //==============================================================================

    [DllImport(Kernel32, SetLastError = true)]
    public static extern uint FormatMessageW(
        uint dwFlags,
        IntPtr lpSource,
        uint dwMessageId,
        uint dwLanguageId,
        out IntPtr lpBuffer,
        uint nSize,
        IntPtr Arguments);

    [DllImport(Kernel32)]
    public static extern uint GetCurrentThreadId();

    [DllImport(Kernel32, SetLastError = true)]
    public static extern IntPtr GetModuleHandleW(
        [MarshalAs(UnmanagedType.LPWStr)]
        string? lpModuleName);

    [DllImport(Kernel32, SetLastError = true)]
    public static extern IntPtr LocalFree(
        IntPtr hMem);
}
