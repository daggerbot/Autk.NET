/*
 * Copyright (c) 2021 Martin Mills <daggerbot@gmail.com>
 *
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */

namespace EventDebugger;

using System;
using System.Drawing;

public class MainWindow : Autk.MainWindow
{
    //==============================================================================
    // Constructors
    //==============================================================================

    public MainWindow()
    {
        Size = new Size(640, 480);

        Disposed += (sender, e) => Console.WriteLine($"Disposed");
        Moved += (sender, e) => Console.WriteLine($"Moved: {e.Location.X}, {e.Location.Y}");
        Resized += (sender, e) => Console.WriteLine($"Resized: {e.Size.Width}, {e.Size.Height}");
        VisibilityChanged += (sender, e) => Console.WriteLine($"VisibilityChanged: {e.IsVisible}");
    }

    //==============================================================================
    // Methods
    //==============================================================================

    protected override void CloseRequested()
    {
        Console.WriteLine("CloseRequested");
        base.CloseRequested();
    }
}
