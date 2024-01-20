// Copyright (c) 2024 Martin Mills <daggerbot@gmail.com>
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace Autk.Native.X11;

using System;
using System.Runtime.InteropServices;

internal static class Xcb
{
    //
    // Constants
    //

    private const string LibXcb = "libxcb.so.1";

    // Atoms
    public const uint XCB_ATOM_NONE = 0;
    public const uint XCB_ATOM_ANY = 0;
    public const uint XCB_ATOM_PRIMARY = 1;
    public const uint XCB_ATOM_SECONDARY = 2;
    public const uint XCB_ATOM_ARC = 3;
    public const uint XCB_ATOM_ATOM = 4;
    public const uint XCB_ATOM_BITMAP = 5;
    public const uint XCB_ATOM_CARDINAL = 6;
    public const uint XCB_ATOM_COLORMAP = 7;
    public const uint XCB_ATOM_CURSOR = 8;
    public const uint XCB_ATOM_CUT_BUFFER0 = 9;
    public const uint XCB_ATOM_CUT_BUFFER1 = 10;
    public const uint XCB_ATOM_CUT_BUFFER2 = 11;
    public const uint XCB_ATOM_CUT_BUFFER3 = 12;
    public const uint XCB_ATOM_CUT_BUFFER4 = 13;
    public const uint XCB_ATOM_CUT_BUFFER5 = 14;
    public const uint XCB_ATOM_CUT_BUFFER6 = 15;
    public const uint XCB_ATOM_CUT_BUFFER7 = 16;
    public const uint XCB_ATOM_DRAWABLE = 17;
    public const uint XCB_ATOM_FONT = 18;
    public const uint XCB_ATOM_INTEGER = 19;
    public const uint XCB_ATOM_PIXMAP = 20;
    public const uint XCB_ATOM_POINT = 21;
    public const uint XCB_ATOM_RECTANGLE = 22;
    public const uint XCB_ATOM_RESOURCE_MANAGER = 23;
    public const uint XCB_ATOM_RGB_COLOR_MAP = 24;
    public const uint XCB_ATOM_RGB_BEST_MAP = 25;
    public const uint XCB_ATOM_RGB_BLUE_MAP = 26;
    public const uint XCB_ATOM_RGB_DEFAULT_MAP = 27;
    public const uint XCB_ATOM_RGB_GRAY_MAP = 28;
    public const uint XCB_ATOM_RGB_GREEN_MAP = 29;
    public const uint XCB_ATOM_RGB_RED_MAP = 30;
    public const uint XCB_ATOM_STRING = 31;
    public const uint XCB_ATOM_VISUALID = 32;
    public const uint XCB_ATOM_WINDOW = 33;
    public const uint XCB_ATOM_WM_COMMAND = 34;
    public const uint XCB_ATOM_WM_HINTS = 35;
    public const uint XCB_ATOM_WM_CLIENT_MACHINE = 36;
    public const uint XCB_ATOM_WM_ICON_NAME = 37;
    public const uint XCB_ATOM_WM_ICON_SIZE = 38;
    public const uint XCB_ATOM_WM_NAME = 39;
    public const uint XCB_ATOM_WM_NORMAL_HINTS = 40;
    public const uint XCB_ATOM_WM_SIZE_HINTS = 41;
    public const uint XCB_ATOM_WM_ZOOM_HINTS = 42;
    public const uint XCB_ATOM_MIN_SPACE = 43;
    public const uint XCB_ATOM_NORM_SPACE = 44;
    public const uint XCB_ATOM_MAX_SPACE = 45;
    public const uint XCB_ATOM_END_SPACE = 46;
    public const uint XCB_ATOM_SUPERSCRIPT_X = 47;
    public const uint XCB_ATOM_SUPERSCRIPT_Y = 48;
    public const uint XCB_ATOM_SUBSCRIPT_X = 49;
    public const uint XCB_ATOM_SUBSCRIPT_Y = 50;
    public const uint XCB_ATOM_UNDERLINE_POSITION = 51;
    public const uint XCB_ATOM_UNDERLINE_THICKNESS = 52;
    public const uint XCB_ATOM_STRIKEOUT_ASCENT = 53;
    public const uint XCB_ATOM_STRIKEOUT_DESCENT = 54;
    public const uint XCB_ATOM_ITALIC_ANGLE = 55;
    public const uint XCB_ATOM_X_HEIGHT = 56;
    public const uint XCB_ATOM_QUAD_WIDTH = 57;
    public const uint XCB_ATOM_WEIGHT = 58;
    public const uint XCB_ATOM_POINT_SIZE = 59;
    public const uint XCB_ATOM_RESOLUTION = 60;
    public const uint XCB_ATOM_COPYRIGHT = 61;
    public const uint XCB_ATOM_NOTICE = 62;
    public const uint XCB_ATOM_FONT_NAME = 63;
    public const uint XCB_ATOM_FAMILY_NAME = 64;
    public const uint XCB_ATOM_FULL_NAME = 65;
    public const uint XCB_ATOM_CAP_HEIGHT = 66;
    public const uint XCB_ATOM_WM_CLASS = 67;
    public const uint XCB_ATOM_WM_TRANSIENT_FOR = 68;

