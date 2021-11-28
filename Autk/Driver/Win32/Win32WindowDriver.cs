/*
 * Copyright (c) 2021 Martin Mills <daggerbot@gmail.com>
 *
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */

namespace Autk.Driver.Win32;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;

using Autk.Native.Win32;

internal class Win32WindowDriver : WindowDriver
{
    //==============================================================================
    // Constants
    //==============================================================================

    private const string WindowClassName = "AutkWindow";

    //==============================================================================
    // Fields
    //==============================================================================

    private static ushort _windowClassAtom;
    private static Dictionary<IntPtr, Win32WindowDriver> _windowMap = new Dictionary<IntPtr, Win32WindowDriver>();

    private IntPtr _hwnd;

    //==============================================================================
    // Constructors
    //==============================================================================

    public Win32WindowDriver(WindowType windowType, Size size)
    {
        uint style;
        uint exStyle = 0;

        switch (windowType)
        {
            case WindowType.Normal:
                style = Winapi.WS_OVERLAPPEDWINDOW;
                break;
            default:
                throw new ArgumentException("Invalid or unimplemented window type");
        }

        var rect = new Winapi.RECT
        {
            left = 0,
            top = 0,
            right = Math.Clamp(size.Width, 1, 0xFFFF),
            bottom = Math.Clamp(size.Height, 1, 0xFFFF),
        };

        if (!Winapi.AdjustWindowRectEx(ref rect, style, bMenu: false, exStyle))
            throw new Win32Exception("AdjustWindowRectEx()", Marshal.GetLastWin32Error());

        RegisterWindowClass();

        _hwnd = Winapi.CreateWindowExW(
            dwExStyle: exStyle,
            lpClassName: WindowClassName,
            lpWindowName: null,
            dwStyle: style,
            X: Winapi.CW_USEDEFAULT,
            Y: Winapi.CW_USEDEFAULT,
            nWidth: rect.right - rect.left,
            nHeight: rect.bottom - rect.top,
            hWndParent: IntPtr.Zero,
            hMenu: IntPtr.Zero,
            hInstance: Win32ApplicationDriver.GetModuleHandle(),
            lpParam: IntPtr.Zero);

        if (_hwnd == IntPtr.Zero)
            throw new Win32Exception("CreateWindowExW()", Marshal.GetLastWin32Error());

        try
        {
            _windowMap[_hwnd] = this;
        }
        catch
        {
            Dispose();
            throw;
        }
    }

    //==============================================================================
    // Properties
    //==============================================================================

    public override bool IsDisposed => _hwnd == IntPtr.Zero;

    public override bool IsVisible
    {
        get => !IsDisposed && (GetWindowStyle() & Winapi.WS_VISIBLE) != 0;

        set
        {
            if (value)
            {
                ThrowIfDisposed();
                Winapi.ShowWindow(_hwnd, Winapi.SW_SHOW);
            }
            else if (!IsDisposed)
                Winapi.ShowWindow(_hwnd, Winapi.SW_HIDE);
        }
    }

    //==============================================================================
    // Methods
    //==============================================================================

    public override void Dispose()
    {
        if (_hwnd != IntPtr.Zero)
        {
            // No need to do anything else here.
            // The WNDPROC will take care of it when handling WM_DESTROY.
            Winapi.DestroyWindow(_hwnd);
        }
    }

    private void Expire()
    {
        bool notify = !IsDisposed;

        if (_hwnd != IntPtr.Zero)
        {
            _windowMap.Remove(_hwnd);
            _hwnd = IntPtr.Zero;
        }

        if (notify)
            OnDisposed();
    }

    public static Win32WindowDriver? GetWindow(IntPtr hwnd)
    {
        return _windowMap.GetValueOrDefault(hwnd);
    }

    private int GetWindowLong(int index)
    {
        ThrowIfDisposed();
        int value = Winapi.GetWindowLongW(_hwnd, index);
        int errorCode = Marshal.GetLastWin32Error();

        if (errorCode != 0)
            throw new Win32Exception("GetWindowLongW()", errorCode);

        return value;
    }

    private uint GetWindowStyle()
    {
        return unchecked((uint)GetWindowLong(Winapi.GWL_STYLE));
    }

    private static void RegisterWindowClass()
    {
        if (_windowClassAtom != 0)
            return;

        var wc = new Winapi.WNDCLASSEXW
        {
            cbSize = (uint)Marshal.SizeOf<Winapi.WNDCLASSEXW>(),
            lpfnWndProc = WndProc,
            hInstance = Win32ApplicationDriver.GetModuleHandle(),
            lpszClassName = WindowClassName,
        };

        wc.hCursor = Winapi.LoadCursorW(IntPtr.Zero, Winapi.IDC_ARROW);

        if (wc.hCursor == IntPtr.Zero)
            throw new Win32Exception("LoadCursorW()", Marshal.GetLastWin32Error());

        _windowClassAtom = Winapi.RegisterClassExW(ref wc);

        if (_windowClassAtom == 0)
            throw new Win32Exception("RegisterClassExW()", Marshal.GetLastWin32Error());
    }

    private static IntPtr WndProc(IntPtr hwnd, uint message, UIntPtr wparam, IntPtr lparam)
    {
        switch (message)
        {
            case Winapi.WM_CLOSE:
                {
                    var window = GetWindow(hwnd);
                    if (window != null)
                        window.OnWindowEventReceived(WindowEventType.CloseRequested);
                }
                return IntPtr.Zero;

            case Winapi.WM_DESTROY:
                {
                    var window = GetWindow(hwnd);
                    if (window != null)
                        window.Expire();
                }
                return IntPtr.Zero;

            default:
                return Winapi.DefWindowProcW(hwnd, message, wparam, lparam);
        }
    }
}
