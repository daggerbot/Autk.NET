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

    private const string User32 = "user32";

    //==============================================================================
    // Methods
    //==============================================================================

    [DllImport(User32, SetLastError = true)]
    public static extern bool AdjustWindowRectEx(
        [In]
        [Out]
        ref RECT lpRect,
        uint dwStyle,
        bool bMenu,
        uint dwExStyle);

    [DllImport(User32, SetLastError = true)]
    public static extern IntPtr CreateWindowExW(
        uint dwExStyle,
        [MarshalAs(UnmanagedType.LPWStr)]
        string lpClassName,
        [MarshalAs(UnmanagedType.LPWStr)]
        string? lpWindowName,
        uint dwStyle,
        int X,
        int Y,
        int nWidth,
        int nHeight,
        IntPtr hWndParent,
        IntPtr hMenu,
        IntPtr hInstance,
        IntPtr lpParam);

    [DllImport(User32)]
    public static extern IntPtr DefWindowProcW(
        IntPtr hWnd,
        uint uMsg,
        UIntPtr wParam,
        IntPtr lParam);

    [DllImport(User32, SetLastError = true)]
    public static extern bool DestroyWindow(
        IntPtr hWnd);

    [DllImport(User32)]
    public static extern IntPtr DispatchMessageW(
        [In]
        ref MSG lpMsg);

    [DllImport(User32, SetLastError = true)]
    public static extern bool GetClientRect(
        IntPtr hWnd,
        out RECT lpRect);

    [DllImport(User32, SetLastError = true)]
    public static extern int GetMessageW(
        out MSG lpMsg,
        IntPtr hWnd,
        uint wMsgFilterMin,
        uint wMsgFilterMax);

    [DllImport(User32, SetLastError = true)]
    public static extern int GetWindowLongW(
        IntPtr hWnd,
        int nIndex);

    [DllImport(User32, SetLastError = true)]
    public static extern bool GetWindowRect(
        IntPtr hWnd,
        out RECT lpRect);

    [DllImport(User32, SetLastError = true)]
    public static extern int GetWindowTextW(
        IntPtr hWnd,
        [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)]
        [Out]
        char[] lpString,
        int nMaxCount);

    [DllImport(User32, SetLastError = true)]
    public static extern int GetWindowTextLengthW(
        IntPtr hWnd);

    [DllImport(User32, SetLastError = true)]
    public static extern IntPtr LoadCursorW(
        IntPtr hInstance,
        [MarshalAs(UnmanagedType.LPWStr)]
        string lpCursorName);

    [DllImport(User32, SetLastError = true)]
    public static extern bool PostThreadMessageW(
        uint idThread,
        uint Msg,
        UIntPtr wParam,
        IntPtr lParam);

    [DllImport(User32, SetLastError = true)]
    public static extern ushort RegisterClassExW(
        [In]
        ref WNDCLASSEXW wc);

    [DllImport(User32, SetLastError = true)]
    public static extern bool SetWindowPos(
        IntPtr hWnd,
        IntPtr hWndInsertAfter,
        int X,
        int Y,
        int cx,
        int cy,
        uint uFlags);

    [DllImport(User32, SetLastError = true)]
    public static extern bool SetWindowTextW(
        IntPtr hWnd,
        [MarshalAs(UnmanagedType.LPWStr)]
        string? lpString);

    [DllImport(User32)]
    public static extern bool ShowWindow(
        IntPtr hWnd,
        int nCmdShow);

    [DllImport(User32)]
    public static extern bool TranslateMessage(
        [In]
        ref MSG lpMsg);
}