    // Event masks
    public const int XCB_EVENT_MASK_NO_EVENT = 0;
    public const int XCB_EVENT_MASK_KEY_PRESS = 0x00000001;
    public const int XCB_EVENT_MASK_KEY_RELEASE = 0x00000002;
    public const int XCB_EVENT_MASK_BUTTON_PRESS = 0x00000004;
    public const int XCB_EVENT_MASK_BUTTON_RELEASE = 0x00000008;
    public const int XCB_EVENT_MASK_ENTER_WINDOW = 0x00000010;
    public const int XCB_EVENT_MASK_LEAVE_WINDOW = 0x00000020;
    public const int XCB_EVENT_MASK_POINTER_MOTION = 0x00000040;
    public const int XCB_EVENT_MASK_POINTER_MOTION_HINT = 0x00000080;
    public const int XCB_EVENT_MASK_BUTTON_1_MOTION = 0x00000100;
    public const int XCB_EVENT_MASK_BUTTON_2_MOTION = 0x00000200;
    public const int XCB_EVENT_MASK_BUTTON_3_MOTION = 0x00000400;
    public const int XCB_EVENT_MASK_BUTTON_4_MOTION = 0x00000800;
    public const int XCB_EVENT_MASK_BUTTON_5_MOTION = 0x00001000;
    public const int XCB_EVENT_MASK_BUTTON_MOTION = 0x00002000;
    public const int XCB_EVENT_MASK_KEYMAP_STATE = 0x00004000;
    public const int XCB_EVENT_MASK_EXPOSURE = 0x00008000;
    public const int XCB_EVENT_MASK_VISIBILITY_CHANGE = 0x00010000;
    public const int XCB_EVENT_MASK_STRUCTURE_NOTIFY = 0x00020000;
    public const int XCB_EVENT_MASK_RESIZE_REDIRECT = 0x00040000;
    public const int XCB_EVENT_MASK_SUBSTRUCTURE_NOTIFY = 0x00080000;
    public const int XCB_EVENT_MASK_SUBSTRUCTURE_REDIRECT = 0x00100000;
    public const int XCB_EVENT_MASK_FOCUS_CHANGE = 0x00200000;
    public const int XCB_EVENT_MASK_PROPERTY_CHANGE = 0x00400000;
    public const int XCB_EVENT_MASK_COLOR_MAP_CHANGE = 0x00800000;
    public const int XCB_EVENT_MASK_OWNER_GRAB_BUTTON = 0x01000000;

    // Event opcodes
    public const byte XCB_KEY_PRESS = 2;
    public const byte XCB_KEY_RELEASE = 3;
    public const byte XCB_BUTTON_PRESS = 4;
    public const byte XCB_BUTTON_RELEASE = 5;
    public const byte XCB_MOTION_NOTIFY = 6;
    public const byte XCB_ENTER_NOTIFY = 7;
    public const byte XCB_LEAVE_NOTIFY = 8;
    public const byte XCB_FOCUS_IN = 9;
    public const byte XCB_FOCUS_OUT = 10;
    public const byte XCB_KEYMAP_NOTIFY = 11;
    public const byte XCB_EXPOSE = 12;
    public const byte XCB_GRAPHICS_EXPOSURE = 13;
    public const byte XCB_NO_EXPOSURE = 14;
    public const byte XCB_VISIBILITY_NOTIFY = 15;
    public const byte XCB_CREATE_NOTIFY = 16;
    public const byte XCB_DESTROY_NOTIFY = 17;
    public const byte XCB_UNMAP_NOTIFY = 18;
    public const byte XCB_MAP_NOTIFY = 19;
    public const byte XCB_MAP_REQUEST = 20;
    public const byte XCB_REPARENT_NOTIFY = 21;
    public const byte XCB_CONFIGURE_NOTIFY = 22;
    public const byte XCB_CONFIGURE_REQUEST = 23;
    public const byte XCB_GRAVITY_NOTIFY = 24;
    public const byte XCB_RESIZE_REQUEST = 25;
    public const byte XCB_CIRCULATE_NOTIFY = 26;
    public const byte XCB_CIRCULATE_REQUEST = 27;
    public const byte XCB_PROPERTY_NOTIFY = 28;
    public const byte XCB_SELECTION_CLEAR = 29;
    public const byte XCB_SELECTION_REQUEST = 30;
    public const byte XCB_SELECTION_NOTIFY = 31;
    public const byte XCB_COLORMAP_NOTIFY = 32;
    public const byte XCB_CLIENT_MESSAGE = 33;
    public const byte XCB_MAPPING_NOTIFY = 34;
    public const byte XCB_GE_GENERIC = 35;

