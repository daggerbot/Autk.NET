// Copyright (c) 2024 Martin Mills <daggerbot@gmail.com>
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace Autk;

using System;
using System.Collections.Generic;

using Autk.Display;

public class Application : IDisposable
{
    //
    // Fields
    //

    private static Application? _instance;

    private DisplayProvider _displayProvider;
    private HashSet<Window> _windows = new HashSet<Window>();
    private bool _running = false;

    //
    // Constructors
    //

    public Application()
    {
        lock (typeof(Application))
        {
            if (_instance != null)
                throw new InvalidOperationException("Multiple application instances created");

            _instance = this;
        }

        _displayProvider = DisplayProvider.GetPlatformDefault();
    }

    //
    // Properties
    //

    public static Application Instance
    {
        get
        {
            var instance = _instance;

            if (instance == null)
                throw new InvalidOperationException("Application instance not yet created");

            return instance;
        }
    }

    internal DisplayProvider DisplayProvider => _displayProvider;

    //
    // Methods
    //

    public virtual void Dispose()
    {
    }

    protected virtual void LastWindowDisposed()
    {
        Quit();
    }

    public static void Quit()
    {
        var displayProvider = DisplayProvider.GetPlatformDefault();
        displayProvider.ThrowIfDisposed();
        displayProvider.PostQuitMessage();
    }

    internal void RegisterWindow(Window window)
    {
        _windows.Add(window);
        window.Disposed += WindowDisposed;
    }

    public void Run()
    {
        if (_running)
            throw new InvalidOperationException("Recursive main loop");

        _displayProvider.ThrowIfDisposed();
        _running = true;

        try
        {
            Start();
            _displayProvider.Run();
            Stop();
        }
        finally
        {
            _running = false;
        }
    }

    protected virtual void Start()
    {
    }

    protected virtual void Stop()
    {
    }

    //
    // Event handler methods
    //

    private void WindowDisposed(object? sender, EventArgs e)
    {
        if (sender == null)
            return;

        var window = (Window)sender;
        window.Disposed -= WindowDisposed;

        if (_windows.Remove(window) && _windows.Count == 0)
            LastWindowDisposed();
    }
}
