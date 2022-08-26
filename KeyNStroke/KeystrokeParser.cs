using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using KeyNStroke;

namespace KeyNStroke
{
    public class KeystrokeParser : IKeystrokeEventProvider
    {
        //KeysConverter Converter = new KeysConverter();
        public bool EnableTextOverSymbol { get; set; }

        #region Constructor

        public KeystrokeParser(IKeyboardRawEventProvider hook, bool enableTextOverSymbol)
        {
            hook.KeyEvent += hook_KeyEvent;
            EnableTextOverSymbol = enableTextOverSymbol;
        }

        #endregion

        /// <summary>
        /// Gets a KeyboardRawKeyEvent, parses it and forwards it via
        /// the KeystrokeEvent
        /// </summary>
        /// <param name="e"></param>
        void hook_KeyEvent(KeyboardRawEventArgs raw_e)
        {
            KeystrokeEventArgs e = new KeystrokeEventArgs(raw_e, EnableTextOverSymbol);

            e.IsAlpha = CheckIsAlpha(e.raw);
            e.IsNumericFromNumpad = CheckIsNumericFromNumpad(e.raw);
            e.IsNumericFromNumbers = CheckIsNumericFromNumbers(e.raw);
            e.IsNoUnicodekey = CheckIsNoUnicodekey(e.raw);
            e.IsFunctionKey = CheckIsFunctionKey(e.raw);
            e.ModifierToggledEvent = CheckKeyIsModifier(e.raw);

            Log.e("KP", "   alpha:" + e.IsAlpha.ToString());

            if (e.Method == KeyUpDown.Down)
            {
                ApplyDeadKey(e);
                if (e.IsAlpha && e.OnlyShiftOrCaps)
                {
                    e.TextModeString = ParseChar(e);
                    e.ShouldBeDisplayed = true;
                    e.StrokeType = KeystrokeType.Text;
                    e.Deletable = true;
                    Log.e("KP", "   IsAlpha and OnlyShiftOrCaps > ParseChar");
                }
                else if (e.IsNumeric && e.NoModifiers)
                {
                    e.TextModeString = ParseNumeric(e);
                    e.ShouldBeDisplayed = true;
                    e.StrokeType = KeystrokeType.Text;
                    e.Deletable = true;
                    Log.e("KP", "   e.IsNumeric && e.NoModifiers > ParseNumeric");
                }
                else if (e.ModifierToggledEvent) // key is modifier
                {
                    e.ShouldBeDisplayed = false;
                    AddModifier(e.Key, e);
                    e.StrokeType = KeystrokeType.Modifiers;
                    Log.e("KP", "   e.ModifierToggledEvent > AddModifier " + e.Key.ToString());
                }
                else if (e.IsNoUnicodekey && e.NoModifiers)
                {
                    ParseTexttViaSpecialkeysParser(e);
                    e.Deletable = IsDeletableSpecialKey(e.Key);
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
                        e.TextModeString = SpecialkeysParser.ToString(e.Key, EnableTextOverSymbol);
                    }
                    catch (NotImplementedException)
                    {
                        Log.e("KP", "   e.NoModifiers 2> try KeyboardLayoutParser.ParseViaToUnicode ");
                        e.TextModeString = KeyboardLayoutParser.ParseViaToUnicode(e.raw);
                        BackupDeadKey(e);
                    }
                    e.ShouldBeDisplayed = true;
                    e.StrokeType = KeystrokeType.Text;
                    e.RequiresNewLineAfterwards = e.Key == Key.Return;
                }
                else if (e.OnlyShiftOrCaps) //  special char, but only Shifted, eg ;:_ÖÄ'*ÜP
                // (e.IsNoUnicodekey is always false here -> could be a unicode key combination)
                {
                    e.TextModeString = KeyboardLayoutParser.ParseViaToUnicode(e.raw);
                    BackupDeadKey(e);
                    Log.e("KP", "   e.OnlyShiftOrCaps > try KeyboardLayoutParser.ParseViaToUnicode ");

                    if (e.TextModeString != "")
                    {
                        e.ShouldBeDisplayed = true;
                        e.StrokeType = KeystrokeType.Text;
                        e.Deletable = true;
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
                    e.TextModeString = ""; //KeyboardLayoutParser.ParseViaToUnicode(e);
                    Log.e("KP", "   !e.NoModifiers && !e.OnlyShiftOrCapss > KeyboardLayoutParser.ParseViaToUnicode");
                    // all other special char keycodes do not use Shift
                    if (e.TextModeString != "" && !e.Shift && !e.IsNoUnicodekey)
                    {
                        e.ShouldBeDisplayed = true;
                        e.StrokeType = KeystrokeType.Text;
                        e.Deletable = true;
                    }
                    else // Shortcut
                    {
                        ParseShortcutViaSpecialkeysParser(e);
                        string possibleChar = KeyboardLayoutParser.ParseViaToUnicode(e.raw);
                        BackupDeadKey(e);
                        if (possibleChar != "" && !CheckIsAlpha(possibleChar) 
                                        && !CheckIsNumeric(possibleChar)
                                        && CheckIsASCII(possibleChar))
                        {
                            e.TextModeString += " (" + possibleChar + ")";
                        }
                        Log.e("KP", "   !e.NoModifiers && !e.OnlyShiftOrCapss 2> ParseShortcutViaSpecialkeysParser ");
                    }
                }
                Log.e("KP", "   str:" + e.TextModeString);
                e.ShortcutString = e.AsShortcutString();
            }

            if (e.Method == KeyUpDown.Up)
            {
                if (e.ModifierToggledEvent) // key is modifier
                {
                    e.ShouldBeDisplayed = false;
                    RemoveModifier(e.Key, e);
                    e.StrokeType = KeystrokeType.Modifiers;
                }
                Log.e("KP", "   code:" + (e.Key).ToString());

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
            if(e.TextModeString == "DEADKEY")
            {
                e.TextModeString = "!";
                lastDeadKeyEvent = e;
            }
        }

        private bool ApplyDeadKey(KeystrokeEventArgs e)
        {
            if(lastDeadKeyEvent != null)
            {
                lastDeadKeyEvent.TextModeString = KeyboardLayoutParser.ProcessDeadkeyWithNextKey(lastDeadKeyEvent.raw, e.raw);
                if(lastDeadKeyEvent.TextModeString == "")
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
                e.TextModeString = SpecialkeysParser.ToString(e.Key, EnableTextOverSymbol);
            }
            catch (NotImplementedException)
            {
                e.TextModeString = e.Key.ToString();
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
                e.TextModeString = SpecialkeysParser.ToString(e.Key, EnableTextOverSymbol);
                e.ShouldBeDisplayed = true;
                e.StrokeType = KeystrokeType.Text;
                e.RequiresNewLineAfterwards = e.Key == Key.Return;
            }
            catch (NotImplementedException)
            {

            }
        }

        private void AddModifier(Key keys, KeystrokeEventArgs e)
        {
            switch (keys)
            {
                case Key.LeftCtrl: e.LCtrl = true; e.Ctrl = true; break;
                case Key.RightCtrl: e.RCtrl = true; e.Ctrl = true; break;
                case Key.LeftShift: e.LShift = true; e.Shift = true; break;
                case Key.RightShift: e.RShift = true; e.Shift = true; break;
                case Key.LWin: e.LWin = true; break;
                case Key.RWin: e.RWin = true; break;
                case Key.LeftAlt: e.LAlt = true; e.Alt = true; break;
                case Key.RightAlt: e.RAlt = true; e.Alt = true; break;
                case Key.NumLock: e.Numlock = true; break;
                case Key.Scroll: e.Scrollock = true; break;
                case Key.CapsLock: e.Caps = true; break;
            }
        }

        private void RemoveModifier(Key keys, KeystrokeEventArgs e)
        {
            // If Left Shift is released, then only unset Shift if RShift is not set
            switch (keys)
            {
                case Key.LeftCtrl: e.LCtrl = false; e.Ctrl = e.RCtrl; break;
                case Key.RightCtrl: e.RCtrl = false; e.Ctrl = e.LCtrl; break;
                case Key.LeftShift: e.LShift = false; e.Shift = e.RShift; break;
                case Key.RightShift: e.RShift = false; e.Shift = e.LShift; break;
                case Key.LWin: e.LWin = false; break;
                case Key.RWin: e.RWin = false; break;
                case Key.LeftAlt: e.LAlt = false; e.Alt = e.RAlt; break;
                case Key.RightAlt: e.RAlt = false; e.Alt = e.LAlt; break;
                case Key.NumLock: e.Numlock = false; break;
                case Key.Scroll: e.Scrollock = false; break;
                case Key.CapsLock: e.Caps = false; break;
            }
        }

        private string ParseChar(KeystrokeEventArgs e)
        {
            string c = e.Key.ToString();
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
                return ((int)e.Key - (int)Key.D0).ToString();
            }
            else if (e.IsNumericFromNumpad)
            {
                return ((int)e.Key - (int)Key.NumPad0).ToString();
            }
            return "BUG1";
        }

