using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PxKeystrokesUi
{
    /// KEYBOARD.CS
    /// (c) 2006 by Emma Burrows
    /// This file contains the following items:
    ///  - KeyboardHook: class to enable low-level keyboard hook using
    ///    the Windows API.
    ///  - KeyboardHookEventHandler: delegate to handle the KeyIntercepted
    ///    event raised by the KeyboardHook class.
    ///  - KeyboardHookEventArgs: EventArgs class to contain the information
    ///    returned by the KeyIntercepted event.
    ///    
    /// Change history:
    /// 17/06/06: 1.0 - First version.
    /// 18/06/06: 1.1 - Modified proc assignment in constructor to make class backward 
    ///                 compatible with 2003.
    /// 10/07/06: 1.2 - Added support for modifier keys:
    ///                 -Changed filter in HookCallback to WM_KEYUP instead of WM_KEYDOWN
    ///                 -Imported GetKeyState from user32.dll
    ///                 -Moved native DLL imports to a separate internal class as this 
    ///                  is a Good Idea according to Microsoft's guidelines
    /// 13/02/07: 1.3 - Improved modifier key support:
    ///                 -Added CheckModifiers() method
    ///                 -Deleted LoWord/HiWord methods as they weren't necessary
    ///                 -Implemented Barry Dorman's suggestion to AND GetKeyState
    ///                  values with 0x8000 to get their result
    /// 23/03/07: 1.4 - Fixed bug which made the Alt key appear stuck
    ///                 - Changed the line
    ///                     if (nCode >= 0 && (wParam == (IntPtr)WM_KEYUP || wParam == (IntPtr)WM_SYSKEYUP))
    ///                   to
    ///                     if (nCode >= 0)
    ///                     {
    ///                        if (wParam == (IntPtr)WM_KEYUP || wParam == (IntPtr)WM_SYSKEYUP)
    ///                        ...
    ///                   Many thanks to "Scottie Numbnuts" for the solution.


    using System;
    using System.Diagnostics;
    using System.Windows.Forms;
    using System.Runtime.InteropServices;
    using System.Text;

    /// <summary>
    /// Low-level keyboard intercept class to trap and suppress system keys.
    /// </summary>
    public class KeyboardHook2 
    {
        public const string CTRL = "Ctrl";
        public const string ALT = "Alt";
        public const string SHIFT = "⇧";
        public const string CAPS = "⇪";

        //Keyboard API constants
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYUP = 0x0101;
        private const int WM_SYSKEYUP = 0x0105;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_SYSKEYDOWN = 0x0104;

        //Modifier key constants
        private const int VK_SHIFT = 0x10;
        private const int VK_CONTROL = 0x11;
        private const int VK_MENU = 0x12;
        private const int VK_CAPITAL = 0x14;

        /*


        #region Hook Callback Method
        /// <summary>
        /// Processes the key event captured by the hook.
        /// </summary>
        private IntPtr HookCallback(
            int nCode, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam)
        {
            List<string> mods;

            //Filter wParam for KeyDown events only
            if (nCode >= 0)
            {
                if (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN)
                {
                    // Check for modifier keys, but only if the key being
                    // currently processed isn't a modifier key (in other
                    // words, CheckModifiers will only run if Ctrl, Shift,
                    // CapsLock or Alt are active at the same time as
                    // another key)
                    if (!(lParam.vkCode >= 160 && lParam.vkCode <= 164) && !(lParam.vkCode == 20))
                    {
                        // vkCode != 20,160..164 -> nomods
                        mods = CheckModifiers();
                        OnKeyIntercepted(new KeyboardHookEventArgs(lParam.vkCode, mods));
                    }
                    else
                    {
                        // mod key down event
                        mods = CheckModifiers();
                        switch (lParam.vkCode) // add current modifier because it is not yet in mods
                        {
                            case 20:
                                mods.Add(CAPS);
                                break;
                            case 160:
                                mods.Add(SHIFT);
                                break;
                            case 161:
                                mods.Add(SHIFT); // right shift
                                break;
                            case 162:
                                mods.Add(CTRL);
                                break;
                            case 163:
                                mods.Add(CTRL); // right ctrl
                                break;
                            case 164:
                                mods.Add(ALT);
                                break;

                        }
                        OnModifierIntercepted(new KeyboardHookEventArgs(0, mods));
                    }

                } 
                else if (wParam == (IntPtr)WM_KEYUP || wParam == (IntPtr)WM_SYSKEYUP)
                {
                    if (!(lParam.vkCode >= 160 && lParam.vkCode <= 164) && !(lParam.vkCode == 20))
                    {
                    }
                    else
                    {
                        // mod key up event
                        mods = CheckModifiers();
                        switch (lParam.vkCode)
                        {
                            case 20:
                                mods.Remove(CAPS);
                                break;
                            case 160:
                                mods.Remove(SHIFT);
                                break;
                            case 161:
                                mods.Remove(SHIFT); // right shift
                                break;
                            case 162:
                                mods.Remove(CTRL);
                                break;
                            case 163:
                                mods.Remove(CTRL); // right ctrl
                                break;
                            case 164:
                                mods.Remove(ALT);
                                break;

                        }
                        OnModifierIntercepted(new KeyboardHookEventArgs(0, mods));
                    }
                }
            }
            //Pass key to next application
            return NativeMethods.CallNextHookEx(hookID, nCode, wParam, ref lParam);

        }
        #endregion

        #region Event Handling
        /// <summary>
        /// Raises the KeyIntercepted event.
        /// </summary>
        /// <param name="e">An instance of KeyboardHookEventArgs</param>
        public void OnKeyIntercepted(KeyboardHookEventArgs e)
        {
            if (KeyIntercepted != null)
                KeyIntercepted(e);
        }

        /// <summary>
        /// Raises the KeyIntercepted event.
        /// </summary>
        /// <param name="e">An instance of KeyboardHookEventArgs</param>
        public void OnModifierIntercepted(KeyboardHookEventArgs e)
        {
            if (ModifierIntercepted != null)
                ModifierIntercepted(e);
        }

        /// <summary>
        /// Delegate for KeyboardHook event handling.
        /// </summary>
        /// <param name="e">An instance of InterceptKeysEventArgs.</param>
        public delegate void KeyboardHookEventHandler(KeyboardHookEventArgs e);

        /// <summary>
        /// Event arguments for the KeyboardHook class's KeyIntercepted event.
        /// </summary>
        public class KeyboardHookEventArgs : System.EventArgs
        {

            private string keyName;
            private int keyCode;
            private List<string> mods;

            /// <summary>
            /// The name of the key that was pressed.
            /// </summary>
            public string KeyName
            {
                get { return keyName; }
            }

            /// <summary>
            /// The virtual key code of the key that was pressed.
            /// </summary>
            public int KeyCode
            {
                get { return keyCode; }
            }

            public List<string> Modifiers
            {
                get { return mods; }
            }

            public bool Shift
            {
                get { return mods.Exists(s => s == KeyboardHook.SHIFT); }
            }

            public bool Caps
            {
                get { return mods.Exists(s => s == KeyboardHook.CAPS); }
            }

            public bool Ctrl
            {
                get { return mods.Exists(s => s == KeyboardHook.CTRL); }
            }

            public bool Alt
            {
                get { return mods.Exists(s => s == KeyboardHook.ALT); }
            }

            public bool OnlyShiftOrCaps
            {
                get { return (Shift || Caps) && !Ctrl && !Alt; }
            }

            public bool NoModifiers
            {
                get { return mods.Count == 0; }
            }

            public KeyboardHookEventArgs(int evtKeyCode, List<string> mods)
            {
                keyName = ((Keys)evtKeyCode).ToString();
                keyCode = evtKeyCode;
                this.mods = mods;
            }

        }

        #endregion

        #region IDisposable Members
        /// <summary>
        /// Releases the keyboard hook.
        /// </summary>
        public void Dispose()
        {
            NativeMethods.UnhookWindowsHookEx(hookID);
        }
        #endregion

        #region Native methods

        [ComVisibleAttribute(false),
         System.Security.SuppressUnmanagedCodeSecurity()]
        internal class NativeMethods
        {
            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern IntPtr GetModuleHandle(string lpModuleName);

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern IntPtr SetWindowsHookEx(int idHook,
                HookHandlerDelegate lpfn, IntPtr hMod, uint dwThreadId);

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool UnhookWindowsHookEx(IntPtr hhk);

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
                IntPtr wParam, ref KBDLLHOOKSTRUCT lParam);

            [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
            public static extern short GetKeyState(int keyCode);

        }


        #endregion

        /*
         *         void k_KeyIntercepted(KeyboardHook.KeyboardHookEventArgs e)
        {
            string k = e.KeyName;
            switch (e.KeyCode)
            {
                case 13:
                    k = "⏎";
                    break;
                case 9:
                    k = "↹";
                    break;
                case 8:
                    k = "⌫";
                    break;
            }

            // A->a if no shift nor caps or both
            if ( (!e.Shift && !e.Caps) || (e.Shift && e.Caps) )
            {
                k = k.ToLower();
            }

            // remove Shift/Caps if no other modifier is pressed
            if (e.OnlyShiftOrCaps)
            {
                e.Modifiers.Remove(KeyboardHook.SHIFT);
                e.Modifiers.Remove(KeyboardHook.CAPS);
            }
            
            if (e.NoModifiers)
            {
                if(recent_is_only_text) {
                    // add new text to recent
                    string last = keystroke_history[keystroke_history.Count - 1];
                    if (last.Length < MAXLEN)
                    {
                        keystroke_history.RemoveAt(keystroke_history.Count - 1);
                        k = last + k;
                    }
                }
                recent_is_only_text = true;
                if (k == "⏎") recent_is_only_text = false;
            }
            else
            {
                recent_is_only_text = false;
            }

            e.Modifiers.Add(k);
            string nt = string.Join(" + ", e.Modifiers);
            keystroke_history.Add(nt);
            setLabel();
        }
         * */
    }

}
