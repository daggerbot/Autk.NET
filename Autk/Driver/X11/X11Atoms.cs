/*
 * Copyright (c) 2021 Martin Mills <daggerbot@gmail.com>
 *
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */

namespace Autk.Driver.X11;

internal struct X11Atoms
{
    //==============================================================================
    // Fields
    //==============================================================================

#pragma warning disable 0649

    public uint _NET_WM_ICON_NAME;
    public uint _NET_WM_NAME;
    public uint _NET_WM_WINDOW_TYPE;
    public uint _NET_WM_WINDOW_TYPE_NORMAL;
    public uint UTF8_STRING;
    public uint WM_DELETE_WINDOW;
    public uint WM_PROTOCOLS;

#pragma warning restore 0649
}
