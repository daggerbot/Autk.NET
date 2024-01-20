// Copyright (c) 2024 Martin Mills <daggerbot@gmail.com>
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace Autk.Native.Unix;

using System;
using System.IO;
using System.Runtime.InteropServices;

internal class PosixPipe : Stream
{
    //
    // Fields
    //

    private FileDescriptor _readFd;
    private FileDescriptor _writeFd;

    //
    // Constructors
    //

    public PosixPipe()
    {
        var fds = new int[2];

        if (Posix.pipe(fds) != 0)
            throw new PosixException("pipe", Marshal.GetLastSystemError());

        _readFd = new FileDescriptor(fds[0]);
        _writeFd = new FileDescriptor(fds[1]);
    }

    //
    // Properties
    //

    public override bool CanRead => true;

    public override bool CanSeek => false;

    public override bool CanWrite => true;

    public override long Length => throw new NotSupportedException();

    public override long Position
    {
        get => throw new NotSupportedException();
        set => throw new NotSupportedException();
    }

    public FileDescriptor ReadFileDescriptor => _readFd;

    public FileDescriptor WriteFileDescriptor => _writeFd;

    //
    // Methods
    //

    protected override void Dispose(bool disposing)
    {
        _readFd.Dispose();
        _writeFd.Dispose();

        base.Dispose(disposing);
    }

    private int DoWrite(byte[] buffer, int offset, int count)
    {
        var pinnedBuffer = GCHandle.Alloc(buffer, GCHandleType.Pinned);
        IntPtr result;

        try
        {
            var bufferPtr = pinnedBuffer.AddrOfPinnedObject() + offset;
            result = Posix.write(_writeFd.FileDescriptorValue, bufferPtr, new UIntPtr(unchecked((uint)count)));
        }
        finally
        {
            pinnedBuffer.Free();
        }

        long longResult = result.ToInt64();

        if (longResult < 0)
            throw new PosixException("write", Marshal.GetLastSystemError());

        return checked((int)longResult);
    }

    public override void Flush()
    {
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        if (count == 0)
            return 0;
        else if (offset < 0 || offset > buffer.Length)
            throw new ArgumentOutOfRangeException(nameof(offset));
        else if (count < 0 || count > buffer.Length - offset)
            throw new ArgumentOutOfRangeException(nameof(count));

        var pinnedBuffer = GCHandle.Alloc(buffer, GCHandleType.Pinned);
        IntPtr result;

        try
        {
            var bufferPtr = pinnedBuffer.AddrOfPinnedObject() + offset;
            result = Posix.read(_readFd.FileDescriptorValue, bufferPtr, new UIntPtr(unchecked((uint)count)));
        }
        finally
        {
            pinnedBuffer.Free();
        }

        long longResult = result.ToInt64();

        if (longResult < 0)
            throw new PosixException("read", Marshal.GetLastSystemError());

        return checked((int)longResult);
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotSupportedException();
    }

    public override void SetLength(long value)
    {
        throw new NotSupportedException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        if (offset < 0 || offset > buffer.Length)
            throw new ArgumentOutOfRangeException(nameof(offset));
        else if (count < 0 || count > buffer.Length - offset)
            throw new ArgumentOutOfRangeException(nameof(count));

        while (count > 0)
        {
            int result = DoWrite(buffer, offset, count);

            if (result == 0)
                throw new EndOfStreamException();

            offset += result;
            count -= result;
        }
    }
}
