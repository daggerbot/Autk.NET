// Copyright (c) 2024 Martin Mills <daggerbot@gmail.com>
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace EventDebuggerApp;

using System;

public class Application : Autk.Application
{
    //
    // Fields
    //

    private MainWindow _mainWindow;

    //
    // Constructors
    //

    private Application()
    {
        _mainWindow = new MainWindow();
    }

    //
    // Methods
    //

    public static void Main(string[] args)
    {
        using (var app = new Application())
        {
            app.Run();
        }
    }

    protected override void Start()
    {
        _mainWindow.IsVisible = true;
    }
}
