/*
 * Copyright (c) 2021 Martin Mills <daggerbot@gmail.com>
 *
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */

namespace Autk.Native.X11;

using System;
using System.Runtime.InteropServices;

internal static partial class Xcb
{
    //==============================================================================
    // Constants
    //==============================================================================

    private const string LibXcb = "libxcb.so.1";

    //==============================================================================
    // Methods
    //==============================================================================

    [DllImport(LibXcb)]
    public static extern xcb_void_cookie_t xcb_change_property(
        IntPtr connection,
        byte mode,
        uint window,
        uint property,
        uint type,
        byte format,
        uint dataLength,
        [In]
        [MarshalAs(UnmanagedType.LPArray)]
        byte[]? data);

    [DllImport(LibXcb)]
    public static extern xcb_void_cookie_t xcb_change_property(
        IntPtr connection,
        byte mode,
        uint window,
        uint property,
        uint type,
        byte format,
        uint dataLength,
        [In]
        [MarshalAs(UnmanagedType.LPArray)]
        ushort[]? data);

    [DllImport(LibXcb)]
    public static extern xcb_void_cookie_t xcb_change_property(
        IntPtr connection,
        byte mode,
        uint window,
        uint property,
        uint type,
        byte format,
        uint dataLength,
        [In]
        [MarshalAs(UnmanagedType.LPArray)]
        uint[]? data);

    [DllImport(LibXcb)]
    public static extern xcb_void_cookie_t xcb_configure_window(
        IntPtr connection,
        uint window,
        ushort valueMask,
        [In]
        [MarshalAs(UnmanagedType.LPArray)]
        uint[] values);

    [DllImport(LibXcb)]
    public static extern int xcb_connection_has_error(
        IntPtr connection);

    [DllImport(LibXcb)]
    public static extern xcb_void_cookie_t xcb_create_window(
        IntPtr connection,
        byte depth,
        uint id,
        uint parent,
        short x,
        short y,
        ushort width,
        ushort height,
        ushort borderWidth,
        ushort class_,
        uint visual,
        uint valueMask,
        [In]
        [MarshalAs(UnmanagedType.LPArray)]
        uint[]? values);

    [DllImport(LibXcb)]
    public static extern void xcb_depth_next(
        ref xcb_depth_iterator_t iter);

    [DllImport(LibXcb)]
    public static extern xcb_visualtype_iterator_t xcb_depth_visuals_iterator(
        IntPtr depthPtr);

    [DllImport(LibXcb)]
    public static extern xcb_void_cookie_t xcb_destroy_window(
        IntPtr connection,
        uint window);

    [DllImport(LibXcb)]
    public static extern int xcb_flush(
        IntPtr connection);

    [DllImport(LibXcb)]
    public static extern uint xcb_generate_id(
        IntPtr connection);

    [DllImport(LibXcb)]
    public static extern int xcb_get_file_descriptor(
        IntPtr connection);

    [DllImport(LibXcb)]
    public static extern IntPtr xcb_get_setup(
        IntPtr connection);

    [DllImport(LibXcb)]
    public static extern xcb_intern_atom_cookie_t xcb_intern_atom(
        IntPtr connection,
        [MarshalAs(UnmanagedType.U1)]
        bool onlyIfExists,
        ushort nameLength,
        [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)]
        byte[] name);

    [DllImport(LibXcb)]
    public static extern IntPtr xcb_intern_atom_reply(
        IntPtr connection,
        xcb_intern_atom_cookie_t cookie,
        ref IntPtr errorPtr);

    [DllImport(LibXcb)]
    public static extern xcb_void_cookie_t xcb_map_window(
        IntPtr connection,
        uint window);

    [DllImport(LibXcb)]
    public static extern IntPtr xcb_poll_for_event(
        IntPtr connection);

    [DllImport(LibXcb)]
    public static extern xcb_depth_iterator_t xcb_screen_allowed_depths_iterator(
        IntPtr screenPtr);

    [DllImport(LibXcb)]
    public static extern void xcb_screen_next(
        ref xcb_screen_iterator_t iter);

    [DllImport(LibXcb)]
    public static extern xcb_screen_iterator_t xcb_setup_roots_iterator(
        IntPtr setupPtr);

    [DllImport(LibXcb)]
    public static extern xcb_void_cookie_t xcb_unmap_window(
        IntPtr connection,
        uint window);

    [DllImport(LibXcb)]
    public static extern void xcb_visualtype_next(
        ref xcb_visualtype_iterator_t iter);
}
