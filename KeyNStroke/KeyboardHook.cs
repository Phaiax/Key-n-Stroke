using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Input;
using KeyNStroke;

namespace KeyNStroke
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

        private const int
        VK_SHIFT = 0x10,
        VK_CONTROL = 0x11,
        VK_MENU = 0x12,
        VK_CAPITAL = 0x14,
        VK_LWIN = 0x5B,
        VK_RWIN = 0x5C,
        VK_NUMLOCK = 0x90,
        VK_SCROLL = 0x91,
        VK_LSHIFT = 0xA0,
        VK_RSHIFT = 0xA1,
        VK_LCONTROL = 0xA2,
        VK_RCONTROL = 0xA3,
        VK_LMENU = 0xA4,
        VK_RMENU = 0xA5;

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
                //NativeMethodsKeyboard.GetKeyboardState(e.keyState); probability of dataraces

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
                    Log.e("KE", $"EVENT scanCode={lParam.scanCode} vkCode={e.vkCode} method={e.Method} uppercase={e.Uppercase} ");
                    OnKeyEvent(e);
                }

                /*System.Diagnostics.Debug.WriteLine(
                    String.Format("Key: sc {0} vk {1} ext {2} fl {3}, {4}", lParam.scanCode,
                        lParam.vkCode, lParam.dwExtraInfo, lParam.flags, e.Method));
                */
                if (e.preventDefault)
                {
                    return IntPtr.Add(IntPtr.Zero, 1);
                } else
                {
                    return NativeMethodsKeyboard.CallNextHookEx(hookID, nCode, wParam, ref lParam);
                }
            }
            else
            {
                //Pass key to next application
                return NativeMethodsKeyboard.CallNextHookEx(hookID, nCode, wParam, ref lParam);
            }
        }

        /// <summary>
        /// Checks whether Alt, Shift, Control or CapsLock
        /// is enabled at the same time as another key.
        /// Modify the relevant sections and return type 
        /// depending on what you want to do with modifier keys.
        /// </summary>
        private void CheckModifiers(KeyboardRawEventArgs e)
        {
            e.Shift = CheckModifierDown(VK_SHIFT);
            e.Ctrl = CheckModifierDown(VK_CONTROL);
            e.Alt = CheckModifierDown(VK_MENU);
            e.Caps = CheckModifierToggled(VK_CAPITAL);
            e.LWin = CheckModifierDown(VK_LWIN);
            e.RWin = CheckModifierDown(VK_RWIN);
            e.Numlock = CheckModifierToggled(VK_NUMLOCK);
            e.Scrollock = CheckModifierToggled(VK_SCROLL);
            e.LShift = CheckModifierDown(VK_LSHIFT);
            e.RShift = CheckModifierDown(VK_RSHIFT);
            e.LCtrl = CheckModifierDown(VK_LCONTROL);
            e.RCtrl = CheckModifierDown(VK_RCONTROL);
            e.LAlt = CheckModifierDown(VK_LMENU);
            e.RAlt = CheckModifierDown(VK_RMENU);
        }

        /// <summary>
        /// Uses the winapi to check if the VK key modifiercode is pressed
        /// </summary>
        /// <param name="modifiercode"></param>
        /// <returns></returns>
        private bool CheckModifierDown(int modifiercode)
        {
            // SHORT NativeMethodsKeyboard.GetKeyState(int)
            // The return value specifies the status of the specified virtual key, as follows:

            // - If the high-order bit is 1, the key is down; otherwise, it is up.
            // - If the low-order bit is 1, the key is toggled. A key, such as 
            //   the CAPS LOCK key, is toggled if it is turned on. The key is 
            //   off and untoggled if the low-order bit is 0. A toggle key's 
            //   indicator light (if any) on the keyboard will be on when the 
            //   key is toggled, and off when the key is untoggled.
            return ((NativeMethodsKeyboard.GetKeyState(modifiercode) & 0x8000) != 0);
        }

        /// <summary>
        /// Uses the winapi to check weather the caps/numlock/scrolllock is activated
        /// </summary>
        /// <param name="modifiercode"></param>
        /// <returns></returns>
        private bool CheckModifierToggled(int modifiercode)
        {
            return (NativeMethodsKeyboard.GetKeyState(modifiercode) & 0x0001) != 0;
        }


        private void FixKeyStateArray(KeyboardRawEventArgs e)
        {
            Log.e("KP", "FixKeyStateArray()");
            if (e.Uppercase)
            {
                e.keyState[VK_SHIFT] = 129;
            } else
            {
                e.keyState[VK_SHIFT] = 0;
            }
            //return;
#pragma warning disable CS0162 // Unerreichbarer Code wurde entdeckt.
            e.keyState[VK_CONTROL] = (byte)(e.Ctrl ? 129 : 0);
#pragma warning restore CS0162 // Unerreichbarer Code wurde entdeckt.
            e.keyState[VK_MENU] = (byte)(e.Alt ? 129 : 0);
            e.keyState[VK_CAPITAL] = (byte)(e.Caps ? 129 : 0);
            e.keyState[VK_LWIN] = (byte)(e.LWin ? 129 : 0);
            e.keyState[VK_RWIN] = (byte)(e.RWin ? 129 : 0);
            e.keyState[VK_NUMLOCK] = (byte)(e.Numlock ? 129 : 0);
            e.keyState[VK_SCROLL] = (byte)(e.Scrollock ? 129 : 0);
            e.keyState[VK_LSHIFT] = (byte)(e.LShift ? 129 : 0);
            e.keyState[VK_RSHIFT] = (byte)(e.RShift ? 129 : 0);
            e.keyState[VK_LCONTROL] = (byte)(e.LCtrl ? 129 : 0);
            e.keyState[VK_RCONTROL] = (byte)(e.RCtrl ? 129 : 0);
            e.keyState[VK_LMENU] = (byte)(e.LAlt ? 129 : 0);
            e.keyState[VK_RMENU] = (byte)(e.RAlt ? 129 : 0);
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
