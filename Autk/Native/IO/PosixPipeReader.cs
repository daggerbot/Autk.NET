/*
 * Copyright (c) 2021 Martin Mills <daggerbot@gmail.com>
 *
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */

namespace Autk.Native.IO;

using System;
using System.IO;
using System.Runtime.InteropServices;

internal class PosixPipeReader : Stream
{
    //==============================================================================
    // Fields
    //==============================================================================

    private int _fd = -1;

    //==============================================================================
    // Constructors
    //==============================================================================

    private PosixPipeReader(int fd)
    {
        _fd = fd;
    }

    //==============================================================================
    // Properties
    //==============================================================================

    public override bool CanRead => IsOpen;

    public override bool CanSeek => false;

    public override bool CanWrite => false;

    public int FileDescriptor => _fd;

    public bool IsOpen => _fd >= 0;

    public override long Length => throw new NotSupportedException();

    public override long Position
    {
        get => throw new NotSupportedException();
        set => throw new NotSupportedException();
    }

    //==============================================================================
    // Methods
    //==============================================================================

    protected override void Dispose(bool disposing)
    {
        if (_fd >= 0)
        {
            Posix.close(_fd);
            _fd = -1;
        }
    }

    public override void Flush()
    {
    }

    public static PosixPipeReader FromFileDescriptor(int fd)
    {
        return new PosixPipeReader(fd);
    }

    // NOTE: Could use optimization.
    public override int Read(byte[] buffer, int offset, int count)
    {
        if (count == 0)
            return 0;

        ThrowIfClosed();
        var readBuffer = buffer;

        if (offset != 0)
            readBuffer = new byte[count];

        int result = Posix.read(_fd, readBuffer, new UIntPtr(checked((uint)count))).ToInt32();

        if (result < 0)
            throw new IOException("read(): " + Libc.strerror(Marshal.GetLastSystemError()));

        if (readBuffer != buffer)
            Array.Copy(readBuffer, 0, buffer, offset, result);

        return result;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotSupportedException();
    }

    public override void SetLength(long value)
    {
        throw new NotSupportedException();
    }

    internal void ThrowIfClosed()
    {
        if (!IsOpen)
            throw new ObjectDisposedException("File descriptor is closed");
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new NotSupportedException();
    }
}
