// Copyright (c) 2024 Martin Mills <daggerbot@gmail.com>
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace Autk.Display;

using System;

internal abstract class WindowProvider : IDisposable
{
    //
    // Fields
    //

    private Window _owner;

    //
    // Constructors
    //

    protected WindowProvider(Window owner)
    {
        _owner = owner;
    }

    //
    // Properties
    //

    public abstract bool IsDisposed { get; }

    public abstract bool IsVisible { get; set; }

    public Window Owner => _owner;

    //
    // Methods
    //

    public abstract void Dispose();

    internal void OnDisposed()
    {
        Disposed?.Invoke(this, EventArgs.Empty);
        _owner.OnDisposed();
    }

    public void ThrowIfDisposed()
    {
        if (IsDisposed)
            throw new InvalidOperationException("Window is disposed");
    }

    //
    // Events
    //

    public event EventHandler<EventArgs>? Disposed;
}
