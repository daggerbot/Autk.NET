/*
 * Copyright (c) 2021 Martin Mills <daggerbot@gmail.com>
 *
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */

namespace Autk;

using System;

using Autk.Driver;

public class Application : IDisposable
{
    //==============================================================================
    // Fields
    //==============================================================================

    private static Application? _instance;

    //==============================================================================
    // Constructors
    //==============================================================================

    public Application()
    {
        lock (typeof(Application))
        {
            if (_instance != null)
                throw new InvalidOperationException("Multiple application instances created");

            _instance = this;
        }

        //DELETEME
        ApplicationDriver.GetDefaultInstance();
    }

    //==============================================================================
    // Properties
    //==============================================================================

    public static Application? Instance => _instance;

    //==============================================================================
    // Methods
    //==============================================================================

    public virtual void Dispose()
    {
    }

    internal protected void LastWindowClosed()
    {
        Quit();
    }

    public static void Quit()
    {
        ApplicationDriver.GetDefaultInstance().Quit();
    }

    public static void Run()
    {
        if (_instance != null)
            _instance.Start();

        ApplicationDriver.GetDefaultInstance().Run();

        if (_instance != null)
            _instance.Stop();
    }

    protected virtual void Start()
    {
    }

    protected virtual void Stop()
    {
    }
}
