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

    // CreateWindow values
    public const int CW_USEDEFAULT = unchecked((int)0x80000000);

    // FormatMessage flags
    public const uint FORMAT_MESSAGE_MAX_WIDTH_MASK = 0x00FF;
    public const uint FORMAT_MESSAGE_ALLOCATE_BUFFER = 0x0100;
    public const uint FORMAT_MESSAGE_IGNORE_INSERTS = 0x0200;
    public const uint FORMAT_MESSAGE_FROM_STRING = 0x0400;
    public const uint FORMAT_MESSAGE_FROM_HMODULE = 0x0800;
    public const uint FORMAT_MESSAGE_FROM_SYSTEM = 0x1000;
    public const uint FORMAT_MESSAGE_ARGUMENT_ARRAY = 0x2000;

    // GetWindowLong indices
    public const int GWL_USERDATA = -21;
    public const int GWL_EXSTYLE = -20;
    public const int GWL_STYLE = -16;
    public const int GWL_ID = -12;
    public const int GWL_HWNDPARENT = -8;
    public const int GWL_HINSTANCE = -6;
    public const int GWL_WNDPROC = -4;

    // SetWindowPos flags
    public const uint SWP_NOSIZE = 0x0001;
    public const uint SWP_NOMOVE = 0x0002;
    public const uint SWP_NOZORDER = 0x0004;
    public const uint SWP_NOREDRAW = 0x0008;
    public const uint SWP_NOACTIVATE = 0x0010;
    public const uint SWP_DRAWFRAME = 0x0020;
    public const uint SWP_FRAMECHANGED = 0x0020;
    public const uint SWP_SHOWWINDOW = 0x0040;
    public const uint SWP_HIDEWINDOW = 0x0080;
    public const uint SWP_NOCOPYBITS = 0x0100;
    public const uint SWP_NOOWNERZORDER = 0x0200;
    public const uint SWP_NOREPOSITION = 0x0200;
    public const uint SWP_NOSENDCHANGING = 0x0400;
    public const uint SWP_DEFERERASE = 0x2000;
    public const uint SWP_ASYNCWINDOWPOS = 0x4000;

    // ShowWindow commands
    public const int SW_HIDE = 0;
    public const int SW_NORMAL = 1;
    public const int SW_SHOWNORMAL = 1;
    public const int SW_SHOWMINIMIZED = 2;
    public const int SW_MAXIMIZE = 3;
    public const int SW_SHOWMAXIMIZED = 3;
    public const int SW_SHOWNOACTIVATE = 4;
    public const int SW_SHOW = 5;
    public const int SW_MINIMIZE = 6;
    public const int SW_SHOWMINNOACTIVE = 7;
    public const int SW_SHOWNA = 8;
    public const int SW_RESTORE = 9;
    public const int SW_SHOWDEFAULT = 10;
    public const int SW_FORCEMINIMIZE = 11;

    // System cursor names
    public const string IDC_ARROW = "#32512";
    public const string IDC_IBEAM = "#32513";
    public const string IDC_WAIT = "#32514";
    public const string IDC_CROSS = "#32515";
    public const string IDC_UPARROW = "#32516";
    public const string IDC_SIZE = "#32640";
    public const string IDC_ICON = "#32641";
    public const string IDC_SIZENWSE = "#32642";
    public const string IDC_SIZENESW = "#32643";
    public const string IDC_SIZEWE = "#32644";
    public const string IDC_SIZENS = "#32645";
    public const string IDC_SIZEALL = "#32646";
    public const string IDC_NO = "#32648";
    public const string IDC_HAND = "#32649";
    public const string IDC_APPSTARTING = "#32650";
    public const string IDC_HELP = "#32651";

    // Window message types
    public const uint WM_DESTROY = 0x0002;
    public const uint WM_MOVE = 0x0003;
    public const uint WM_SIZE = 0x0005;
    public const uint WM_CLOSE = 0x0010;
    public const uint WM_QUIT = 0x0012;
    public const uint WM_SHOWWINDOW = 0x0018;
    public const uint WM_SIZING = 0x0214;
    public const uint WM_USER = 0x0400;

    // Window style flags
    public const uint WS_OVERLAPPED = 0;
    public const uint WS_TILED = 0;
    public const uint WS_MAXIMIZEBOX = 0x00010000;
    public const uint WS_TABSTOP = 0x00010000;
    public const uint WS_GROUP = 0x00020000;
    public const uint WS_MINIMIZEBOX = 0x00020000;
    public const uint WS_SIZEBOX = 0x00040000;
    public const uint WS_THICKFRAME = 0x00040000;
    public const uint WS_SYSMENU = 0x00080000;
    public const uint WS_HSCROLL = 0x00100000;
    public const uint WS_VSCROLL = 0x00200000;
    public const uint WS_DLGFRAME = 0x00400000;
    public const uint WS_BORDER = 0x00800000;
    public const uint WS_CAPTION = 0x00C00000;
    public const uint WS_MAXIMIZE = 0x01000000;
    public const uint WS_CLIPCHILDREN = 0x02000000;
    public const uint WS_CLIPSIBLINGS = 0x04000000;
    public const uint WS_DISABLED = 0x08000000;
    public const uint WS_VISIBLE = 0x10000000;
    public const uint WS_ICONIC = 0x20000000;
    public const uint WS_MINIMIZE = 0x20000000;
    public const uint WS_CHILD = 0x40000000;
    public const uint WS_CHILDWINDOW = 0x40000000;
    public const uint WS_POPUP = 0x80000000;

    public const uint WS_OVERLAPPEDWINDOW =
        WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX;
    public const uint WS_POPUPWINDOW =
        WS_POPUP | WS_BORDER | WS_SYSMENU;
    public const uint WS_TILEDWINDOW =
        WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX;

    //==============================================================================
    // Methods
    //==============================================================================

    public static ushort HIWORD(int n)
    {
        return HIWORD(unchecked((uint)n));
    }

    public static ushort HIWORD(uint n)
    {
        return unchecked((ushort)(n >> 16));
    }

    public static ushort HIWORD(IntPtr n)
    {
        return HIWORD(unchecked((uint)n.ToInt64()));
    }

    public static ushort HIWORD(UIntPtr n)
    {
        return HIWORD(unchecked((uint)n.ToUInt64()));
    }

    public static ushort LOWORD(int n)
    {
        return LOWORD(unchecked((uint)n));
    }

    public static ushort LOWORD(uint n)
    {
        return unchecked((ushort)n);
    }

    public static ushort LOWORD(IntPtr n)
    {
        return LOWORD(unchecked((uint)n.ToInt64()));
    }

    public static ushort LOWORD(UIntPtr n)
    {
        return LOWORD(unchecked((uint)n.ToUInt64()));
    }

    //==============================================================================
    // Types
    //==============================================================================

    [StructLayout(LayoutKind.Sequential)]
    public struct MSG
    {
        public IntPtr hwnd;
        public uint message;
        public UIntPtr wParam;
        public IntPtr lParam;
        public uint time;
        public POINT pt;
        public uint lPrivate;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int x;
        public int y;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct WNDCLASSEXW
    {
        public uint cbSize;
        public uint style;
        public WNDPROC lpfnWndProc;
        public int cbClsExtra;
        public int cbWndExtra;
        public IntPtr hInstance;
        public IntPtr hIcon;
        public IntPtr hCursor;
        public IntPtr hbrBackground;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string lpszMenuName;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string lpszClassName;
        public IntPtr hIconSm;
    }

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate IntPtr WNDPROC(IntPtr hWnd, uint uMsg, UIntPtr wParam, IntPtr lParam);
}
