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

    public class KeystrokeEventArgs : KeyboardRawEventArgs
    {
        public string TextModeString; // for use in TextMode
        public string ShortcutString; // will always contain a shortcut

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
            if (OrigShift) Modifiers.Add(SpecialkeysParser.ToString(Key.LeftShift));
            if (Ctrl) Modifiers.Add(SpecialkeysParser.ToString(Key.LeftCtrl));
            if (Alt) Modifiers.Add(SpecialkeysParser.ToString(Key.LeftAlt));
            if (Win) Modifiers.Add(SpecialkeysParser.ToString(Key.LWin));
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


        public KeystrokeEventArgs(KeyboardRawEventArgs e)
            : base(e.Kbdllhookstruct)
        {
            this.Shift = e.Shift;
            this.LShift = e.LShift;
            this.RShift = e.RShift;
            this.Ctrl = e.Ctrl;
            this.LCtrl = e.LCtrl;
            this.RCtrl = e.RCtrl;
            this.Caps = e.Caps;
            this.LWin = e.LWin;
            this.RWin = e.RWin;
            this.Alt = e.Alt;
            this.LAlt = e.LAlt;
            this.RAlt = e.RAlt;
            this.Numlock = e.Numlock;
            this.Scrollock = e.Scrollock;
            //this.Kbdllhookstruct = e.Kbdllhookstruct;
            this.keyState = e.keyState;
            this.Method = e.Method;

            this.OrigShift = e.Shift;
            this.OrigCaps = e.Caps;
            this.OrigLShift = e.LShift;
            this.OrigRShift = e.RShift;
        }
    }

    public delegate void KeystrokeEventHandler(KeystrokeEventArgs e);

    public interface IKeystrokeEventProvider
    {
        event KeystrokeEventHandler KeystrokeEvent;
    }
}
