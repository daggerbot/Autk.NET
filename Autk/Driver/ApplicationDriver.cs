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
using System.Runtime.InteropServices;

internal abstract class ApplicationDriver : IDisposable
{
    //==============================================================================
    // Fields
    //==============================================================================

    private static ApplicationDriver? _defaultInstance;

    //==============================================================================
    // Properties
    //==============================================================================

    public abstract bool IsDisposed { get; }

    //==============================================================================
    // Methods
    //==============================================================================

    public abstract WindowDriver CreateWindow(WindowType type, Size size);

    public abstract void Dispose();

    public static ApplicationDriver GetDefaultInstance()
    {
        if (_defaultInstance == null)
        {
            lock (typeof(ApplicationDriver))
            {
                if (_defaultInstance != null)
                    return _defaultInstance;

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                    || RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
                {
                    _defaultInstance = new X11.X11ApplicationDriver();
                }
                else
                    throw new PlatformNotSupportedException();
            }
        }

        return _defaultInstance!;
    }

    internal void OnDisposed()
    {
        if (Disposed != null)
            Disposed.Invoke(this, new EventArgs());
    }

    public abstract void Quit();

    public abstract void Run();

    internal void ThrowIfDisposed()
    {
        if (IsDisposed)
            throw new ObjectDisposedException("Application is disposed");
    }

    //==============================================================================
    // Events
    //==============================================================================

    public event EventHandler<EventArgs>? Disposed;
}
