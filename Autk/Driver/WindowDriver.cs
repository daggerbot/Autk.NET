/*
 * Copyright (c) 2021 Martin Mills <daggerbot@gmail.com>
 *
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */

namespace Autk.Driver;

using System;

internal abstract class WindowDriver : IDisposable
{
    //==============================================================================
    // Properties
    //==============================================================================

    public abstract bool IsDisposed { get; }

    public abstract bool IsVisible { get; set; }

    //==============================================================================
    // Methods
    //==============================================================================

    public abstract void Dispose();

    internal void OnDisposed()
    {
        if (Disposed != null)
            Disposed.Invoke(this, new EventArgs());
    }

    internal void OnWindowEventReceived(WindowEventType eventType)
    {
        if (WindowEventReceived != null)
            WindowEventReceived.Invoke(this, new WindowEventArgs(eventType));
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
