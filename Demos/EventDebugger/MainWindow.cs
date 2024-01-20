// Copyright (c) 2024 Martin Mills <daggerbot@gmail.com>
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace EventDebuggerApp;

using System;

public class MainWindow : Autk.MainWindow
{
    //
    // Constructors
    //

    public MainWindow()
    {
        Disposed += (sender, e) => Console.WriteLine("Window disposed");
        VisibilityChanged += (sender, e) => Console.WriteLine($"Visibility changed: {e.IsVisible}");
    }

    //
    // Methods
    //

    protected override void CloseRequested()
    {
        Console.WriteLine("Close requested");
        base.CloseRequested();
    }
}
