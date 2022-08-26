using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KeyNStroke
{
    public enum KeystrokeType
    {
        Undefined,
        Shortcut,
        Text,
        Modifiers
    }

    public class KeystrokeEventArgs
    {
        public string TextModeString; // for use in TextMode
        public string ShortcutString; // will always contain a shortcut

        public KeyboardRawEventArgs raw;

        public bool Shift { get { return raw.Shift; } set { raw.Shift = value; } }
        public bool LShift { get { return raw.LShift; } set { raw.LShift = value; } }
        public bool RShift { get { return raw.RShift; } set { raw.RShift = value; } }
        public bool Ctrl { get { return raw.Ctrl; } set { raw.Ctrl = value; } }
        public bool LCtrl { get { return raw.LCtrl; } set { raw.LCtrl = value; } }
        public bool RCtrl { get { return raw.RCtrl; } set { raw.RCtrl = value; } }
        public bool Caps { get { return raw.Caps; } set { raw.Caps = value; } }
        public bool LWin { get { return raw.LWin; } set { raw.LWin = value; } }
        public bool RWin { get { return raw.RWin; } set { raw.RWin = value; } }
        public bool Alt { get { return raw.Alt; } set { raw.Alt = value; } }
        public bool LAlt { get { return raw.LAlt; } set { raw.LAlt = value; } }
        public bool RAlt { get { return raw.RAlt; } set { raw.RAlt = value; } } // Alt Gr
        public bool Numlock { get { return raw.Numlock; } set { raw.Numlock = value; } }
        public bool Scrollock { get { return raw.Scrollock; } set { raw.Scrollock = value; } }

        public Key Key { get { return raw.Key; } }
        public KeyUpDown Method { get { return raw.Method; } }

        public bool Uppercase { get { return raw.Uppercase; } }
        public bool OnlyShiftOrCaps { get { return raw.OnlyShiftOrCaps; } }
        public bool NoModifiers { get { return raw.NoModifiers; } }
        public bool Win { get { return raw.Win; } }

        public bool OrigShift;
        public bool OrigLShift;
        public bool OrigRShift;
        public bool OrigCaps;

        public bool IsAlpha;
        public bool IsNumericFromNumpad;
        public bool IsNumericFromNumbers;
        public bool IsFunctionKey;
        public bool IsNoUnicodekey;
        public bool ModifierToggledEvent;

        public KeystrokeType StrokeType = KeystrokeType.Undefined;
        public bool ShouldBeDisplayed;
        public bool RequiresNewLine;
        public bool RequiresNewLineAfterwards;
        public bool Deletable = false;

        public bool IsNumeric { get { return IsNumericFromNumbers || IsNumericFromNumpad;  } }

        public override string ToString()
        {
            return ToString(true);
        }

        public string ToString(bool textMode)
        {
            if (ShortcutString != null && (StrokeType == KeystrokeType.Shortcut || !textMode))
            {
                return ShortcutString;
            }
            else if (StrokeType == KeystrokeType.Text && textMode)
            {
                return TextModeString;
            }
            return "BUG2";
        }

        public string ToString(bool textMode, bool DoubleAmpersand)
        {
            return DoubleAmpersand ? ToString(textMode).Replace("&", "&&") : ToString(textMode);
        }

        public string AsShortcutString()
        {
            List<string> output = ShortcutModifiersToList();
            if (StrokeType == KeystrokeType.Text)
            {
                output.Add(TextModeString.ToUpper());
            }
            else
            {
                output.Add(TextModeString);
            }
            return string.Join(" + ", output);
        }

        public List<string> ShortcutModifiersToList()
        {
            List<string> Modifiers = new List<string>();
            if (OrigShift) Modifiers.Add(SpecialkeysParser.ToString(Key.LeftShift, EnableTextOverSymbol));
            if (Ctrl) Modifiers.Add(SpecialkeysParser.ToString(Key.LeftCtrl, EnableTextOverSymbol));
            if (Alt) Modifiers.Add(SpecialkeysParser.ToString(Key.LeftAlt, EnableTextOverSymbol));
            if (Win) Modifiers.Add(SpecialkeysParser.ToString(Key.LWin, EnableTextOverSymbol));
            return Modifiers;
        }

        public string ShortcutIdentifier()
        {
            if (StrokeType == KeystrokeType.Text)
            {
                return null;
            }
            else if(StrokeType == KeystrokeType.Shortcut)
            {

                List<string> output = new List<string>();

                if (this.LCtrl)
                {
                    output.Add("LeftCtrl");
                }
                if (this.RCtrl)
                {
                    output.Add("RightCtrl");
                }
                // else if (this.Ctrl)
                // {
                    // output.Add("Ctrl");
                // }

                if (this.LWin)
                {
                    output.Add("LeftWin");
                }
                if (this.RWin)
                {
                    output.Add("RightWin");
                }

                if (this.LAlt)
                {
                    output.Add("LeftAlt");
                }
                if (this.RAlt)
                {
                    output.Add("RightAlt");
                }
                // else if (this.Alt)
                // {
                    // output.Add("Alt");
                // }

                if (this.LShift)
                {
                    output.Add("LeftShift");
                }
                if (this.RShift)
                {
                    output.Add("RightShift");
                }
                // else if (this.Shift)
                // {
                // output.Add("Shift");
                // }
                string trimmed = TextModeString.Trim();
                if (trimmed.Length == 0) // The Space key
                {
                    output.Add(TextModeString);
                }
                else
                {
                    output.Add(trimmed);
                }
                return string.Join(" + ", output);
            }
            return null;
        }

        private bool EnableTextOverSymbol;

        public KeystrokeEventArgs(KeyboardRawEventArgs e, bool enableTextOverSymbol)
        {
            this.raw = e;
            this.OrigShift = e.Shift;
            this.OrigCaps = e.Caps;
            this.OrigLShift = e.LShift;
            this.OrigRShift = e.RShift;
            this.EnableTextOverSymbol = enableTextOverSymbol;
        }
    }

    public delegate void KeystrokeEventHandler(KeystrokeEventArgs e);

    public interface IKeystrokeEventProvider
    {
        bool EnableTextOverSymbol { get; set; }

        event KeystrokeEventHandler KeystrokeEvent;
    }
}
