/*
 * Copyright (c) 2021 Martin Mills <daggerbot@gmail.com>
 *
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */

namespace Autk.Native.IO;

using System.IO;
using System.Runtime.InteropServices;

internal static class PosixPipe
{
    //==============================================================================
    // Methods
    //==============================================================================

    public static void Open(out PosixPipeReader reader, out PosixPipeWriter writer)
    {
        var fds = new int[2];

        if (Posix.pipe(fds) != 0)
            throw new IOException("pipe(): " + Libc.strerror(Marshal.GetLastSystemError()));

        reader = PosixPipeReader.FromFileDescriptor(fds[0]);
        writer = PosixPipeWriter.FromFileDescriptor(fds[1]);
    }
}
