/*
 * Copyright (c) 2021 Martin Mills <daggerbot@gmail.com>
 *
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */

namespace Autk;

using System;
using System.Collections.Generic;
using System.Drawing;

using Autk.Driver;

public class Window : IDisposable
{
    //==============================================================================
    // Fields
    //==============================================================================

    private static HashSet<Window> _allWindows = new HashSet<Window>();

    private ApplicationDriver _appDriver;
    private WindowDriver _driver;

    //==============================================================================
    // Constructors
    //==============================================================================

    public Window(WindowType type, Size size)
    {
        _appDriver = ApplicationDriver.GetDefaultInstance();

        _driver = _appDriver.CreateWindow(type, size);
        _driver.Disposed += Driver_Disposed;
        _driver.WindowEventReceived += Driver_WindowEventReceived;

        lock (_allWindows)
        {
            _allWindows.Add(this);
        }
    }

    //==============================================================================
    // Properties
    //==============================================================================

    public bool IsDisposed => _driver.IsDisposed;

    public bool IsVisible
    {
        get => _driver.IsVisible;
        set => _driver.IsVisible = value;
    }

    //==============================================================================
    // Methods
    //==============================================================================

    protected virtual void CloseRequested()
    {
        Dispose();
    }

    public void Dispose()
    {
        _driver.Dispose();
    }

    internal void OnDisposed()
    {
        if (Disposed != null)
            Disposed.Invoke(this, new EventArgs());
    }

    internal void ThrowIfDisposed()
    {
        _driver.ThrowIfDisposed();
    }

    private void Driver_Disposed(object? sender, EventArgs e)
    {
        _driver.Disposed -= Driver_Disposed;
        OnDisposed();

        lock (_allWindows)
        {
            if (_allWindows.Remove(this) && _allWindows.Count == 0)
            {
                var app = Application.Instance;

                if (app != null)
                    app.LastWindowClosed();
            }
        }
    }

    private void Driver_WindowEventReceived(object? sender, WindowEventArgs e)
    {
        switch (e.EventType)
        {
            case WindowEventType.CloseRequested:
                CloseRequested();
                break;

            default:
                break;
        }
    }

    //==============================================================================
    // Events
    //==============================================================================

    public event EventHandler<EventArgs>? Disposed;
}
