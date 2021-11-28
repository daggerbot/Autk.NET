/*
 * Copyright (c) 2021 Martin Mills <daggerbot@gmail.com>
 *
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */

namespace WidgetFactory;

using System.Drawing;

using Autk;

public class Application : Autk.Application
{
    //==============================================================================
    // Fields
    //==============================================================================

    private MainWindow _mainWindow;

    //==============================================================================
    // Constructors
    //==============================================================================

    public Application()
    {
        _mainWindow = new MainWindow();
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
