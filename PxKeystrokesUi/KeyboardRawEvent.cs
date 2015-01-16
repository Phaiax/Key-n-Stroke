using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PxKeystrokesUi
{
    public enum KeyUpDown
    {
        Undefined,
        Up,
        Down
    }

    public class KeyboardRawEventArgs
    {
        public bool Shift;
        public bool LShift;
        public bool RShift;
        public bool Ctrl;
        public bool LCtrl;
        public bool RCtrl;
        public bool Caps;
        public bool LWin;
        public bool RWin;
        public bool Alt;
        public bool LAlt;
        public bool RAlt; // Alt Gr
        public bool Numlock;
        public bool Scrollock;

        public int vkCode { get { return Kbdllhookstruct.vkCode; } }
        public Keys Key { get { return (Keys)vkCode; } }
        public KeyUpDown Method = KeyUpDown.Undefined;

        public bool Uppercase { get { return (Shift && !Caps) || (Caps && !Shift);  } }
        public bool OnlyShiftOrCaps { get { return !Ctrl && !LWin && !RWin && !Alt;  } }
        public bool NoModifiers { get { return !Ctrl && !LWin && !RWin && !Alt && !Shift; } }
        public bool Win { get { return LWin || RWin; } }

        public NativeMethodsKeyboard.KBDLLHOOKSTRUCT Kbdllhookstruct;
        public byte[] keyState; // 256 bytes

        public KeyboardRawEventArgs(NativeMethodsKeyboard.KBDLLHOOKSTRUCT Kbdllhookstruct)
        {
            this.Kbdllhookstruct = Kbdllhookstruct;
        }
    }

    public delegate void KeyboardRawEventHandler(KeyboardRawEventArgs e);

    interface IKeyboardRawEventProvider : IDisposable
    {
        event KeyboardRawEventHandler KeyEvent;
    }

}
