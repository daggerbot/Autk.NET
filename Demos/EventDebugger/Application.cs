/*
 * Copyright (c) 2021 Martin Mills <daggerbot@gmail.com>
 *
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */

namespace EventDebugger;

using System;

public class Application : Autk.Application
{
    //==============================================================================
    // Fields
    //==============================================================================

    private MainWindow _mainWindow;

    //==============================================================================
    // Constructors
    //==============================================================================

    private Application()
    {
        _mainWindow = new MainWindow();
        _mainWindow.Disposed += (sender, e) => Console.WriteLine($"Disposed");
        _mainWindow.Moved += (sender, e) => Console.WriteLine($"Moved: {e.Location.X}, {e.Location.Y}");
        _mainWindow.Resized += (sender, e) => Console.WriteLine($"Resized: {e.Size.Width}, {e.Size.Height}");
        _mainWindow.VisibilityChanged += (sender, e) => Console.WriteLine($"VisibilityChanged: {e.IsVisible}");
    }

    //==============================================================================
    // Methods
    //==============================================================================

    public static void Main(string[] args)
    {
        using (var app = new Application())
        {
            Run();
        }
    }

    protected override void Start()
    {
        _mainWindow.IsVisible = true;
    }
}
