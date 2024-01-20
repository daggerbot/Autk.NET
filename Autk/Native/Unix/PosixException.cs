// Copyright (c) 2024 Martin Mills <daggerbot@gmail.com>
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace Autk.Native.Unix;

using System;

public class PosixException : Exception
{
    //
    // Fields
    //

    public readonly int ErrorCode;

    //
    // Constructors
    //

    internal PosixException(string message, Exception? innerException = null)
        : base(message, innerException)
    {
    }

    internal PosixException(string messagePrefix, int errorCode, Exception? innerException = null)
        : base($"{messagePrefix}: {PosixUtils.GetErrorString(errorCode)}", innerException)
    {
        this.ErrorCode = errorCode;
    }
}