        bool CheckIsAlpha(KeyboardRawEventArgs e)
        {
            return CheckIsAlpha(e.Key);
        }

        bool CheckIsAlpha(Key key)
        {
            return ((int)Key.A <= (int)key && (int)key <= (int)Key.Z);
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
            return CheckIsNumericFromNumpad(e.Key);
        }

        bool CheckIsNumericFromNumpad(Key key)
        {
            return ((int)Key.NumPad0 <= (int)key && (int)key <= (int)Key.NumPad9);
        }

        bool CheckIsNumeric(string s)
        {
            return (s.Length == 1 && (s[0] >= '0' && s[0] <= '9'));
        }
        
        bool CheckIsNumericFromNumbers(KeyboardRawEventArgs e)
        {
            return CheckIsNumericFromNumbers(e.Key);
        }

        bool CheckIsNumericFromNumbers(Key key)
        {
            return ((int)Key.D0 <= (int)key && (int)key <= (int)Key.D9);
        }

        bool CheckIsFunctionKey(KeyboardRawEventArgs e)
        {
            return ((int)Key.F1 <= (int)e.Key && (int)e.Key <= (int)Key.F24);
        }

        /// <summary>
        /// If this function returns true, there will be no attemt to decode the key press
        /// via ToUnicode to reveal Strg+Alt+E = € .
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        bool CheckIsNoUnicodekey(KeyboardRawEventArgs e)
        {
            Key[] NoUnicodeKeys = { Key.Cancel, Key.Tab, Key.LineFeed, Key.Clear, 
                   Key.Enter, Key.Return, 
                   Key.Pause, Key.CapsLock, Key.Capital, Key.KanaMode,
                   Key.JunjaMode, Key.FinalMode, Key.KanjiMode, Key.HanjaMode,
                   Key.Escape, Key.ImeConvert, Key.ImeNonConvert, Key.ImeAccept,
                   Key.ImeModeChange, Key.Space, Key.Prior, Key.PageUp, Key.Next, 
                   Key.PageDown, Key.End, Key.Home, Key.Left, Key.Up, Key.Right, 
                   Key.Down, Key.Select, Key.Print, Key.Execute, Key.PrintScreen,
                   Key.Snapshot, Key.Insert, Key.Delete, Key.Help,
                   Key.LWin, Key.RWin, Key.Apps, Key.Sleep,
                   Key.Multiply, Key.Add, Key.Separator, Key.Subtract, Key.Decimal, 
                   Key.Divide, Key.F1, Key.F2, Key.F3, Key.F4, Key.F5, Key.F6, 
                   Key.F7, Key.F8, Key.F9, Key.F10, Key.F11, Key.F12, Key.F13,
                   Key.F14, Key.F15, Key.F16, Key.F17, Key.F18, Key.F19, Key.F20, 
                   Key.F21, Key.F22, Key.F23, Key.F24, Key.NumLock, Key.Scroll, 
                   Key.LeftShift, Key.RightShift, Key.LeftCtrl, Key.RightCtrl,
                   Key.LeftAlt, Key.RightAlt, Key.BrowserBack, Key.BrowserForward, 
                   Key.BrowserRefresh, Key.BrowserStop, Key.BrowserSearch, 
                   Key.BrowserFavorites, Key.BrowserHome, Key.VolumeMute, Key.VolumeDown, 
                   Key.VolumeUp, Key.MediaNextTrack, Key.MediaPreviousTrack, Key.MediaStop, 
                   Key.MediaPlayPause, Key.LaunchMail, Key.SelectMedia, Key.LaunchApplication1, 
                   Key.LaunchApplication2, Key.ImeProcessed,
                   0, Key.Attn, Key.CrSel, Key.ExSel, Key.EraseEof, Key.Play,
                   Key.Zoom, Key.NoName, Key.Pa1, Key.OemClear };
            return NoUnicodeKeys.Contains(e.Key);
        }

        private bool IsDeletableSpecialKey(Key key)
        {
            Key[] DeletableKeys = { 
                   Key.Multiply, Key.Add, Key.Subtract, Key.Decimal, 
                   Key.Divide, Key.Space};
            return DeletableKeys.Contains(key);
        }


        bool CheckKeyIsModifier(KeyboardRawEventArgs e)
        {
            Key[] ModifierKeys = { Key.LeftShift, Key.RightShift,
                                   Key.LeftCtrl, Key.RightCtrl,
                                   Key.LeftAlt, Key.RightAlt,
                                   Key.LWin, Key.RWin,
                                   Key.NumLock, Key.Scroll,
                                   Key.CapsLock};
            return ModifierKeys.Contains(e.Key);
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
