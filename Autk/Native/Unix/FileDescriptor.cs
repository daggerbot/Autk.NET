// Copyright (c) 2024 Martin Mills <daggerbot@gmail.com>
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace Autk.Native.Unix;

using System;
using System.Runtime.InteropServices;

internal class FileDescriptor : SafeHandle, IFileDescriptor
{
    //
    // Constructors
    //

    public FileDescriptor()
        : base(new IntPtr(-1), true)
    {
    }

    public FileDescriptor(int fd)
        : this()
    {
        SetHandle(new IntPtr(fd));
    }

    //
    // Properties
    //

    public int FileDescriptorValue => handle.ToInt32();

    public override bool IsInvalid => FileDescriptorValue < 0;

    //
    // Methods
    //

    protected override bool ReleaseHandle()
    {
        int fd = FileDescriptorValue;
        bool released = true;

        if (fd >= 0)
        {
            if (Posix.close(fd) != 0)
                released = false;

            handle = new IntPtr(-1);
        }

        return released;
    }
}
