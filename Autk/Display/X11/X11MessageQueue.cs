// Copyright (c) 2024 Martin Mills <daggerbot@gmail.com>
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace Autk.Display.X11;

using System;
using System.Collections.Concurrent;
using System.IO;

using Autk.Native.Unix;

internal class X11MessageQueue : IDisposable
{
    //
    // Constants
    //

    private const byte OpcodeNop = 0;
    private const byte OpcodeQuit = 1;
    private const byte OpcodeAction = 2;

    //
    // Fields
    //

    private PosixPipe _pipe = new();
    private ConcurrentQueue<Action> _actionQueue = new();

    //
    // Properties
    //

    public FileDescriptor ReadFileDescriptor => _pipe.ReadFileDescriptor;

    //
    // Methods
    //

    public void Dispose()
    {
        _pipe.Dispose();
    }

    // Returns false if a quit message is received.
    public bool HandleNextMessage()
    {
        switch (_pipe.ReadByte())
        {
            case -1:
            case OpcodeNop:
                return true;

            case OpcodeQuit:
                return false;

            case OpcodeAction:
                if (_actionQueue.TryDequeue(out var action))
                    action.Invoke();
                return true;

            default:
                throw new InvalidDataException("Invalid message opcode");
        }
    }

    public void PostAction(Action action)
    {
        lock (this)
        {
            _actionQueue.Enqueue(action);
            _pipe.WriteByte(OpcodeAction);
        }
    }

    public void PostQuitMessage()
    {
        lock (this)
        {
            _pipe.WriteByte(OpcodeQuit);
        }
    }
}
