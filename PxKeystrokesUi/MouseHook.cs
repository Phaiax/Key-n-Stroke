using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PxKeystrokesUi
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
        private NativeMethodsMouse.HookHandlerDelegate proc;
        private IntPtr hookID = IntPtr.Zero;

        /// <summary>
        /// Registers the function HookCallback for the global mouse events winapi 
        /// </summary>
        private void RegisterMouseHook()
        {
            if (hookID != IntPtr.Zero)
                return;

            proc = new NativeMethodsMouse.HookHandlerDelegate(HookCallback);
            using (Process curProcess = Process.GetCurrentProcess())
            {
                using (ProcessModule curModule = curProcess.MainModule)
                {
                    hookID = NativeMethodsMouse.SetWindowsHookEx(NativeMethodsMouse.WH_MOUSE_LL, proc,
                        NativeMethodsKeyboard.GetModuleHandle(curModule.ModuleName), 0);
                }
            }
        }
        
        /// <summary>
        /// Unregisters the winapi hook for global mouse events
        /// </summary>
        private void UnregisterMouseHook()
        {
            if (hookID == IntPtr.Zero)
                return;
            NativeMethodsMouse.UnhookWindowsHookEx(hookID);
            hookID = IntPtr.Zero;
        }

        #endregion


        #region Event Handling

        MouseRawEventArgs lastDownEvent;

        /// <summary>
        /// Processes the key event captured by the hook.
        /// </summary>
        private IntPtr HookCallback(int nCode, 
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
            return NativeMethodsMouse.CallNextHookEx(hookID, nCode, wParam, ref lParam);
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
