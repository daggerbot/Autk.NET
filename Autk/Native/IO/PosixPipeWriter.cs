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

internal class PosixPipeWriter : Stream
{
    //==============================================================================
    // Fields
    //==============================================================================

    private int _fd = -1;

    //==============================================================================
    // Constructors
    //==============================================================================

    private PosixPipeWriter(int fd)
    {
        _fd = fd;
    }

    //==============================================================================
    // Properties
    //==============================================================================

    public override bool CanRead => false;

    public override bool CanSeek => false;

    public override bool CanWrite => IsOpen;

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

    public static PosixPipeWriter FromFileDescriptor(int fd)
    {
        return new PosixPipeWriter(fd);
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        throw new NotSupportedException();
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

    // NOTE: Could use optimization.
    public override void Write(byte[] buffer, int offset, int count)
    {
        if (count == 0)
            return;

        ThrowIfClosed();

        if (offset != 0)
            buffer = new ReadOnlySpan<byte>(buffer, offset, count).ToArray();

        int result = Posix.write(_fd, buffer, new UIntPtr(checked((uint)count))).ToInt32();

        if (result < 0)
            throw new IOException("write(): " + Libc.strerror(Marshal.GetLastSystemError()));
        else if (result == 0)
            throw new EndOfStreamException();
        else if (result < count)
            Write(buffer, result, count - result);
    }
}
