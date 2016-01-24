using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace PxKeystrokesUi
{
    /// Some Parts are from 
    /// KEYBOARD.CS (c) 2006 by Emma Burrows

    /// <summary>
    /// Low-level keyboard intercept class
    /// </summary>
    public class KeyboardHook : IDisposable, IKeyboardRawEventProvider
    {
 
        #region Initializion

        /// <summary>
        /// Sets up a keyboard hook
        /// </summary>
        public KeyboardHook()
        {
            RegisterKeyboardHook();
        }

        #endregion

        #region Hook Add/Remove

        // Keyboard Hook Identifier for WinApi
        private const int WH_KEYBOARD_LL = 13;

        //Variables used in the call to SetWindowsHookEx
        private NativeMethodsKeyboard.HookHandlerDelegate proc;
        private IntPtr hookID = IntPtr.Zero;

        /// <summary>
        /// Registers the function HookCallback for the global key events winapi 
        /// </summary>
        private void RegisterKeyboardHook()
        {
            if (hookID != IntPtr.Zero)
                return;
            proc = new NativeMethodsKeyboard.HookHandlerDelegate(HookCallback);
            using (Process curProcess = Process.GetCurrentProcess())
            {
                using (ProcessModule curModule = curProcess.MainModule)
                {
                    hookID = NativeMethodsKeyboard.SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                        NativeMethodsKeyboard.GetModuleHandle(curModule.ModuleName), 0);
                }
            }
        }

        /// <summary>
        /// Unregisters the winapi hook for global key events
        /// </summary>
        private void UnregisterKeyboardHook()
        {
            if (hookID == IntPtr.Zero)
                return;
            NativeMethodsKeyboard.UnhookWindowsHookEx(hookID);
            hookID = IntPtr.Zero;
        }

        #endregion

        #region EventHandling

        //Keyboard API constants: press or release  
        private const int WM_KEYUP = 0x0101;
        private const int WM_SYSKEYUP = 0x0105;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_SYSKEYDOWN = 0x0104;


        /// <summary>
        /// Processes the key event captured by the hook.
        /// </summary>
        private IntPtr HookCallback(int nCode, 
                                    IntPtr wParam, 
                                    ref NativeMethodsKeyboard.KBDLLHOOKSTRUCT lParam)
        {
            if (nCode >= 0)
            {
                KeyboardRawEventArgs e = new KeyboardRawEventArgs(lParam);
                e.keyState = new byte[256];
                NativeMethodsKeyboard.GetKeyboardState(e.keyState);

                if (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN)
                {
                    e.Method = KeyUpDown.Down;
                }
                else if (wParam == (IntPtr)WM_KEYUP || wParam == (IntPtr)WM_SYSKEYUP)
                {
                    e.Method = KeyUpDown.Up;
                }

                if (e.Method != KeyUpDown.Undefined)
                {
                    CheckModifiers(e);
                    FixKeyStateArray(e);
                    Log.e("KE", "EVENT " + e.Method.ToString() + " shift:" + e.Uppercase.ToString());
                    OnKeyEvent(e);
                }

                /*System.Diagnostics.Debug.WriteLine(
                    String.Format("Key: sc {0} vk {1} ext {2} fl {3}, {4}", lParam.scanCode,
                        lParam.vkCode, lParam.dwExtraInfo, lParam.flags, e.Method));
                */
            }
            //Pass key to next application
            IntPtr ret = NativeMethodsKeyboard.CallNextHookEx(hookID, nCode, wParam, ref lParam);
            return ret;
        }

        /// <summary>
        /// Checks whether Alt, Shift, Control or CapsLock
        /// is enabled at the same time as another key.
        /// Modify the relevant sections and return type 
        /// depending on what you want to do with modifier keys.
        /// </summary>
        private void CheckModifiers(KeyboardRawEventArgs e)
        {
            e.Shift = CheckModifierDown(Keys.ShiftKey);
            e.Ctrl = CheckModifierDown(Keys.ControlKey);
            e.Alt = CheckModifierDown(Keys.Menu);
            e.Caps = CheckModifierToggled(Keys.CapsLock);
            e.LWin = CheckModifierDown(Keys.LWin);
            e.RWin = CheckModifierDown(Keys.RWin);
            e.Numlock = CheckModifierToggled(Keys.NumLock);
            e.Scrollock = CheckModifierToggled(Keys.Scroll);
            e.LShift = CheckModifierDown(Keys.LShiftKey);
            e.RShift = CheckModifierDown(Keys.RShiftKey);
            e.LCtrl = CheckModifierDown(Keys.LControlKey);
            e.RCtrl = CheckModifierDown(Keys.RControlKey);
            e.LAlt = CheckModifierDown(Keys.LMenu);
            e.RAlt = CheckModifierDown(Keys.RMenu);
        }

        /// <summary>
        /// Uses the winapi to check if the VK key modifiercode is pressed
        /// </summary>
        /// <param name="modifiercode"></param>
        /// <returns></returns>
        private bool CheckModifierDown(Keys modifiercode)
        {
            // SHORT NativeMethodsKeyboard.GetKeyState(int)
            // The return value specifies the status of the specified virtual key, as follows:

            // - If the high-order bit is 1, the key is down; otherwise, it is up.
            // - If the low-order bit is 1, the key is toggled. A key, such as 
            //   the CAPS LOCK key, is toggled if it is turned on. The key is 
            //   off and untoggled if the low-order bit is 0. A toggle key's 
            //   indicator light (if any) on the keyboard will be on when the 
            //   key is toggled, and off when the key is untoggled.
            return ((NativeMethodsKeyboard.GetKeyState((int)modifiercode) & 0x8000) != 0);
        }

        /// <summary>
        /// Uses the winapi to check weather the caps/numlock/scrolllock is activated
        /// </summary>
        /// <param name="modifiercode"></param>
        /// <returns></returns>
        private bool CheckModifierToggled(Keys modifiercode)
        {
            return (NativeMethodsKeyboard.GetKeyState((int)modifiercode) & 0x0001) != 0;
        }


        private void FixKeyStateArray(KeyboardRawEventArgs e)
        {
            if (e.Uppercase)
            {
                e.keyState[(int)Keys.ShiftKey] = 129;

            }
            return;
            e.keyState[(int)Keys.LShiftKey] = (byte)(e.LShift ? 129 : 1);
            e.keyState[(int)Keys.RShiftKey] = (byte)(e.RShift ? 129 : 1);
            e.keyState[(int)Keys.CapsLock] = (byte)(e.Caps ? 129 : 1);
            e.keyState[(int)Keys.ControlKey] = (byte)(e.Ctrl ? 129 : 1);
            e.keyState[(int)Keys.Menu] = (byte)(e.Alt ? 129 : 1);
            e.keyState[(int)Keys.LWin] = (byte)(e.LWin ? 129 : 1);
            e.keyState[(int)Keys.RWin] = (byte)(e.RWin ? 129 : 1);
            e.keyState[(int)Keys.NumLock] = (byte)(e.Numlock ? 129 : 1);
            e.keyState[(int)Keys.Scroll] = (byte)(e.Scrollock ? 129 : 1);
            e.keyState[(int)Keys.LControlKey] = (byte)(e.LCtrl ? 129 : 1);
            e.keyState[(int)Keys.RControlKey] = (byte)(e.RCtrl ? 129 : 1);
            e.keyState[(int)Keys.LMenu] = (byte)(e.LAlt ? 129 : 1);
            e.keyState[(int)Keys.RMenu] = (byte)(e.RAlt ? 129 : 1);
        }

        #endregion

        #region Event Forwarding

        /// <summary>
        /// Fires if key is pressed or released.
        /// </summary>
        public event KeyboardRawEventHandler KeyEvent;
        
        /// <summary>
        /// Raises the KeyEvent event.
        /// </summary>
        /// <param name="e">An instance of KeyboardRawEvent</param>
        public void OnKeyEvent(KeyboardRawEventArgs e)
        {
            if (KeyEvent != null)
                KeyEvent(e);
        }

        #endregion

        #region Finalizing and Disposing

        ~KeyboardHook()
        {
            Log.e("HOOK", "~KeyboardHook");
            UnregisterKeyboardHook();
        }

        /// <summary>
        /// Releases the keyboard hook.
        /// </summary>
        public void Dispose()
        {
            UnregisterKeyboardHook();
        }
        #endregion


    }

}
