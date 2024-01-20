// Copyright (c) 2024 Martin Mills <daggerbot@gmail.com>
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace Autk;

using System;
using System.Drawing;

public class MainWindow : Window
{
    //
    // Fields
    //

    private static readonly Size DefaultSize = new Size(640, 480);

    //
    // Constructors
    //

    public MainWindow()
        : base(WindowStyle.Normal, DefaultSize)
    {
    }
}
