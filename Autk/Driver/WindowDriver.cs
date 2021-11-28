/*
 * Copyright (c) 2021 Martin Mills <daggerbot@gmail.com>
 *
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */

namespace Autk.Driver;

using System;
using System.Drawing;

internal abstract class WindowDriver : IDisposable
{
    //==============================================================================
    // Properties
    //==============================================================================

    public abstract bool IsDisposed { get; }

    public abstract bool IsVisible { get; set; }

    public abstract Point Location { get; set; }

    public abstract Size Size { get; set; }

    public abstract string? Title { get; set; }

    //==============================================================================
    // Methods
    //==============================================================================

    public abstract void Dispose();

    internal void OnDisposed()
    {
        if (Disposed != null)
            Disposed.Invoke(this, new EventArgs());
    }

    internal void OnWindowEventReceived(WindowEventArgs e)
    {
        if (WindowEventReceived != null)
            WindowEventReceived.Invoke(this, e);
    }

    private void SetAtomProperty(uint property, uint value)
    {
        if (IsDisposed)
            return;
    }

    internal void ThrowIfDisposed()
    {
        if (IsDisposed)
            throw new ObjectDisposedException("Window is disposed");
    }

    //==============================================================================
    // Events
    //==============================================================================

    public event EventHandler<EventArgs>? Disposed;

    public event EventHandler<WindowEventArgs>? WindowEventReceived;
}
