using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using DnfServerSwitcher.Models.KrazyIni;
using DnfServerSwitcher.Models.KrazyIni.Data;
using DnfServerSwitcher.Models.Trace;
namespace DnfServerSwitcher.Models {
    public class DnfHotkeyHelper {

        public const byte VK_SHIFT = 0x10;
        public const byte VK_CONTROL = 0x11;
        public const byte VK_MENU = 0x12;
        
        // this transaltes VirtualKey code values to DNF user.ini binding names
        public static Dictionary<int, string> DnfVirtualKeyToString { get; }
        // this translats VirtualKey codes to a virtual key enum string
        // see: https://learn.microsoft.com/en-us/windows/win32/inputdev/virtual-key-codes
        public static Dictionary<int, string> VirtualKeyToString { get; }
        public static Dictionary<string, int> DnfStringToVirtualKey { get; }

        static DnfHotkeyHelper() {
            // NOTE: 0x00 and 0xFF have special meanings

            VirtualKeyToString = new Dictionary<int, string>() {
                [0x01] = "LeftMouse",           // VK_LBUTTON
                [0x02] = "VK_RBUTTON",
                [0x03] = "VK_CANCEL",
                [0x04] = "VK_MBUTTON",
                [0x05] = "VK_XBUTTON1", 
                [0x06] = "VK_XBUTTON2",
                [0x07] = "reserved",
                [0x08] = "VK_BACK",
                [0x09] = "VK_TAB",
                [0x0A] = "reserved",
                [0x0B] = "reserved",
                [0x0C] = "VK_CLEAR",
                [0x0D] = "VK_RETURN",
                [0x0E] = "reserved",
                [0x0F] = "reserved",
                [0x10] = "VK_SHIFT",
                [0x11] = "VK_CONTROL",
                [0x12] = "VK_MENU",
                [0x13] = "VK_PAUSE",
                [0x14] = "VK_CAPITAL",
                [0x15] = "(IME)VK_KANA / VK_HANGUL",
                [0x16] = "(IME)VK_IME_ON",
                [0x17] = "(IME)VK_JUNJA",
                [0x18] = "(IME)VK_FINAL",
                [0x19] = "(IME)VK_HANJA / VK_KANJI",
                [0x1A] = "(IME)VK_IME_OFF",
                [0x1B] = "VK_ESCAPE",
                [0x1C] = "(IME)VK_CONVERT",
                [0x1D] = "(IME)VK_NONCONVERT",
                [0x1E] = "(IME)VK_ACCEPT",
                [0x1F] = "(IME)VK_MODECHANGE",
                [0x20] = "VK_SPACE",
                [0x21] = "VK_PRIOR",
                [0x22] = "VK_NEXT",
                [0x23] = "VK_END",
                [0x24] = "VK_HOME",
                [0x25] = "VK_LEFT",
                [0x26] = "VK_UP",
                [0x27] = "VK_RIGHT",
                [0x28] = "VK_DOWN",
                [0x29] = "VK_SELECT",
                [0x2A] = "VK_PRINT",
                [0x2B] = "VK_EXECUTE",
                [0x2C] = "VK_SNAPSHOT",
                [0x2D] = "VK_INSERT",
                [0x2E] = "VK_DELETE",
                [0x2F] = "VK_HELP",
                [0x30] = "0", 
                [0x31] = "1", 
                [0x32] = "2", 
                [0x33] = "3", 
                [0x34] = "4", 
                [0x35] = "5", 
                [0x36] = "6", 
                [0x37] = "7", 
                [0x38] = "8", 
                [0x39] = "9", 
                [0x3A] = "Undefined", 
                [0x3B] = "Undefined",
                [0x3C] = "Undefined",
                [0x3D] = "Undefined",
                [0x3E] = "Undefined",
                [0x3F] = "Undefined",
                [0x40] = "Undefined",
                [0x41] = "A",
                [0x42] = "B",
                [0x43] = "C",
                [0x44] = "D",
                [0x45] = "E",
                [0x46] = "F",
                [0x47] = "G",
                [0x48] = "H",
                [0x49] = "I",
                [0x4A] = "J",
                [0x4B] = "K",
                [0x4C] = "L",
                [0x4D] = "M",
                [0x4E] = "N",
                [0x4F] = "O",
                [0x50] = "P",
                [0x51] = "Q",
                [0x52] = "R",
                [0x53] = "S",
                [0x54] = "T",
                [0x55] = "U",
                [0x56] = "V",
                [0x57] = "W",
                [0x58] = "X",
                [0x59] = "Y",
                [0x5A] = "Z",
                [0x5B] = "VK_LWIN",
                [0x5C] = "VK_RWIN",
                [0x5D] = "VK_APPS",
                [0x5E] = "reserved",
                [0x5F] = "VK_SLEEP",
                [0x60] = "NumPad0",             //
                [0x61] = "NumPad1",             //
                [0x62] = "NumPad2",             //
                [0x63] = "NumPad3",             //
                [0x64] = "NumPad4",             //
                [0x65] = "NumPad5",             //
                [0x66] = "NumPad6",             //
                [0x67] = "NumPad7",             //
                [0x68] = "NumPad8",             //
                [0x69] = "NumPad9",             //
                [0x6A] = "VK_MULTIPLY",
                [0x6B] = "VK_ADD",
                [0x6C] = "VK_SEPARATOR",
                [0x6D] = "VK_SUBTRACT",
                [0x6E] = "VK_DECIMAL",
                [0x6F] = "VK_DIVIDE",
                [0x70] = "VK_F1",
                [0x71] = "VK_F2",
                [0x72] = "VK_F3",
                [0x73] = "VK_F4",
                [0x74] = "VK_F5",
                [0x75] = "VK_F6",
                [0x76] = "VK_F7",
                [0x77] = "VK_F8",
                [0x78] = "VK_F9",
                [0x79] = "VK_F10",
                [0x7A] = "VK_F11",
                [0x7B] = "VK_F12",
                [0x7C] = "VK_F13",
                [0x7D] = "VK_F14",
                [0x7E] = "VK_F15",
                [0x7F] = "VK_F16",
                [0x80] = "VK_F17",
                [0x81] = "VK_F18",
                [0x82] = "VK_F19",
                [0x83] = "VK_F20",
                [0x84] = "VK_F21",
                [0x85] = "VK_F22",
                [0x86] = "VK_F23",
                [0x87] = "VK_F24",
                [0x88] = "reserved",
                [0x89] = "reserved",
                [0x8A] = "reserved",
                [0x8B] = "reserved",
                [0x8C] = "reserved",
                [0x8D] = "reserved",
                [0x8E] = "reserved",
                [0x8F] = "reserved",
                [0x90] = "VK_NUMLOCK",
                [0x91] = "VK_SCROLL",
                [0x92] = "OEM specific",
                [0x93] = "OEM specific",
                [0x94] = "OEM specific",
                [0x95] = "OEM specific",
                [0x96] = "OEM specific",
                [0x97] = "Unassigned",
                [0x98] = "Unassigned",
                [0x99] = "Unassigned",
                [0x9A] = "Unassigned",
                [0x9B] = "Unassigned",
                [0x9C] = "Unassigned",
                [0x9D] = "Unassigned",
                [0x9E] = "Unassigned",
                [0x9F] = "Unassigned",
                [0xA0] = "VK_LSHIFT",
                [0xA1] = "VK_RSHIFT",
                [0xA2] = "VK_LCONTROL",
                [0xA3] = "VK_RCONTROL",
                [0xA4] = "VK_LMENU (Left Alt)",
                [0xA5] = "VK_RMENU (Right Alt)",
                [0xA6] = "VK_BROWSER_BACK",
                [0xA7] = "VK_BROWSER_FORWARD",
                [0xA8] = "VK_BROWSER_REFRESH",
                [0xA9] = "VK_BROWSER_STOP",
                [0xAA] = "VK_BROWSER_SEARCH",
                [0xAB] = "VK_BROWSER_FAVORITES", 
                [0xAC] = "VK_BROWSER_HOME",
                [0xAD] = "VK_VOLUME_MUTE",
                [0xAE] = "VK_VOLUME_DOWN",
                [0xAF] = "VK_VOLUME_UP",
                [0xB0] = "VK_MEDIA_NEXT_TRACK",
                [0xB1] = "VK_MEDIA_PREV_TRACK",
                [0xB2] = "VK_MEDIA_STOP",
                [0xB3] = "VK_MEDIA_PLAY_PAUSE",
                [0xB4] = "VK_LAUNCH_MAIL",
                [0xB5] = "VK_LAUNCH_MEDIA_SELECT",
                [0xB6] = "VK_LAUNCH_APP1",
                [0xB7] = "VK_LAUNCH_APP2",
                [0xB8] = "reserved",
                [0xB9] = "reserved",
                [0xBA] = "VK_OEM_1",
                [0xBB] = "VK_OEM_PLUS",
                [0xBC] = "VK_OEM_COMMA",
                [0xBD] = "VK_OEM_MINUS",
                [0xBE] = "VK_OEM_PERIOD",
                [0xBF] = "VK_OEM_2",
                [0xC0] = "VK_OEM_3",
                [0xC1] = "reserved",
                [0xC2] = "reserved",
                [0xC3] = "reserved",
                [0xC4] = "reserved",
                [0xC5] = "reserved",
                [0xC6] = "reserved",
                [0xC7] = "reserved",
                [0xC8] = "reserved",
                [0xC9] = "reserved",
                [0xCA] = "reserved",
                [0xCB] = "reserved",
                [0xCC] = "reserved",
                [0xCD] = "reserved",
                [0xCE] = "reserved",
                [0xCF] = "reserved",
                [0xD0] = "reserved",
                [0xD1] = "reserved",
                [0xD2] = "reserved",
                [0xD3] = "reserved",
                [0xD4] = "reserved",
                [0xD5] = "reserved",
                [0xD6] = "reserved",
                [0xD7] = "reserved",
                [0xD8] = "reserved",
                [0xD9] = "reserved",
                [0xDA] = "reserved",
                [0xDB] = "VK_OEM_4",
                [0xDC] = "VK_OEM_5",
                [0xDD] = "VK_OEM_6",
                [0xDE] = "VK_OEM_7",
                [0xDF] = "VK_OEM_8",
                [0xE0] = "reserved",
                [0xE1] = "reserved",
                [0xE2] = "VK_OEM_102",
                [0xE3] = "OEM specific",
                [0xE4] = "OEM specific",
                [0xE5] = "VK_PROCESSKEY",
                [0xE6] = "OEM specific",
                [0xE7] = "VK_PACKET",
                [0xE8] = "Unassigned",
                [0xE9] = "OEM specific",
                [0xEA] = "OEM specific",
                [0xEB] = "OEM specific",
                [0xEC] = "OEM specific",
                [0xED] = "OEM specific",
                [0xEE] = "OEM specific",
                [0xEF] = "OEM specific",
                [0xF0] = "OEM specific",
                [0xF1] = "OEM specific",
                [0xF2] = "OEM specific",
                [0xF3] = "OEM specific",
                [0xF4] = "OEM specific",
                [0xF5] = "OEM specific",
                [0xF6] = "VK_ATTN",
                [0xF7] = "VK_CRSEL",
                [0xF8] = "VK_EXSEL",
                [0xF9] = "VK_EREOF",
                [0xFA] = "VK_PLAY",
                [0xFB] = "VK_ZOOM",
                [0xFC] = "VK_NONAME",
                [0xFD] = "VK_PA1",
                [0xFE] = "VK_OEM_CLEAR",
            };
            
            DnfVirtualKeyToString = new Dictionary<int, string>() {
                [0x01] = "LeftMouse",           // VK_LBUTTON
                [0x02] = "RightMouse",          // VK_RBUTTON
                [0x03] = "Cancel",              // VK_CANCEL
                [0x04] = "MiddleMouse",         // VK_MBUTTON
                [0x05] = "MouseWheelDown",      // VK_XBUTTON1, note: not sure if correct 
                [0x06] = "MouseWheelUp",        // VK_XBUTTON2, note: not sure if correct
                [0x07] = "UNDEFINED_07",        // reserved TODO: figure out if ini uses this?
                [0x08] = "Backspace",           // VK_BACK
                [0x09] = "Tab",                 // VK_TAB
                [0x0A] = "Unknown0A",           // reserved
                [0x0B] = "Unknown0B",           // reserved
                [0x0C] = "UNDEFINED_0C",        // VK_CLEAR, TODO: figure out if ini uses this?
                [0x0D] = "Enter",               // VK_RETURN
                [0x0E] = "Unknown0E",           // reserved
                [0x0F] = "Unknown0F",           // reserved
                [0x10] = "Shift",               // VK_SHIFT
                [0x11] = "Ctrl",                // VK_CONTROL
                [0x12] = "Alt",                 // VK_MENU
                [0x13] = "Pause",               // VK_PAUSE
                [0x14] = "CapsLock",            // VK_CAPITAL
                [0x15] = "Unknown15",           // (IME)VK_KANA / VK_HANGUL
                [0x16] = "Unknown16",           // (IME)VK_IME_ON
                [0x17] = "Unknown17",           // (IME)VK_JUNJA
                [0x18] = "Unknown18",           // (IME)VK_FINAL
                [0x19] = "Unknown19",           // (IME)VK_HANJA / VK_KANJI
                [0x1A] = "Unknown1A",           // (IME)VK_IME_OFF
                [0x1B] = "Escape",              // VK_ESCAPE
                [0x1C] = "Unknown1C",           // (IME)VK_CONVERT
                [0x1D] = "Unknown1D",           // (IME)VK_NONCONVERT
                [0x1E] = "Unknown1E",           // (IME)VK_ACCEPT
                [0x1F] = "Unknown1F",           // (IME)VK_MODECHANGE
                [0x20] = "Space",               // VK_SPACE
                [0x21] = "PageUp",              // VK_PRIOR
                [0x22] = "PageDown",            // VK_NEXT
                [0x23] = "End",                 // VK_END
                [0x24] = "Home",                // VK_HOME
                [0x25] = "Left",                // VK_LEFT
                [0x26] = "Up",                  // VK_UP
                [0x27] = "Right",               // VK_RIGHT
                [0x28] = "Down",                // VK_DOWN
                [0x29] = "Select",              // VK_SELECT
                [0x2A] = "Print",               // VK_PRINT
                [0x2B] = "Execute",             // VK_EXECUTE
                [0x2C] = "PrintScrn",           // VK_SNAPSHOT
                [0x2D] = "Insert",              // VK_INSERT
                [0x2E] = "Delete",              // VK_DELETE
                [0x2F] = "Help",                // VK_HELP
                [0x30] = "0",                   // 
                [0x31] = "1",                   // 
                [0x32] = "2",                   // 
                [0x33] = "3",                   // 
                [0x34] = "4",                   // 
                [0x35] = "5",                   // 
                [0x36] = "6",                   // 
                [0x37] = "7",                   // 
                [0x38] = "8",                   // 
                [0x39] = "9",                   // 
                [0x3A] = "Mouse7",              // Undefined, note: not sure if correct assuming based on neighbouring key defs in the ini, TODO: figure out if ini uses this? 
                [0x3B] = "Mouse8",              // Undefined, note: not sure if correct assuming based on neighbouring key defs in the ini, TODO: figure out if ini uses this?
                [0x3C] = "Unknown3C",           // Undefined
                [0x3D] = "Unknown3D",           // Undefined
                [0x3E] = "Unknown3E",           // Undefined
                [0x3F] = "Unknown3F",           // Undefined
                [0x40] = "Unknown40",           // Undefined
                [0x41] = "A",                   //
                [0x42] = "B",                   //
                [0x43] = "C",                   //
                [0x44] = "D",                   //
                [0x45] = "E",                   //
                [0x46] = "F",                   //
                [0x47] = "G",                   //
                [0x48] = "H",                   //
                [0x49] = "I",                   //
                [0x4A] = "J",                   //
                [0x4B] = "K",                   //
                [0x4C] = "L",                   //
                [0x4D] = "M",                   //
                [0x4E] = "N",                   //
                [0x4F] = "O",                   //
                [0x50] = "P",                   //
                [0x51] = "Q",                   //
                [0x52] = "R",                   //
                [0x53] = "S",                   //
                [0x54] = "T",                   //
                [0x55] = "U",                   //
                [0x56] = "V",                   //
                [0x57] = "W",                   //
                [0x58] = "X",                   //
                [0x59] = "Y",                   //
                [0x5A] = "Z",                   //
                [0x5B] = "LWindows",            // VK_LWIN
                [0x5C] = "RWindows",            // VK_RWIN
                [0x5D] = "Application",         // VK_APPS (note: this is the menu key)
                [0x5E] = "Unknown5E",           // reserved
                [0x5F] = "Unknown5F",           // VK_SLEEP
                [0x60] = "NumPad0",             //
                [0x61] = "NumPad1",             //
                [0x62] = "NumPad2",             //
                [0x63] = "NumPad3",             //
                [0x64] = "NumPad4",             //
                [0x65] = "NumPad5",             //
                [0x66] = "NumPad6",             //
                [0x67] = "NumPad7",             //
                [0x68] = "NumPad8",             //
                [0x69] = "NumPad9",             //
                [0x6A] = "NumPadStar",          // VK_MULTIPLY
                [0x6B] = "NumPadPlus",          // VK_ADD
                [0x6C] = "Separator",           // VK_SEPARATOR
                [0x6D] = "NumPadMinus",         // VK_SUBTRACT
                [0x6E] = "NumPadPeriod",        // VK_DECIMAL
                [0x6F] = "NumPadSlash",         // VK_DIVIDE
                [0x70] = "F1",                  // VK_F1
                [0x71] = "F2",                  // VK_F2
                [0x72] = "F3",                  // VK_F3
                [0x73] = "F4",                  // VK_F4
                [0x74] = "F5",                  // VK_F5
                [0x75] = "F6",                  // VK_F6
                [0x76] = "F7",                  // VK_F7
                [0x77] = "F8",                  // VK_F8
                [0x78] = "F9",                  // VK_F9
                [0x79] = "F10",                 // VK_F10
                [0x7A] = "F11",                 // VK_F11
                [0x7B] = "F12",                 // VK_F12
                [0x7C] = "F13",                 // VK_F13
                [0x7D] = "F14",                 // VK_F14
                [0x7E] = "F15",                 // VK_F15
                [0x7F] = "F16",                 // VK_F16
                [0x80] = "F17",                 // VK_F17
                [0x81] = "F18",                 // VK_F18
                [0x82] = "F19",                 // VK_F19
                [0x83] = "F20",                 // VK_F20
                [0x84] = "F21",                 // VK_F21
                [0x85] = "F22",                 // VK_F22
                [0x86] = "F23",                 // VK_F23
                [0x87] = "F24",                 // VK_F24
                [0x88] = "Unknown88",           // reserved
                [0x89] = "Unknown89",           // reserved
                [0x8A] = "Unknown8A",           // reserved
                [0x8B] = "Unknown8B",           // reserved
                [0x8C] = "Unknown8C",           // reserved
                [0x8D] = "Unknown8D",           // reserved
                [0x8E] = "Unknown8E",           // reserved
                [0x8F] = "Unknown8F",           // reserved
                [0x90] = "NumLock",             // VK_NUMLOCK
                [0x91] = "ScrollLock",          // VK_SCROLL
                [0x92] = "Unknown92",           // OEM specific
                [0x93] = "Unknown93",           // OEM specific
                [0x94] = "Unknown94",           // OEM specific
                [0x95] = "Unknown95",           // OEM specific
                [0x96] = "Unknown96",           // OEM specific
                [0x97] = "Unknown97",           // Unassigned
                [0x98] = "Unknown98",           // Unassigned
                [0x99] = "Unknown99",           // Unassigned
                [0x9A] = "Unknown9A",           // Unassigned
                [0x9B] = "Unknown9B",           // Unassigned
                [0x9C] = "Unknown9C",           // Unassigned
                [0x9D] = "Unknown9D",           // Unassigned
                [0x9E] = "Unknown9E",           // Unassigned
                [0x9F] = "Unknown9F",           // Unassigned
                [0xA0] = "LShift",              // VK_LSHIFT
                [0xA1] = "RShift",              // VK_RSHIFT
                [0xA2] = "LControl",            // VK_LCONTROL
                [0xA3] = "RControl",            // VK_RCONTROL
                [0xA4] = "UnknownA4",           // VK_LMENU (Left Alt)
                [0xA5] = "UnknownA5",           // VK_RMENU (Right Alt)
                [0xA6] = "UnknownA6",           // VK_BROWSER_BACK
                [0xA7] = "UnknownA7",           // VK_BROWSER_FORWARD
                [0xA8] = "UnknownA8",           // VK_BROWSER_REFRESH
                [0xA9] = "UnknownA9",           // VK_BROWSER_STOP
                [0xAA] = "UnknownAA",           // VK_BROWSER_SEARCH
                [0xAB] = "UnknownAB",           // VK_BROWSER_FAVORITES 
                [0xAC] = "UnknownAC",           // VK_BROWSER_HOME
                [0xAD] = "UnknownAD",           // VK_VOLUME_MUTE
                [0xAE] = "UnknownAE",           // VK_VOLUME_DOWN
                [0xAF] = "UnknownAF",           // VK_VOLUME_UP
                [0xB0] = "UnknownB0",           // VK_MEDIA_NEXT_TRACK
                [0xB1] = "UnknownB1",           // VK_MEDIA_PREV_TRACK
                [0xB2] = "UnknownB2",           // VK_MEDIA_STOP
                [0xB3] = "UnknownB3",           // VK_MEDIA_PLAY_PAUSE
                [0xB4] = "UnknownB4",           // VK_LAUNCH_MAIL
                [0xB5] = "UnknownB5",           // VK_LAUNCH_MEDIA_SELECT
                [0xB6] = "UnknownB6",           // VK_LAUNCH_APP1
                [0xB7] = "UnknownB7",           // VK_LAUNCH_APP2
                [0xB8] = "UnknownB8",           // reserved
                [0xB9] = "UnknownB9",           // reserved
                [0xBA] = "Semicolon",           // VK_OEM_1
                [0xBB] = "Equals",              // VK_OEM_PLUS
                [0xBC] = "Comma",               // VK_OEM_COMMA
                [0xBD] = "Minus",               // VK_OEM_MINUS
                [0xBE] = "Period",              // VK_OEM_PERIOD
                [0xBF] = "Slash",               // VK_OEM_2
                [0xC0] = "Tilde",               // VK_OEM_3
                [0xC1] = "UnknownC1",           // reserved
                [0xC2] = "UnknownC2",           // reserved
                [0xC3] = "UnknownC3",           // reserved
                [0xC4] = "UNDEFINED_C4",        // reserved TODO: figure out if ini uses this?
                [0xC5] = "UNDEFINED_C5",        // reserved TODO: figure out if ini uses this?
                [0xC6] = "UNDEFINED_C6",        // reserved TODO: figure out if ini uses this?
                [0xC7] = "UNDEFINED_C7",        // reserved TODO: figure out if ini uses this?
                [0xC8] = "UNDEFINED_C8",        // reserved TODO: figure out if ini uses this?
                [0xC9] = "UNDEFINED_C9",        // reserved TODO: figure out if ini uses this?
                [0xCA] = "UNDEFINED_CA",        // reserved TODO: figure out if ini uses this?
                [0xCB] = "UNDEFINED_CB",        // reserved TODO: figure out if ini uses this?
                [0xCC] = "UNDEFINED_CC",        // reserved TODO: figure out if ini uses this?
                [0xCD] = "UNDEFINED_CD",        // reserved TODO: figure out if ini uses this?
                [0xCE] = "UNDEFINED_CE",        // reserved TODO: figure out if ini uses this?
                [0xCF] = "UNDEFINED_CF",        // reserved TODO: figure out if ini uses this?
                [0xD0] = "UNDEFINED_D0",        // reserved TODO: figure out if ini uses this?
                [0xD1] = "UNDEFINED_D1",        // reserved TODO: figure out if ini uses this?
                [0xD2] = "UNDEFINED_D2",        // reserved TODO: figure out if ini uses this?
                [0xD3] = "UNDEFINED_D3",        // reserved TODO: figure out if ini uses this?
                [0xD4] = "UNDEFINED_D4",        // reserved TODO: figure out if ini uses this?
                [0xD5] = "UNDEFINED_D5",        // reserved TODO: figure out if ini uses this?
                [0xD6] = "UNDEFINED_D6",        // reserved TODO: figure out if ini uses this?
                [0xD7] = "UNDEFINED_D7",        // reserved TODO: figure out if ini uses this?
                [0xD8] = "UnknownD8",           // reserved
                [0xD9] = "UnknownD9",           // reserved
                [0xDA] = "UnknownDA",           // reserved
                [0xDB] = "LeftBracket",         // VK_OEM_4
                [0xDC] = "Backslash",           // VK_OEM_5
                [0xDD] = "RightBracket",        // VK_OEM_6
                [0xDE] = "SingleQuote",         // VK_OEM_7
                [0xDF] = "UnknownDF",           // VK_OEM_8
                [0xE0] = "UNDEFINED_E0",        // reserved TODO: figure out if ini uses this?
                [0xE1] = "UNDEFINED_E1",        // reserved TODO: figure out if ini uses this?
                [0xE2] = "UNDEFINED_E2",        // VK_OEM_102 TODO: figure out if ini uses this?
                [0xE3] = "UNDEFINED_E3",        // OEM specific TODO: figure out if ini uses this?
                [0xE4] = "UNDEFINED_E4",        // OEM specific TODO: figure out if ini uses this?
                [0xE5] = "UNDEFINED_E5",        // VK_PROCESSKEY TODO: figure out if ini uses this?
                [0xE6] = "UNDEFINED_E6",        // OEM specific TODO: figure out if ini uses this?
                [0xE7] = "UNDEFINED_E7",        // VK_PACKET TODO: figure out if ini uses this?
                [0xE8] = "UNDEFINED_E8",        // Unassigned TODO: figure out if ini uses this?
                [0xE9] = "UNDEFINED_E9",        // OEM specific TODO: figure out if ini uses this?
                [0xEA] = "UnknownEA",           // OEM specific
                [0xEB] = "UnknownEB",           // OEM specific
                [0xEC] = "UNDEFINED_EC",        // OEM specific TODO: figure out if ini uses this?
                [0xED] = "UNDEFINED_ED",        // OEM specific TODO: figure out if ini uses this?
                [0xEE] = "UNDEFINED_EE",        // OEM specific TODO: figure out if ini uses this?
                [0xEF] = "UNDEFINED_EF",        // OEM specific TODO: figure out if ini uses this?
                [0xF0] = "UNDEFINED_F0",        // OEM specific TODO: figure out if ini uses this?
                [0xF1] = "UNDEFINED_F1",        // OEM specific TODO: figure out if ini uses this?
                [0xF2] = "UNDEFINED_F2",        // OEM specific TODO: figure out if ini uses this?
                [0xF3] = "UNDEFINED_F3",        // OEM specific TODO: figure out if ini uses this?
                [0xF4] = "UnknownF4",           // OEM specific
                [0xF5] = "UnknownF5",           // OEM specific
                [0xF6] = "Attn",                // VK_ATTN
                [0xF7] = "CrSel",               // VK_CRSEL
                [0xF8] = "ExSel",               // VK_EXSEL
                [0xF9] = "ErEof",               // VK_EREOF
                [0xFA] = "Play",                // VK_PLAY
                [0xFB] = "Zoom",                // VK_ZOOM
                [0xFC] = "NoName",              // VK_NONAME
                [0xFD] = "PA1",                 // VK_PA1
                [0xFE] = "OEMClear",            // VK_OEM_CLEAR
            };
            
            DnfStringToVirtualKey = new Dictionary<string, int>();

            foreach (var kvp in DnfVirtualKeyToString) {
                DnfStringToVirtualKey.Add(kvp.Value, kvp.Key);
            }
            
            // NOTE: other ini bindings that might be related to some of the VK codes but i'm not sure which...
            // Joy1, Joy2,..., Joy16
            // JoyPovRight, JoyPovLeft, JoyPovDown, JoyPovUp
            // JoyRLeft, JoyRDown, JoyRRight, JoyRUp
            // Unknown10F
            // Unknown10E
            // NumMid
            // NumMid
            // Mouse4, Mouse5, Mouse6
            // StrProperty___0
            
            // NOTE: ini bindings i'm not sure i assigned to the correct key codes..
            // MouseWheelDown, MouseWheelUp, Mouse8, Mouse7
            
            // NOTE: ini bindings that are analog axes and not virtual keys:
            // MouseX, MouseY, MouseZ, MouseW
            // JoyX, JoyY, JoyU, JoyV, JoyZ, JoyR 
            

        }


