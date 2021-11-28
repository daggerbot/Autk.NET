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

internal class WindowEventArgs : EventArgs
{
    //==============================================================================
    // Properties
    //==============================================================================

    public WindowEventType EventType { get; init; }

    public Point? Location { get; init; }

    public Size? Size { get; init; }
}
