// Copyright (c) 2024 Martin Mills <daggerbot@gmail.com>
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace Autk;

using System;
using System.Drawing;

using Autk.Display;

public class Window : IDisposable
{
    //
    // Fields
    //

    private WindowProvider _provider;

    //
    // Constructors
    //

    public Window(WindowStyle style, Size size)
    {
        var application = Application.Instance;
        var display = application.DisplayProvider;

        _provider = display.CreateWindow(this, style, size);

        application.RegisterWindow(this);
    }

    //
    // Properties
    //

    public bool IsDisposed => _provider.IsDisposed;

    public bool IsVisible
    {
        get => _provider.IsVisible;
        set
        {
            if (value)
            {
                ThrowIfDisposed();
                _provider.IsVisible = true;
            }
            else if (!IsDisposed)
                _provider.IsVisible = false;
        }
    }

    //
    // Methods
    //

    protected internal virtual void CloseRequested()
    {
        Dispose();
    }

    public void Dispose()
    {
        _provider.Dispose();
    }

    protected internal void OnDisposed()
    {
        Disposed?.Invoke(this, EventArgs.Empty);
    }

    protected internal void OnVisibilityChanged(bool visible)
    {
        if (VisibilityChanged != null)
        {
            if (visible)
                VisibilityChanged.Invoke(this, VisibilityEventArgs.Shown);
            else
                VisibilityChanged.Invoke(this, VisibilityEventArgs.Hidden);
        }
    }

    public void ThrowIfDisposed()
    {
        _provider.ThrowIfDisposed();
    }

    //
    // Events
    //

    public event EventHandler<EventArgs>? Disposed;

    public event EventHandler<VisibilityEventArgs>? VisibilityChanged;

    //
    // Types
    //

    public class VisibilityEventArgs : EventArgs
    {
        public static readonly VisibilityEventArgs Hidden = new VisibilityEventArgs(false);
        public static readonly VisibilityEventArgs Shown = new VisibilityEventArgs(true);

        public readonly bool IsVisible;

        internal VisibilityEventArgs(bool visible)
        {
            this.IsVisible = visible;
        }
    }
}
