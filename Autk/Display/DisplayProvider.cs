// Copyright (c) 2024 Martin Mills <daggerbot@gmail.com>
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace Autk.Display;

using System;
using System.Drawing;
using System.Runtime.InteropServices;

internal abstract class DisplayProvider : IDisposable
{
    //
    // Fields
    //

    private static DisplayProvider? _default;

    //
    // Properties
    //

    public abstract bool IsDisposed { get; }

    //
    // Methods
    //

    public abstract WindowProvider CreateWindow(Window owner, WindowStyle style, Size size);

    public abstract void Dispose();

    public static DisplayProvider GetPlatformDefault()
    {
        if (_default != null)
            return _default;

        lock (typeof(DisplayProvider))
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
                _default = new X11.X11DisplayProvider();
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                _default = new X11.X11DisplayProvider();
            else
                throw new PlatformNotSupportedException();
        }

        return _default;
    }

    internal void OnDisposed()
    {
        Disposed?.Invoke(this, EventArgs.Empty);
    }

    public abstract void PostQuitMessage();

    public abstract void Run();

    public void ThrowIfDisposed()
    {
        if (IsDisposed)
            throw new InvalidOperationException("Display provider is disposed");
    }

    //
    // Events
    //

    public event EventHandler<EventArgs>? Disposed;
}
