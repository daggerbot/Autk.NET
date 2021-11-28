/*
 * Copyright (c) 2021 Martin Mills <daggerbot@gmail.com>
 *
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */

namespace Autk.Driver.Win32;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;

using Autk.Native.Win32;

internal class Win32ApplicationDriver : ApplicationDriver
{
    //==============================================================================
    // Fields
    //==============================================================================

    private static Win32ApplicationDriver? _instance;

    private uint? _mainThreadId;
    private Queue<Winapi.MSG> _pendingMessages = new Queue<Winapi.MSG>();
    private ConcurrentQueue<Action> _actionQueue = new ConcurrentQueue<Action>();

    //==============================================================================
    // Constructors
    //==============================================================================

    private Win32ApplicationDriver()
    {
        lock (typeof(Win32ApplicationDriver))
        {
            if (_instance != null)
                throw new InvalidOperationException("Multiple Win32 application drivers created");

            _instance = this;
        }
    }

    //==============================================================================
    // Properties
    //==============================================================================

    public override bool IsDisposed => false;

    //==============================================================================
    // Methods
    //==============================================================================

    public override WindowDriver CreateWindow(WindowType type, Size size)
    {
        return new Win32WindowDriver(type, size);
    }

    public override void Dispose()
    {
        throw new System.NotImplementedException();
    }

    public static Win32ApplicationDriver GetInstance()
    {
        if (_instance == null)
        {
            lock (typeof(Win32ApplicationDriver))
            {
                if (_instance == null)
                    new Win32ApplicationDriver();
            }
        }

        return _instance!;
    }

    public static IntPtr GetModuleHandle()
    {
        var hInstance = Winapi.GetModuleHandleW(null);

        if (hInstance == IntPtr.Zero)
            throw new Win32Exception("GetModuleHandleW()", Marshal.GetLastWin32Error());

        return hInstance;
    }

    public override void Quit()
    {
        lock (this)
        {
            if (_mainThreadId.HasValue)
            {
                if (!Winapi.PostThreadMessageW(_mainThreadId.Value, Winapi.WM_QUIT, new UIntPtr(), new IntPtr()))
                {
                    throw new Win32Exception("PostThreadMessageW()", Marshal.GetLastWin32Error());
                }
            }
            else
                _pendingMessages.Enqueue(new Winapi.MSG { message = Winapi.WM_QUIT });
        }
    }

    public override void Run()
    {
        Winapi.MSG msg;

        lock (this)
        {
            if (!_mainThreadId.HasValue)
                _mainThreadId = Winapi.GetCurrentThreadId();
            else if (_mainThreadId.Value != Winapi.GetCurrentThreadId())
                throw new InvalidOperationException("Application.Run() called from multiple threads");

            while (_pendingMessages.TryDequeue(out msg))
            {
                switch (msg.message)
                {
                    case Winapi.WM_QUIT:
                        return;

                    case Winapi.WM_USER:
                        if (_actionQueue.TryDequeue(out var action))
                            action.Invoke();
                        break;

                    default:
                        break;
                }
            }
        }

        for (; ; )
        {
            int pollResult = Winapi.GetMessageW(out msg, IntPtr.Zero, 0, 0);

            switch (pollResult)
            {
                case -1:
                    throw new Win32Exception("GetMessageW()", Marshal.GetLastWin32Error());

                case 0:
                    return;

                default:
                    if (msg.message == Winapi.WM_USER && msg.hwnd == IntPtr.Zero)
                    {
                        if (_actionQueue.TryDequeue(out var action))
                            action.Invoke();
                    }
                    else
                    {
                        Winapi.TranslateMessage(ref msg);
                        Winapi.DispatchMessageW(ref msg);
                    }
                    break;
            }
        }
    }
}
