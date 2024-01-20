// Copyright (c) 2024 Martin Mills <daggerbot@gmail.com>
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace Autk.Display.X11;

using System;
using System.Runtime.InteropServices;
using System.Text;

using Autk.Native.Unix;
using Autk.Native.X11;

internal struct X11Atoms
{
    //
    // Fields
    //

    // These fields are assigned using reflection.
#pragma warning disable CS0649

    public uint WM_DELETE_WINDOW;
    public uint WM_PROTOCOLS;

#pragma warning restore CS0649

    //
    // Methods
    //

    public static X11Atoms InternAtoms(X11DisplayProvider display)
    {
        display.ThrowIfDisposed();

        var xcb = display.XcbConnectionPtr;
        var atoms = (object)new X11Atoms();
        var fields = typeof(X11Atoms).GetFields();
        var cookies = new Xcb.xcb_intern_atom_cookie_t[fields.Length];

        // Send intern request for every atom in one pass.
        for (int i = 0; i < fields.Length; i++)
        {
            var nameBytes = Encoding.ASCII.GetBytes(fields[i].Name);
            cookies[i] = Xcb.xcb_intern_atom(xcb, false, checked((ushort)nameBytes.Length), nameBytes);
        }

        // Get replies for every atom in one pass.
        for (int i = 0; i < fields.Length; i++)
        {
            var replyPtr = Xcb.xcb_intern_atom_reply(xcb, cookies[i], out var errorPtr);

            try
            {
                if (errorPtr != IntPtr.Zero)
                {
                    var error = Marshal.PtrToStructure<Xcb.xcb_generic_error_t>(errorPtr);
                    throw new X11Exception($"X_InternAtom failed: {fields[i].Name}: {error}");
                }
                else if (replyPtr == IntPtr.Zero)
                    throw new X11Exception($"X_InternAtom failed: {fields[i].Name}");

                var reply = Marshal.PtrToStructure<Xcb.xcb_intern_atom_reply_t>(replyPtr);
                fields[i].SetValue(atoms, (object)reply.atom);
            }
            finally
            {
                if (replyPtr != IntPtr.Zero)
                    Posix.free(replyPtr);
                if (errorPtr != IntPtr.Zero)
                    Posix.free(errorPtr);
            }
        }

        return (X11Atoms)atoms;
    }
}