        public static Key GetKeyFromIniString(string str) {
            if (DnfStringToVirtualKey.TryGetValue(str, out int vk)) {
                Key key = KeyInterop.KeyFromVirtualKey(vk);
                return key;
            }
            return Key.None;
        }

        public static string GetKeyIniString(Key key) {
            int vk = Key2VirtualKey(key);
            if (DnfVirtualKeyToString.TryGetValue(vk, out string retVal)) {
                return retVal;
            }
            return "None";
        }
        
        public static string GetKeyVirtualKeyName(Key key) {
            int vk = Key2VirtualKey(key);
            if (VirtualKeyToString.TryGetValue(vk, out string retVal)) {
                return retVal;
            }
            return "None";
        }

        public static int Key2VirtualKey(Key key, bool combineRightLeftShiftCtrlAlt = true) {
            if (combineRightLeftShiftCtrlAlt) {
                if (key == Key.LeftShift || key == Key.RightShift)
                    return DnfHotkeyHelper.VK_SHIFT;
                if (key == Key.LeftCtrl || key == Key.RightCtrl)
                    return DnfHotkeyHelper.VK_CONTROL;
                if (key == Key.LeftAlt || key == Key.RightAlt)
                    return DnfHotkeyHelper.VK_MENU;
            }
            
            int vk = KeyInterop.VirtualKeyFromKey(key);
            if (key == Key.System) vk = DnfHotkeyHelper.VK_MENU; // special override for left alt key...
            return vk;
        }
        
