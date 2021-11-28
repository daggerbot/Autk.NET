/*
 * Copyright (c) 2021 Martin Mills <daggerbot@gmail.com>
 *
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */

namespace Autk.Native.X11;

using System;

public class X11Exception : NativeRuntimeException
{
    //==============================================================================
    // Constructors
    //==============================================================================

    internal X11Exception(string message, Exception? innerException = null)
        : base(message, innerException)
    {
    }

    internal X11Exception(Xcb.xcb_generic_error_t error)
        : this($"xcb_generic_error_t(response_type: {error.response_type}, error_code: {error.error_code}, "
               + $"sequence: {error.sequence}, resource_id: {error.resource_id}, minor_code: {error.minor_code}, "
               + $"major_code: {error.major_code}, full_sequence: {error.full_sequence}")
    {
    }
}