    // xcb_connection_has_error() return values
    public const int XCB_CONN_ERROR = 1;
    public const int XCB_CONN_CLOSED_EXT_NOTSUPPORTED = 2;
    public const int XCB_CONN_CLOSED_MEM_INSUFFICIENT = 3;
    public const int XCB_CONN_CLOSED_REQ_LEN_EXCEED = 4;
    public const int XCB_CONN_CLOSED_PARSE_ERR = 5;
    public const int XCB_CONN_CLOSED_INVALID_SCREEN = 6;
    public const int XCB_CONN_CLOSED_FDPASSING_FAILED = 7;

    // xcb_create_window() value flags
    public const uint XCB_CW_BACK_PIXMAP = 0x0001;
    public const uint XCB_CW_BACK_PIXEL = 0x0002;
    public const uint XCB_CW_BORDER_PIXMAP = 0x0004;
    public const uint XCB_CW_BORDER_PIXEL = 0x0008;
    public const uint XCB_CW_BIT_GRAVITY = 0x0010;
    public const uint XCB_CW_WIN_GRAVITY = 0x0020;
    public const uint XCB_CW_BACKING_STORE = 0x0040;
    public const uint XCB_CW_BACKING_PLANES = 0x0080;
    public const uint XCB_CW_BACKING_PIXEL = 0x0100;
    public const uint XCB_CW_OVERRIDE_REDIRECT = 0x0200;
    public const uint XCB_CW_SAVE_UNDER = 0x0400;
    public const uint XCB_CW_EVENT_MASK = 0x0800;
    public const uint XCB_CW_DONT_PROPAGATE = 0x1000;
    public const uint XCB_CW_COLORMAP = 0x2000;
    public const uint XCB_CW_CURSOR = 0x4000;

    //
    // xcb methods
    //

