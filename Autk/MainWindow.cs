/*
 * Copyright (c) 2021 Martin Mills <daggerbot@gmail.com>
 *
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */

namespace Autk;

using System.Drawing;
using System.IO;
using System.Reflection;

public class MainWindow : Window
{
    //==============================================================================
    // Fields
    //==============================================================================

    private static string _defaultTitle = Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly()!.Location)!;

    //==============================================================================
    // Constructors
    //==============================================================================

    public MainWindow()
        : base(WindowType.Normal, new Size(100, 100))
    {
        Title = _defaultTitle;
    }
}
