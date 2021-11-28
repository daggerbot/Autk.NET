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

    // Connection error codes
    public const int XCB_CONN_ERROR = 1;
    public const int XCB_CONN_CLOSED_EXT_NOTSUPPORTED = 2;
    public const int XCB_CONN_CLOSED_MEM_INSUFFICIENT = 3;
    public const int XCB_CONN_CLOSED_REQ_LEN_EXCEED = 4;
    public const int XCB_CONN_CLOSED_PARSE_ERR = 5;
    public const int XCB_CONN_CLOSED_INVALID_SCREEN = 6;
    public const int XCB_CONN_CLOSED_FDPASSING_FAILED = 7;

    // Event opcodes
    public const byte XCB_DESTROY_NOTIFY = 17;
    public const byte XCB_CLIENT_MESSAGE = 33;

    // Predefined atoms
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

    //==============================================================================
    // Types
    //==============================================================================

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
    public struct xcb_depth_t
    {
        public byte depth;
        public byte pad0;
        public ushort visuals_len;
        public uint pad1;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct xcb_depth_iterator_t
    {
        public IntPtr data;
        public int rem;
        public int index;
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

    public enum xcb_prop_mode_t
    {
        XCB_PROP_MODE_REPLACE,
        XCB_PROP_MODE_PREPEND,
        XCB_PROP_MODE_APPEND,
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
    public struct xcb_setup_t
    {
        public byte status;
        public byte pad0;
        public ushort protocol_major_version;
        public ushort protocol_minor_version;
        public ushort length;
        public uint release_number;
        public uint resource_id_base;
        public uint resource_id_mask;
        public uint motion_buffer_size;
        public ushort vendor_len;
        public ushort maximum_request_length;
        public byte roots_len;
        public byte pixmap_formats_len;
        public byte image_byte_order;
        public byte bitmap_format_bit_order;
        public byte bitmap_format_scanline_unit;
        public byte bitmap_format_scanline_pad;
        public byte min_keycode;
        public byte max_keycode;
        public uint pad1;
    }

    public enum xcb_visual_class_t
    {
        XCB_VISUAL_CLASS_STATIC_GRAY,
        XCB_VISUAL_CLASS_GRAY_SCALE,
        XCB_VISUAL_CLASS_STATIC_COLOR,
        XCB_VISUAL_CLASS_PSEUDO_COLOR,
        XCB_VISUAL_CLASS_TRUE_COLOR,
        XCB_VISUAL_CLASS_DIRECT_COLOR,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct xcb_visualtype_t
    {
        public uint visual_id;
        public byte class_;
        public byte bits_per_rgb_value;
        public ushort colormap_entries;
        public uint red_mask;
        public uint green_mask;
        public uint blue_mask;
        public uint pad0;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct xcb_visualtype_iterator_t
    {
        public IntPtr data;
        public int rem;
        public int index;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct xcb_void_cookie_t
    {
        public uint sequence;
    }

    public enum xcb_window_class_t
    {
        XCB_WINDOW_CLASS_COPY_FROM_PARENT,
        XCB_WINDOW_CLASS_INPUT_OUTPUT,
        XCB_WINDOW_CLASS_INPUT_ONLY,
    }
}