    [DllImport(LibXcb)]
    public static extern xcb_void_cookie_t xcb_change_property(
        IntPtr connectionPtr,
        byte mode,
        uint window,
        uint property,
        uint type,
        byte format,
        uint dataLength,
        [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 6), In]
        byte[] data);

    [DllImport(LibXcb)]
    public static extern xcb_void_cookie_t xcb_change_property(
        IntPtr connectionPtr,
        byte mode,
        uint window,
        uint property,
        uint type,
        byte format,
        uint dataLength,
        [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 6), In]
        sbyte[] data);

    [DllImport(LibXcb)]
    public static extern xcb_void_cookie_t xcb_change_property(
        IntPtr connectionPtr,
        byte mode,
        uint window,
        uint property,
        uint type,
        byte format,
        uint dataLength,
        [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 6), In]
        short[] data);

    [DllImport(LibXcb)]
    public static extern xcb_void_cookie_t xcb_change_property(
        IntPtr connectionPtr,
        byte mode,
        uint window,
        uint property,
        uint type,
        byte format,
        uint dataLength,
        [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 6), In]
        ushort[] data);

    [DllImport(LibXcb)]
    public static extern xcb_void_cookie_t xcb_change_property(
        IntPtr connectionPtr,
        byte mode,
        uint window,
        uint property,
        uint type,
        byte format,
        uint dataLength,
        [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 6), In]
        int[] data);

    [DllImport(LibXcb)]
    public static extern xcb_void_cookie_t xcb_change_property(
        IntPtr connectionPtr,
        byte mode,
        uint window,
        uint property,
        uint type,
        byte format,
        uint dataLength,
        [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 6), In]
        uint[] data);

    [DllImport(LibXcb)]
    public static extern int xcb_connection_has_error(
        IntPtr connectionPtr);

    [DllImport(LibXcb)]
    public static extern xcb_void_cookie_t xcb_create_window(
        IntPtr connectionPtr,
        byte depth,
        uint window,
        uint parentId,
        short x,
        short y,
        ushort width,
        ushort height,
        ushort borderWidth,
        ushort class_,
        uint visualId,
        uint valueMask,
        [MarshalAs(UnmanagedType.LPArray)]
        int[]? valueList);

    [DllImport(LibXcb)]
    public static extern xcb_void_cookie_t xcb_destroy_window(
        IntPtr connectionPtr,
        uint window);

    [DllImport(LibXcb)]
    public static extern int xcb_flush(
        IntPtr connectionPtr);

    [DllImport(LibXcb)]
    public static extern uint xcb_generate_id(
        IntPtr connectionPtr);

    [DllImport(LibXcb)]
    public static extern int xcb_get_file_descriptor(
        IntPtr connectionPtr);

    [DllImport(LibXcb)]
    public static extern IntPtr xcb_get_setup(
        IntPtr connectionPtr);

    [DllImport(LibXcb)]
    public static extern xcb_intern_atom_cookie_t xcb_intern_atom(
        IntPtr connectionPtr,
        [MarshalAs(UnmanagedType.U1)]
        bool onlyIfExists,
        ushort nameLength,
        [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)]
        byte[] name);

    [DllImport(LibXcb)]
    public static extern IntPtr xcb_intern_atom_reply(
        IntPtr connectionPtr,
        xcb_intern_atom_cookie_t cookie,
        out IntPtr errorPtr);

    [DllImport(LibXcb)]
    public static extern xcb_void_cookie_t xcb_map_window(
        IntPtr connectionPtr,
        uint window);

    [DllImport(LibXcb)]
    public static extern IntPtr xcb_poll_for_event(
        IntPtr connectionPtr);

    [DllImport(LibXcb)]
    public static extern void xcb_screen_next(
        ref xcb_screen_iterator_t iter);

    [DllImport(LibXcb)]
    public static extern xcb_screen_iterator_t xcb_setup_roots_iterator(
        IntPtr setupPtr);

    [DllImport(LibXcb)]
    public static extern xcb_void_cookie_t xcb_unmap_window(
        IntPtr connectionPtr,
        uint window);

    //
    // Types
    //

    [StructLayout(LayoutKind.Sequential)]
    public struct xcb_client_message_data_t
    {
        public uint data0;
        public uint data1;
        public uint data2;
        public uint data3;
        public uint data4;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct xcb_client_message_event_t
    {
        public byte response_type;
        public byte format;
        public ushort sequence;
        public uint window;
        public uint type;
        public xcb_client_message_data_t data;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct xcb_destroy_notify_event_t
    {
        public byte response_type;
        public byte pad0;
        public ushort sequence;
        public uint event_;
        public uint window;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct xcb_generic_error_t
    {
        public byte response_type;
        public byte error_code;
        public ushort sequence;
        public uint resource_id;
        public ushort minor_code;
        public byte major_code;
        public byte pad0;
        public uint pad1;
        public uint pad2;
        public uint pad3;
        public uint pad4;
        public uint pad5;
        public uint full_sequence;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct xcb_generic_event_t
    {
        public byte response_type;
        public byte pad0;
        public ushort sequence;
        public uint pad1;
        public uint pad2;
        public uint pad3;
        public uint pad4;
        public uint pad5;
        public uint pad6;
        public uint pad7;
        public uint full_sequence;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct xcb_intern_atom_cookie_t
    {
        public uint sequence;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct xcb_intern_atom_reply_t
    {
        public byte response_type;
        public byte pad0;
        public ushort sequence;
        public uint length;
        public uint atom;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct xcb_map_notify_event_t
    {
        public byte response_type;
        public byte pad0;
        public ushort sequence;
        public uint event_;
        public uint window;
        public byte override_redirect;
        public byte pad1;
        public byte pad2;
        public byte pad3;
    }

    public enum xcb_prop_mode_t
    {
        XCB_PROP_MODE_REPLACE = 0,
        XCB_PROP_MODE_PREPEND = 1,
        XCB_PROP_MODE_APPEND = 2,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct xcb_screen_t
    {
        public uint root;
        public uint default_colormap;
        public uint white_pixel;
        public uint black_pixel;
        public uint current_input_masks;
        public ushort width_in_pixels;
        public ushort height_in_pixels;
        public ushort width_in_millimeters;
        public ushort height_in_millimeters;
        public ushort min_installed_maps;
        public ushort max_installed_maps;
        public uint root_visual;
        public byte backing_stores;
        public byte save_unders;
        public byte root_depth;
        public byte allowed_depths_len;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct xcb_screen_iterator_t
    {
        public IntPtr data;
        public int rem;
        public int index;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct xcb_unmap_notify_event_t
    {
        public byte response_type;
        public byte pad0;
        public ushort sequence;
        public uint event_;
        public uint window;
        public byte from_configure;
        public byte pad1;
        public byte pad2;
        public byte pad3;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct xcb_void_cookie_t
    {
        public uint sequence;
    }

    public enum xcb_window_class_t
    {
        XCB_WINDOW_CLASS_COPY_FROM_PARENT = 0,
        XCB_WINDOW_CLASS_INPUT_OUTPUT = 1,
        XCB_WINDOW_CLASS_INPUT_ONLY = 2,
    }
}
