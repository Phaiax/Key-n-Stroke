using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace KeyNStroke
{
    [ComVisibleAttribute(false),
    System.Security.SuppressUnmanagedCodeSecurity()]
    public class NativeMethodsMouse
    {

        public const int WM_MOUSEMOVE = 0x200;
        public const int WM_LBUTTONDOWN = 0x201;
        public const int WM_LBUTTONUP = 0x202;
        public const int WM_LBUTTONDBLCLK = 0x203;
        public const int WM_RBUTTONDOWN = 0x204;
        public const int WM_RBUTTONUP = 0x205;
        public const int WM_RBUTTONDBLCLK = 0x206;
        public const int WM_MBUTTONDOWN = 0x207;
        public const int WM_MBUTTONUP = 0x208;
        public const int WM_MBUTTONDBLCLK = 0x209;
        public const int WM_MOUSEWHEEL1 = 0x20A;
        public const int WM_XBUTTONDOWN = 0x20B;
        public const int WM_XBUTTONUP = 0x20C;
        public const int WM_XBUTTONDBLCLK = 0x20D;
        public const int WM_MOUSEHWHEEL2 = 0x20E;


        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SetWindowsHookEx(int idHook,
            HookHandlerDelegate lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
            UIntPtr wParam, ref MSLLHOOKSTRUCT lParam);
        
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern int GetDoubleClickTime();

        public const int WH_MOUSE_LL = 14;
        public const int HC_ACTION = 0; // The wParam and lParam parameters contain information about a mouse message.

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public Int32 X;
            public Int32 Y;

            public POINT(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }

            public POINT(System.Drawing.Point pt) : this(pt.X, pt.Y) { }

            public static implicit operator System.Drawing.Point(POINT p)
            {
                return new System.Drawing.Point(p.X, p.Y);
            }

            public static implicit operator POINT(System.Drawing.Point p)
            {
                return new POINT(p.X, p.Y);
            }

            public static implicit operator System.Windows.Point(POINT p)
            {
                return new System.Windows.Point(p.X, p.Y);
            }

            public bool Equals(POINT p2)
            {
                return X == p2.X && Y == p2.Y;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MSLLHOOKSTRUCT {
            public POINT pt;
            public UInt32 mouseData; // be careful, this must be ints, not uints (was wrong before I changed it...). regards, cmew.
            public UInt32 flags;
            public UInt32 time;
            public UIntPtr dwExtraInfo;
        }

        public delegate IntPtr HookHandlerDelegate(int nCode,
                                             UIntPtr wParam,
                                             ref MSLLHOOKSTRUCT lParam);


        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool GetCursorPos(ref NativeMethodsMouse.POINT lpPoint);

        public static POINT CursorPosition
        {
            get
            {
                POINT pos = new POINT(0, 0);
                GetCursorPos(ref pos);
                return pos;
            }
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct CURSORINFO {
            public UInt32 cbSize;
            public UInt32 flags;
            public UIntPtr hCursor;
            public POINT ptScreenPos;
        }
        public const UInt32 CURSOR_HIDDEN = 0x00000000;
        public const UInt32 CURSOR_SHOWING = 0x00000001;
        public const UInt32 CURSOR_SUPPRESSED = 0x00000002;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool GetCursorInfo(ref CURSORINFO pci);

    }
}
