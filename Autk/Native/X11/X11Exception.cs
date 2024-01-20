// Copyright (c) 2024 Martin Mills <daggerbot@gmail.com>
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace Autk.Native.X11;

using System;

public class X11Exception : Exception
{
    //
    // Constructors
    //

    internal X11Exception(string message, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}
