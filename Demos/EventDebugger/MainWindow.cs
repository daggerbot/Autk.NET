/*
 * Copyright (c) 2021 Martin Mills <daggerbot@gmail.com>
 *
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */

namespace EventDebugger;

using System;

public class MainWindow : Autk.MainWindow
{
    //==============================================================================
    // Methods
    //==============================================================================

    protected override void CloseRequested()
    {
        Console.WriteLine("CloseRequested");
        base.CloseRequested();
    }
}
