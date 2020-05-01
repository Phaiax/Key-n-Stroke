using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PxKeystrokesUi
{
    [ComVisibleAttribute(false),
    System.Security.SuppressUnmanagedCodeSecurity()]
    public class NativeMethodsKeyboard
    {
        // Structure returned by the hook whenever a key is pressed
        [StructLayout(LayoutKind.Sequential)]
        public struct KBDLLHOOKSTRUCT
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }

        public delegate IntPtr HookHandlerDelegate(int nCode, 
                                                     IntPtr wParam, 
                                                     ref KBDLLHOOKSTRUCT lParam);


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


        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int GetKeyNameText(int lParam, [MarshalAs(UnmanagedType.LPTStr)] StringBuilder lpString, int nSize);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern uint MapVirtualKey(uint uCode, uint uMapType);
        public const uint MAPVK_VK_TO_CHAR = 2;

        /// <summary>
        /// The ToAscii function translates the specified virtual-key code and keyboard 
        /// state to the corresponding character or characters. The function translates the code 
        /// using the input language and physical keyboard layout identified by the keyboard layout handle.
        /// </summary>
        /// <param name="uVirtKey">
        /// [in] Specifies the virtual-key code to be translated. 
        /// </param>
        /// <param name="uScanCode">
        /// [in] Specifies the hardware scan code of the key to be translated. 
        /// The high-order bit of this value is set if the key is up (not pressed). 
        /// </param>
        /// <param name="lpbKeyState">
        /// [in] Pointer to a 256-byte array that contains the current keyboard state. 
        /// Each element (byte) in the array contains the state of one key. 
        /// If the high-order bit of a byte is set, the key is down (pressed). 
        /// The low bit, if set, indicates that the key is toggled on. In this function, 
        /// only the toggle bit of the CAPS LOCK key is relevant. The toggle state 
        /// of the NUM LOCK and SCROLL LOCK keys is ignored.
        /// </param>
        /// <param name="lpwTransKey">
        /// [out] Pointer to the buffer that receives the translated character or characters. 
        /// </param>
        /// <param name="fuState">
        /// [in] Specifies whether a menu is active. This parameter must be 1 if a menu is active, or 0 otherwise. 
        /// </param>
        /// <returns>
        /// If the specified key is a dead key, the return value is negative. Otherwise, it is one of the following values. 
        /// Value Meaning 
        /// 0 The specified virtual key has no translation for the current state of the keyboard. 
        /// 1 One character was copied to the buffer. 
        /// 2 Two characters were copied to the buffer. This usually happens when a dead-key character 
        /// (accent or diacritic) stored in the keyboard layout cannot be composed with the specified 
        /// virtual key to form a single character. 
        /// </returns>
        /// <remarks>
        /// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/userinput/keyboardinput/keyboardinputreference/keyboardinputfunctions/toascii.asp
        /// </remarks>
        [DllImport("user32")]
        public static extern int ToAscii(
            int uVirtKey,
            int uScanCode,
            byte[] lpbKeyState,
            byte[] lpwTransKey,
            int fuState);

    /*            int WINAPI ToAscii(
  _In_      UINT uVirtKey,
  _In_      UINT uScanCode,
  _In_opt_  const BYTE *lpKeyState,
  _Out_     LPWORD lpChar,
  _In_      UINT uFlags
);*/
        [DllImport("user32", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int ToUnicode(
            int uVirtKey,
            int uScanCode,
            byte[] lpbKeyState,
            [MarshalAs(UnmanagedType.LPWStr)] StringBuilder pwszBuff,
            int cchBuff,
            uint wFlags);
        /*
        int WINAPI ToUnicode(
  _In_      UINT wVirtKey,
  _In_      UINT wScanCode,
  _In_opt_  const BYTE *lpKeyState,
  _Out_     LPWSTR pwszBuff,
  _In_      int cchBuff,
  _In_      UINT wFlags
);*/


        /// <summary>
        /// The GetKeyboardState function copies the status of the 256 virtual keys to the 
        /// specified buffer. 
        /// </summary>
        /// <param name="pbKeyState">
        /// [in] Pointer to a 256-byte array that contains keyboard key states. 
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is nonzero.
        /// If the function fails, the return value is zero. To get extended error information, call GetLastError. 
        /// </returns>
        /// <remarks>
        /// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/userinput/keyboardinput/keyboardinputreference/keyboardinputfunctions/toascii.asp
        /// </remarks>
        [DllImport("user32")]
        public static extern int GetKeyboardState(byte[] pbKeyState);

    }
}
