using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PxKeystrokesWPF
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
        public string KeyString;

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
            if (StrokeType == KeystrokeType.Text)
            {
                return KeyString;
            }
            else if(StrokeType == KeystrokeType.Shortcut)
            {
                List<string> output = ShortcutModifiersToList();
                output.Add(KeyString);
                return string.Join(" + ", output);
            }
            return "BUG2";
        }

        public string ToString(bool DoubleAmpersand)
        {
            return DoubleAmpersand ? ToString().Replace("&", "&&") : ToString();
        }

        public List<string> ShortcutModifiersToList()
        {
            List<string> Modifiers = new List<string>();
            if (Shift) Modifiers.Add(SpecialkeysParser.ToString(Key.LeftShift));
            if (Ctrl) Modifiers.Add(SpecialkeysParser.ToString(Key.LeftCtrl));
            if (Alt) Modifiers.Add(SpecialkeysParser.ToString(Key.LeftAlt));
            if (Win) Modifiers.Add(SpecialkeysParser.ToString(Key.LWin));
            return Modifiers;
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
        }
    }

    public delegate void KeystrokeEventHandler(KeystrokeEventArgs e);

    public interface IKeystrokeEventProvider
    {
        event KeystrokeEventHandler KeystrokeEvent;
    }
}
