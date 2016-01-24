using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PxKeystrokesUi
{
    class KeystrokeParser : IKeystrokeEventProvider
    {
        KeysConverter Converter = new KeysConverter();

        #region Constructor

        public KeystrokeParser(IKeyboardRawEventProvider hook)
        {
            hook.KeyEvent += hook_KeyEvent;
        }

        #endregion

        /// <summary>
        /// Gets a KeyboardRawKeyEvent, parses it and forwards it via
        /// the KeystrokeEvent
        /// </summary>
        /// <param name="e"></param>
        void hook_KeyEvent(KeyboardRawEventArgs raw_e)
        {
            KeystrokeEventArgs e = new KeystrokeEventArgs(raw_e);

            e.IsAlpha = CheckIsAlpha(e);
            e.IsNumericFromNumpad = CheckIsNumericFromNumpad(e);
            e.IsNumericFromNumbers = CheckIsNumericFromNumbers(e);
            e.IsNoUnicodekey = CheckIsNoUnicodekey(e);
            e.IsFunctionKey = CheckIsFunctionKey(e);
            e.ModifierToggledEvent = CheckVkCodeIsModifier(e);

            Log.e("KP", "   alpha:" + e.IsAlpha.ToString());

            if (e.Method == KeyUpDown.Down)
            {
                ApplyDeadKey(e);
                if (e.IsAlpha && e.OnlyShiftOrCaps)
                {
                    e.KeyString = ParseChar(e);
                    e.ShouldBeDisplayed = true;
                    e.StrokeType = KeystrokeType.Text;
                    Log.e("KP", "   IsAlpha and OnlyShiftOrCaps > ParseChar");
                }
                else if (e.IsNumeric && e.NoModifiers)
                {
                    e.KeyString = ParseNumeric(e);
                    e.ShouldBeDisplayed = true;
                    e.StrokeType = KeystrokeType.Text;
                    Log.e("KP", "   e.IsNumeric && e.NoModifiers > ParseNumeric");
                }
                else if (e.ModifierToggledEvent) // vkcode is modifier
                {
                    e.ShouldBeDisplayed = false;
                    AddModifier((Keys)e.vkCode, e);
                    e.StrokeType = KeystrokeType.Modifiers;
                    Log.e("KP", "   e.ModifierToggledEvent > AddModifier " + ((Keys)e.vkCode).ToString());
                }
                else if (e.IsNoUnicodekey && e.NoModifiers)
                {
                    ParseTexttViaSpecialkeysParser(e);
                    Log.e("KP", "   e.IsNoUnicodekey && e.NoModifiers > ParseTexttViaSpecialkeysParser ");
                }
                else if (e.IsNoUnicodekey && !e.NoModifiers) // Shortcut
                {
                    ParseShortcutViaSpecialkeysParser(e);
                    Log.e("KP", "   e.IsNoUnicodekey && !e.NoModifiers > ParseShortcutViaSpecialkeysParser ");
                }
                else if (e.NoModifiers) // Simple Key, but not alphanumeric (first try special then unicode)
                {
                    Log.e("KP", "   e.NoModifiers > try SpecialkeysParser.ToString ");
                    try
                    {
                        e.KeyString = SpecialkeysParser.ToString((Keys)e.vkCode);
                    }
                    catch (NotImplementedException)
                    {
                        Log.e("KP", "   e.NoModifiers 2> try KeyboardLayoutParser.ParseViaToUnicode ");
                        e.KeyString = KeyboardLayoutParser.ParseViaToUnicode(e);
                        BackupDeadKey(e);
                    }
                    e.ShouldBeDisplayed = true;
                    e.StrokeType = KeystrokeType.Text;
                    e.RequiresNewLineAfterwards = (Keys)e.vkCode == Keys.Return;
                }
                else if (e.OnlyShiftOrCaps) //  special char, but only Shifted, eg ;:_ÖÄ'*ÜP
                // (e.IsNoUnicodekey is always false here -> could be a unicode key combinatin)
                {
                    e.KeyString = KeyboardLayoutParser.ParseViaToUnicode(e);
                    BackupDeadKey(e);
                    Log.e("KP", "   e.OnlyShiftOrCaps > try KeyboardLayoutParser.ParseViaToUnicode ");

                    if (e.KeyString != "")
                    {
                        e.ShouldBeDisplayed = true;
                        e.StrokeType = KeystrokeType.Text;
                    }
                    else
                    {
                        // Is no unicode key combination? maybe a shortcut then: Shift + F2
                        ParseShortcutViaSpecialkeysParser(e);
                        Log.e("KP", "   e.OnlyShiftOrCaps 2> ParseShortcutViaSpecialkeysParser ");
                    }
                }
                else if (!e.NoModifiers && !e.OnlyShiftOrCaps) // Special Char with Strg + Alt
                // or shortcut else
                {

                    // could be something like the german @ (Ctrl + Alt + Q)
                    // Temporary disabled because ToUnicode returns more often values than it should
                    e.KeyString = ""; //KeyboardLayoutParser.ParseViaToUnicode(e);
                    Log.e("KP", "   !e.NoModifiers && !e.OnlyShiftOrCapss > KeyboardLayoutParser.ParseViaToUnicode");
                    // all other special char keycodes do not use Shift
                    if (e.KeyString != "" && !e.Shift && !e.IsNoUnicodekey)
                    {
                        e.ShouldBeDisplayed = true;
                        e.StrokeType = KeystrokeType.Text;
                    }
                    else // Shortcut
                    {
                        ParseShortcutViaSpecialkeysParser(e);
                        string possibleChar = KeyboardLayoutParser.ParseViaToUnicode(e);
                        BackupDeadKey(e);
                        if (possibleChar != "" && !CheckIsAlpha(possibleChar) 
                                        && !CheckIsNumeric(possibleChar)
                                        && CheckIsASCII(possibleChar))
                        {
                            e.KeyString += " (" + possibleChar + ")";
                        }
                        Log.e("KP", "   !e.NoModifiers && !e.OnlyShiftOrCapss 2> ParseShortcutViaSpecialkeysParser ");
                    }
                }
                Log.e("KP", "   str:" + e.KeyString);

            }

            if (e.Method == KeyUpDown.Up)
            {
                if (e.ModifierToggledEvent) // vkcode is modifier
                {
                    e.ShouldBeDisplayed = false;
                    RemoveModifier((Keys)e.vkCode, e);
                    e.StrokeType = KeystrokeType.Modifiers;
                }
                Log.e("KP", "   code:" + ((Keys)e.vkCode).ToString());

                // only react to modifiers on key up, nothing else
            }

            if (e.StrokeType != KeystrokeType.Undefined)
            {
                OnKeystrokeEvent(e);
            }

        }

        private KeystrokeEventArgs lastDeadKeyEvent;

        private void BackupDeadKey(KeystrokeEventArgs e)
        {
            if(e.KeyString == "DEADKEY")
            {
                e.KeyString = "!";
                lastDeadKeyEvent = e;
            }
        }

        private bool ApplyDeadKey(KeystrokeEventArgs e)
        {
            if(lastDeadKeyEvent != null)
            {
                lastDeadKeyEvent.KeyString = KeyboardLayoutParser.ProcessDeadkeyWithNextKey(lastDeadKeyEvent, e);
                if(lastDeadKeyEvent.KeyString == "")
                {
                    lastDeadKeyEvent = null;
                    return false;
                }
                OnKeystrokeEvent(lastDeadKeyEvent);
                lastDeadKeyEvent = null;
                return true;
            }
            return false;
        }

        private void ParseShortcutViaSpecialkeysParser(KeystrokeEventArgs e)
        {
            try
            {
                e.KeyString = SpecialkeysParser.ToString(e.Key);
            }
            catch (NotImplementedException)
            {
                e.KeyString = e.Key.ToString();
            }
            e.ShouldBeDisplayed = true;
            e.StrokeType = KeystrokeType.Shortcut;
            e.RequiresNewLine = true;
            e.RequiresNewLineAfterwards = true;
        }

        private void ParseTexttViaSpecialkeysParser(KeystrokeEventArgs e)
        {
            try
            {
                e.KeyString = SpecialkeysParser.ToString(e.Key);
                e.ShouldBeDisplayed = true;
                e.StrokeType = KeystrokeType.Text;
                e.RequiresNewLineAfterwards = (Keys)e.vkCode == Keys.Return;
            }
            catch (NotImplementedException)
            {

            }
        }

        private void AddModifier(Keys keys, KeystrokeEventArgs e)
        {
            switch (keys)
            {
                case Keys.ControlKey: e.Ctrl = true; break;
                case Keys.LControlKey: e.LCtrl = true; e.Ctrl = true; break;
                case Keys.RControlKey: e.RCtrl = true; e.Ctrl = true; break;
                case Keys.ShiftKey: e.Shift = true; break;
                case Keys.LShiftKey: e.LShift = true; e.Shift = true; break;
                case Keys.RShiftKey: e.RShift = true; e.Shift = true; break;
                case Keys.LWin: e.LWin = true; break;
                case Keys.RWin: e.RWin = true; break;
                case Keys.Menu: e.Alt = true; break;
                case Keys.LMenu: e.LAlt = true; e.Alt = true; break;
                case Keys.RMenu: e.RAlt = true; e.Alt = true; break;
                case Keys.NumLock: e.Numlock = true; break;
                case Keys.Scroll: e.Scrollock = true; break;
                case Keys.CapsLock: e.Caps = true; break;
            }
        }

        private void RemoveModifier(Keys keys, KeystrokeEventArgs e)
        {
            // If Left Shift is released, then only unset Shift if RShift is not set
            switch (keys)
            {
                case Keys.ControlKey: e.Ctrl = false; break;
                case Keys.LControlKey: e.LCtrl = false; e.Ctrl = e.RCtrl; break;
                case Keys.RControlKey: e.RCtrl = false; e.Ctrl = e.LCtrl; break;
                case Keys.ShiftKey: e.Shift = false; break;
                case Keys.LShiftKey: e.LShift = false; e.Shift = e.RShift; break;
                case Keys.RShiftKey: e.RShift = false; e.Shift = e.LShift; break;
                case Keys.LWin: e.LWin = false; break;
                case Keys.RWin: e.RWin = false; break;
                case Keys.Menu: e.Alt = false; break;
                case Keys.LMenu: e.LAlt = false; e.Alt = e.RAlt; break;
                case Keys.RMenu: e.RAlt = false; e.Alt = e.LAlt; break;
                case Keys.NumLock: e.Numlock = false; break;
                case Keys.Scroll: e.Scrollock = false; break;
                case Keys.CapsLock: e.Caps = false; break;
            }
        }

        private string ParseChar(KeystrokeEventArgs e)
        {
            string c = ((Keys)e.vkCode).ToString();
            if (!e.Uppercase)
                c = c.ToLower();

            // We parse the Shift and Caps keys into the Upper/Lowercase
            e.Shift = false;
            e.Caps = false;
            e.LShift = false;
            e.RShift = false;
            return c;
        }

        private string ParseNumeric(KeystrokeEventArgs e)
        {
            if (e.IsNumericFromNumbers)
            {
                return (e.vkCode - (int)Keys.D0).ToString();
            }
            else if (e.IsNumericFromNumpad)
            {
                return (e.vkCode - (int)Keys.NumPad0).ToString();
            }
            return "BUG1";
        }

        bool CheckIsAlpha(KeyboardRawEventArgs e)
        {
            return CheckIsAlpha(e.vkCode);
        }

        bool CheckIsAlpha(int vkCode)
        {
            return ((int)Keys.A <= vkCode && vkCode <= (int)Keys.Z);
        }

        bool CheckIsAlpha(string s)
        {
            return (s.Length == 1 && ( (s[0] >= 'a' && s[0] <= 'z') || (s[0] >= 'A' && s[0] <= 'Z')));
        }

        bool CheckIsASCII(string s)
        {
            return (s.Length == 1 && ((s[0] >= 32 && s[0] <= 126) ));
        }

        bool CheckIsNumericFromNumpad(KeyboardRawEventArgs e)
        {
            return CheckIsNumericFromNumpad(e.vkCode);
        }

        bool CheckIsNumericFromNumpad(int vkCode)
        {
            return ((int)Keys.NumPad0 <= vkCode && vkCode <= (int)Keys.NumPad9);
        }

        bool CheckIsNumeric(string s)
        {
            return (s.Length == 1 && (s[0] >= '0' && s[0] <= '9'));
        }
        
        bool CheckIsNumericFromNumbers(KeyboardRawEventArgs e)
        {
            return CheckIsNumericFromNumbers(e.vkCode);
        }

        bool CheckIsNumericFromNumbers(int vkCode)
        {
            return ((int)Keys.D0 <= vkCode && vkCode <= (int)Keys.D9);
        }

        bool CheckIsFunctionKey(KeyboardRawEventArgs e)
        {
            return ((int)Keys.F1 <= e.vkCode && e.vkCode <= (int)Keys.F24);
        }

        /// <summary>
        /// If this function returns true, there will be no attemt to decode the key press
        /// via ToUnicode to reveal Strg+Alt+E = € .
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        bool CheckIsNoUnicodekey(KeyboardRawEventArgs e)
        {
            Keys[] NoUnicodeKeys = { Keys.Cancel, Keys.Tab, Keys.LineFeed, Keys.Clear, 
                   Keys.Enter, Keys.Return, Keys.ShiftKey, Keys.ControlKey, Keys.Menu, 
                   Keys.Pause, Keys.CapsLock, Keys.Capital, Keys.KanaMode, Keys.HanguelMode,
                   Keys.HangulMode, Keys.JunjaMode, Keys.FinalMode, Keys.KanjiMode, Keys.HanjaMode,
                   Keys.Escape, Keys.IMEConvert, Keys.IMENonconvert, Keys.IMEAceept, Keys.IMEAccept,
                   Keys.IMEModeChange, Keys.Space, Keys.Prior, Keys.PageUp, Keys.Next, 
                   Keys.PageDown, Keys.End, Keys.Home, Keys.Left, Keys.Up, Keys.Right, 
                   Keys.Down, Keys.Select, Keys.Print, Keys.Execute, Keys.PrintScreen,
                   Keys.Snapshot, Keys.Insert, Keys.Delete, Keys.Help,
                   Keys.LWin, Keys.RWin, Keys.Apps, Keys.Sleep,
                   Keys.Multiply, Keys.Add, Keys.Separator, Keys.Subtract, Keys.Decimal, 
                   Keys.Divide, Keys.F1, Keys.F2, Keys.F3, Keys.F4, Keys.F5, Keys.F6, 
                   Keys.F7, Keys.F8, Keys.F9, Keys.F10, Keys.F11, Keys.F12, Keys.F13,
                   Keys.F14, Keys.F15, Keys.F16, Keys.F17, Keys.F18, Keys.F19, Keys.F20, 
                   Keys.F21, Keys.F22, Keys.F23, Keys.F24, Keys.NumLock, Keys.Scroll, 
                   Keys.LShiftKey, Keys.RShiftKey, Keys.LControlKey, Keys.RControlKey,
                   Keys.LMenu, Keys.RMenu, Keys.BrowserBack, Keys.BrowserForward, 
                   Keys.BrowserRefresh, Keys.BrowserStop, Keys.BrowserSearch, 
                   Keys.BrowserFavorites, Keys.BrowserHome, Keys.VolumeMute, Keys.VolumeDown, 
                   Keys.VolumeUp, Keys.MediaNextTrack, Keys.MediaPreviousTrack, Keys.MediaStop, 
                   Keys.MediaPlayPause, Keys.LaunchMail, Keys.SelectMedia, Keys.LaunchApplication1, 
                   Keys.LaunchApplication2, Keys.ProcessKey,
                   Keys.Packet, Keys.Attn, Keys.Crsel, Keys.Exsel, Keys.EraseEof, Keys.Play,
                   Keys.Zoom, Keys.NoName, Keys.Pa1, Keys.OemClear, Keys.KeyCode, Keys.Shift,
                   Keys.Control, Keys.Alt };
            return NoUnicodeKeys.Contains((Keys)e.vkCode);
        }

        bool CheckVkCodeIsModifier(KeyboardRawEventArgs e)
        {
            Keys[] ModifierKeys = { Keys.ControlKey, Keys.LControlKey, Keys.RControlKey,
                                    Keys.ShiftKey, Keys.LShiftKey, Keys.RShiftKey,
                                    Keys.LWin, Keys.RWin,
                                    Keys.Menu, Keys.LMenu, Keys.RMenu,
                                    Keys.NumLock, Keys.Scroll,
                                    Keys.CapsLock};
            return ModifierKeys.Contains((Keys)e.vkCode);
        }

        #region Event Forwarding

        public event KeystrokeEventHandler KeystrokeEvent;

        private void OnKeystrokeEvent(KeystrokeEventArgs e)
        {
            if (KeystrokeEvent != null)
            {
                KeystrokeEvent(e);
            }
        }

        #endregion
    }
}
