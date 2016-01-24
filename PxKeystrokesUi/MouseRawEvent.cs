using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PxKeystrokesUi
{
    public enum MouseEventType
    {
        MOUSEMOVE,
        LBUTTONDOWN,
        LBUTTONUP,
        LBUTTONDBLCLK,
        RBUTTONDOWN,
        RBUTTONUP,
        RBUTTONDBLCLK,
        MBUTTONDOWN,
        MBUTTONUP,
        MBUTTONDBLCLK,
        MOUSEWHEEL1,
        XBUTTONDOWN,
        XBUTTONUP,
        XBUTTONDBLCLK,
        MOUSEHWHEEL2
    }

    public enum MouseButton
    {
        LButton,
        RButton,
        MButton,
        XButton,
        None
    }

    public enum MouseAction
    {
        Up,
        Down,
        DblClk,
        Move,
        Wheel
    }

    public class MouseRawEventArgs
    {
        public NativeMethodsMouse.MSLLHOOKSTRUCT Msllhookstruct;
        public MouseEventType Event;
        public MouseButton Button;
        public MouseAction Action;
        public int wheelDelta = 0;

        public MouseRawEventArgs(NativeMethodsMouse.MSLLHOOKSTRUCT msllhookstruct)
        {
            this.Msllhookstruct = msllhookstruct;
        }

        public Point Position
        {
            get { return new Point(Msllhookstruct.pt.X, Msllhookstruct.pt.Y); }
        }

        public void ParseWparam(UIntPtr wParam)
        {
            switch((int) wParam)
            {
            case NativeMethodsMouse.WM_MOUSEMOVE:
                Event = MouseEventType.MOUSEMOVE;
                Button = MouseButton.None;
                Action = MouseAction.Move;
                break;
            case NativeMethodsMouse.WM_LBUTTONDOWN:
                Event = MouseEventType.LBUTTONDOWN;
                Button = MouseButton.LButton;
                Action = MouseAction.Down;
                break;
            case NativeMethodsMouse.WM_LBUTTONUP:
                Event = MouseEventType.LBUTTONUP;
                Button = MouseButton.LButton;
                Action = MouseAction.Up;
                break;
            case NativeMethodsMouse.WM_LBUTTONDBLCLK:
                Event = MouseEventType.LBUTTONDBLCLK;
                Button = MouseButton.LButton;
                Action = MouseAction.DblClk;
                break;
            case NativeMethodsMouse.WM_RBUTTONDOWN:
                Event = MouseEventType.RBUTTONDOWN;
                Button = MouseButton.RButton;
                Action = MouseAction.Down;
                break;
            case NativeMethodsMouse.WM_RBUTTONUP:
                Event = MouseEventType.RBUTTONUP;
                Button = MouseButton.RButton;
                Action = MouseAction.Up;
                break;
            case NativeMethodsMouse.WM_RBUTTONDBLCLK:
                Event = MouseEventType.RBUTTONDBLCLK;
                Button = MouseButton.RButton;
                Action = MouseAction.DblClk;
                break;
            case NativeMethodsMouse.WM_MBUTTONDOWN:
                Event = MouseEventType.MBUTTONDOWN;
                Button = MouseButton.MButton;
                Action = MouseAction.Down;
                break;
            case NativeMethodsMouse.WM_MBUTTONUP:
                Event = MouseEventType.MBUTTONUP;
                Button = MouseButton.MButton;
                Action = MouseAction.Up;
                break;
            case NativeMethodsMouse.WM_MBUTTONDBLCLK:
                Event = MouseEventType.MBUTTONDBLCLK;
                Button = MouseButton.MButton;
                Action = MouseAction.DblClk;
                break;
            case NativeMethodsMouse.WM_XBUTTONDOWN:
                Event = MouseEventType.XBUTTONDOWN;
                Button = MouseButton.XButton;
                Action = MouseAction.Down;
                break;
            case NativeMethodsMouse.WM_XBUTTONUP:
                Event = MouseEventType.XBUTTONUP;
                Button = MouseButton.XButton;
                Action = MouseAction.Up;
                break;
            case NativeMethodsMouse.WM_XBUTTONDBLCLK:
                Event = MouseEventType.XBUTTONDBLCLK;
                Button = MouseButton.XButton;
                Action = MouseAction.DblClk;
                break;
            case NativeMethodsMouse.WM_MOUSEWHEEL1:
                Event = MouseEventType.MOUSEWHEEL1;
                Button = MouseButton.None;
                Action = MouseAction.Wheel;
                    
                unchecked {
                    wheelDelta = BitConverter.ToInt16(BitConverter.GetBytes(Msllhookstruct.mouseData), 2);
                }
                break;
            case NativeMethodsMouse.WM_MOUSEHWHEEL2:
                Event = MouseEventType.MOUSEHWHEEL2;
                Button = MouseButton.None;
                Action = MouseAction.Wheel;
                unchecked {
                    wheelDelta = BitConverter.ToInt16(BitConverter.GetBytes(Msllhookstruct.mouseData), 2);
                }
                break;
            default:
                Log.e("ME", "Unknown Mouse Event: " + wParam.ToString());
                break;
            }
        }
    }

    public delegate void MouseRawEventHandler(MouseRawEventArgs raw_e);

    public interface IMouseRawEventProvider : IDisposable
    {
        event MouseRawEventHandler MouseEvent;
    }
}
