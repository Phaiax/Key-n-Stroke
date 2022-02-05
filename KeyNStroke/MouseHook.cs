using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeyNStroke;

namespace KeyNStroke
{

    /// <summary>
    /// Low level mouse event interception class
    /// </summary>
    public class MouseHook : IDisposable, IMouseRawEventProvider
    {

        #region Initializion

        int doubleClickTime;

        /// <summary>
        /// Set up the mouse hook
        /// </summary>
        public MouseHook()
        {
            doubleClickTime = NativeMethodsMouse.GetDoubleClickTime();
            Log.e("ME", "doubleClickTime " + doubleClickTime.ToString());
            RegisterMouseHook();
        }

        #endregion

        #region Hook Add/Remove


        //Variables used in the call to SetWindowsHookEx
        private NativeMethodsMouse.HookHandlerDelegate windowsHookExProc;
        private IntPtr windowsHookExID = IntPtr.Zero;

        //Variables used in the call to SetWinEventHook
        private NativeMethodsEvents.WinEventDelegate setWinEventHookProc;
        private IntPtr setWinEventHookID = IntPtr.Zero;


        /// <summary>
        /// Registers the function WindowsHookExCallback for the global mouse events winapi. 
        /// Also registers a WinEventHook to check if the cursor is hidden.
        /// </summary>
        private void RegisterMouseHook()
        {
            // Register WindowsHookExCallback
            if (windowsHookExID == IntPtr.Zero) {
                windowsHookExProc = new NativeMethodsMouse.HookHandlerDelegate(WindowsHookExCallback);
                using (Process curProcess = Process.GetCurrentProcess())
                {
                    using (ProcessModule curModule = curProcess.MainModule)
                    {
                        windowsHookExID = NativeMethodsMouse.SetWindowsHookEx(NativeMethodsMouse.WH_MOUSE_LL, windowsHookExProc,
                            NativeMethodsKeyboard.GetModuleHandle(curModule.ModuleName), 0);
                    }
                }
            }

            // Register WinEventCallback
            // https://devblogs.microsoft.com/oldnewthing/20151116-00/?p=92091
            if (setWinEventHookID == IntPtr.Zero) {
                setWinEventHookProc = new NativeMethodsEvents.WinEventDelegate(WinEventCallback);
                setWinEventHookID = NativeMethodsEvents.SetWinEventHook(
                    NativeMethodsEvents.WinEvents.EVENT_OBJECT_SHOW,
                    NativeMethodsEvents.WinEvents.EVENT_OBJECT_HIDE,
                    IntPtr.Zero,
                    setWinEventHookProc,
                    0, // ProcessId = 0 and ThreadId = 0 -> global events
                    0,
                    NativeMethodsEvents.WinEventFlags.WINEVENT_SKIPOWNPROCESS
                    );
            }
        }
        
        /// <summary>
        /// Unregisters the winapi hook for global mouse events
        /// </summary>
        private void UnregisterMouseHook()
        {
            if (windowsHookExID != IntPtr.Zero)
            {
                NativeMethodsMouse.UnhookWindowsHookEx(windowsHookExID);
                windowsHookExID = IntPtr.Zero;
            }
            if (setWinEventHookID != IntPtr.Zero)
            {
                NativeMethodsEvents.UnhookWinEvent(setWinEventHookID);
                setWinEventHookID = IntPtr.Zero;
            }
        }

        #endregion


        #region Event Handling

        MouseRawEventArgs lastDownEvent;

        /// <summary>
        /// Processes the key event captured by the hook.
        /// </summary>
        private IntPtr WindowsHookExCallback(int nCode, 
                                    UIntPtr wParam,
                                    ref NativeMethodsMouse.MSLLHOOKSTRUCT lParam)
        {
            if (nCode == NativeMethodsMouse.HC_ACTION)
            {
                MouseRawEventArgs args = new MouseRawEventArgs(lParam);
                args.ParseWparam(wParam);
                CheckDoubleClick(args);

                Log.e("ME", String.Format("MOUSE: Button:{0} Action:{1} Orig:{2}",
                    args.Button.ToString(), args.Action.ToString(),
                    args.Event.ToString()));

                OnMouseEvent(args);
            }
            return NativeMethodsMouse.CallNextHookEx(windowsHookExID, nCode, wParam, ref lParam);
        }

        private void CheckDoubleClick(MouseRawEventArgs args)
        {
            if (lastDownEvent != null && args.Action == MouseAction.Down)
            {
                if (args.Button == lastDownEvent.Button
                    && args.Msllhookstruct.time <= lastDownEvent.Msllhookstruct.time + doubleClickTime
                    && args.Msllhookstruct.pt.Equals(lastDownEvent.Msllhookstruct.pt))
                {
                    args.Action = MouseAction.DblClk;
                    Log.e("ME", "DBLCLK");
                }
            }

            if (args.Action == MouseAction.Down)
                lastDownEvent = args;
        }

        private void WinEventCallback(IntPtr hWinEventHook, NativeMethodsEvents.WinEvents eventType, IntPtr hwnd, uint idObject, uint idChild, uint dwEventThread, uint dwmsEventTime)
        {
            if (hwnd == IntPtr.Zero &&
                idObject == (uint)NativeMethodsEvents.ObjIds.OBJID_CURSOR &&
                idChild == NativeMethodsEvents.CHILDID_SELF)
            {
                switch (eventType)
                {
                    case NativeMethodsEvents.WinEvents.EVENT_OBJECT_HIDE:
                        OnCursorEvent(false);
                        break;
                    case NativeMethodsEvents.WinEvents.EVENT_OBJECT_SHOW:
                        OnCursorEvent(true);
                        break;
                }
            }
        }

        #endregion

        #region Event Forwarding

        /// <summary>
        /// Fires if mouse is moved or clicked.
        /// </summary>
        public event MouseRawEventHandler MouseEvent;
        
        /// <summary>
        /// Raises the MouseEvent event.
        /// </summary>
        /// <param name="e">An instance of MouseRawEventArgs</param>
        public void OnMouseEvent(MouseRawEventArgs e)
        {
            if (MouseEvent != null)
                MouseEvent(e);
        }

        /// <summary>
        /// Fires if cursor is shown or hidden.
        /// </summary>
        public event CursorEventHandler CursorEvent;

        /// <summary>
        /// Raises the CursorEvent event.
        /// </summary>
        /// <param name="visible">True if the cursor is visible</param>
        public void OnCursorEvent(bool visible)
        {
            if (CursorEvent != null)
                CursorEvent(visible);
        }
        #endregion


        #region Finalizing and Disposing

        ~MouseHook()
        {
            Log.e("HOOK", "~MouseHook");
            UnregisterMouseHook();
        }

        /// <summary>
        /// Releases the mouse hook.
        /// </summary>
        public void Dispose()
        {
            Log.e("HOOK", "Dispose MouseHook");
            UnregisterMouseHook();
        }
        #endregion
    }
}