        public static Key VirtualKey2Key(int vk) {
            return KeyInterop.KeyFromVirtualKey(vk);
        }

        public Dictionary<int, string> CurrentBindings { get; } = new Dictionary<int, string>();

        
        public bool RefreshBindingsFromUserIni(string fileName) {
            try {
                IniDocument? data = DnfIniParseHelper.ParseDnf2011Ini(fileName);
                if (data == null) {
                    return false;
                }
            
                Glog.Message(MyTraceCategory.General, $"Successfully parsed user.ini at path={fileName}");

                this.RefreshBindingsFromUserIni(data);
                return true;
            } catch (Exception ex) {
                Glog.Error(MyTraceCategory.General, "Error parsing user.ini file!", ex);
                return false;
            }
        }
        
        public bool RefreshBindingsFromUserIni(IniDocument data) {
            try {
                this.CurrentBindings.Clear();

                IniSection input = data["Engine.Input"];

                int bindings = 0;
                int nonemptyBindings = 0;

                foreach (var x in input.KeyDictionary) {
                    if (DnfStringToVirtualKey.TryGetValue(x.Key, out int vk)) {
                        string val = x.Value.GetSimpleValue();
                        if (string.IsNullOrWhiteSpace(val) == false) {
                            this.CurrentBindings.Add(vk, val);
                            nonemptyBindings ++;
                        }
                        bindings++;
                    }
                }
                
                Glog.Message(MyTraceCategory.General, $"Detected {bindings} hotkeys, out of which {nonemptyBindings} have command bindings assigned!");
                return true;
            } catch (Exception ex) {
                Glog.Error(MyTraceCategory.General, "Error parsing user.ini file!", ex);
                return false;
            }
        }
    }
}
