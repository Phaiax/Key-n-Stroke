using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace PxKeystrokesUi
{
    /// <summary>
    /// Each Window has some associated memory with holds flags for window behaviour
    /// The bytes that hold these informations are numbered as nIndex with the values GWL.GWL_****
    /// These Methods can be used to define a click through behaviour
    /// </summary>
    public class NativeMethodsGWL
    {
        /// <summary>
        /// Activate Click Through
        /// </summary>
        /// <param name="Handle">The Forms myform.Handle</param>
        public static void ClickThrough(IntPtr Handle)
        {
            uint current_val = GetWindowLong(Handle, GWL.GWL_EXSTYLE);
            uint changed_val = Convert.ToUInt32(current_val | (uint)WindowStyles.WS_EX_LAYERED | (uint)WindowStyles.WS_EX_TRANSPARENT);
            SetWindowLong(Handle, GWL.GWL_EXSTYLE, changed_val);
        }

        /// <summary>
        /// Deactivate Click Through
        /// </summary>
        /// <param name="Handle">The Forms myform.Handle</param>
        public static void CatchClicks(IntPtr Handle)
        {
            uint current_val = GetWindowLong(Handle, GWL.GWL_EXSTYLE);
            uint changed_val = Convert.ToUInt32(current_val & ~((uint)WindowStyles.WS_EX_LAYERED)) & ~((uint)WindowStyles.WS_EX_TRANSPARENT);
            SetWindowLong(Handle, GWL.GWL_EXSTYLE, changed_val);
        }

        public static void HideFromAltTab(IntPtr Handle)
        {
            int current_val = (int)GetWindowLong(Handle, GWL.GWL_EXSTYLE);
            uint changed_val = Convert.ToUInt32(current_val | (uint)WindowStyles.WS_EX_TOOLWINDOW);
            SetWindowLong(Handle, GWL.GWL_EXSTYLE, changed_val);
        }
 

        /// <summary>
        /// These are the possible longs which can be get and set via GetWindowLong and ... 
        /// </summary>
        public enum GWL : int
        {
            GWL_WNDPROC = (-4),
            GWL_HINSTANCE = (-6),
            GWL_HWNDPARENT = (-8),
            GWL_STYLE = (-16),
            GWL_EXSTYLE = (-20),
            GWL_USERDATA = (-21),
            GWL_ID = (-12)
        }


        /// <summary>
        /// These are the Flags for the Extended Style Bytes (nIndex=GWL_EXTSTYLE)
        /// </summary>
        [Flags]
        public enum WindowStyles : uint
        {
            //Extended Window Styles

            WS_EX_DLGMODALFRAME = 0x00000001,
            WS_EX_NOPARENTNOTIFY = 0x00000004,
            WS_EX_TOPMOST = 0x00000008,
            WS_EX_ACCEPTFILES = 0x00000010,
            WS_EX_TRANSPARENT = 0x00000020,
            WS_EX_LAYERED = 0x00080000,

            WS_EX_TOOLWINDOW = 0x00000080,
        }

        // Choose 32/64 bit variant of function

        /// <summary>
        /// returns long bytes at nIndex
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="nIndex"></param>
        /// <returns>the LONG value</returns>
        public static uint GetWindowLong(IntPtr hWnd, GWL nIndex)
        {
            if (IntPtr.Size == 4)
            {
                return (uint)GetWindowLong32(hWnd, (int)nIndex);
            }
            return (uint)GetWindowLongPtr64(hWnd, (int)nIndex);
        }

        /// <summary>
        /// sets the LONG value at nIndex to nwNewLong
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="nIndex"></param>
        /// <param name="dwNewLong"></param>
        /// <returns>dunno</returns>
        public static int SetWindowLong(IntPtr hWnd, GWL nIndex, uint dwNewLong)
        {
            if (IntPtr.Size == 4)
            {
                return SetWindowLong32(hWnd, (int)nIndex, dwNewLong);
            }
            return SetWindowLongPtr64(hWnd, (int)nIndex, dwNewLong);
        }

        // Different Functions for Win32 and 64 bit

        [DllImport("user32.dll", EntryPoint = "GetWindowLong", CharSet = CharSet.Auto)]
        private static extern int GetWindowLong32(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr", CharSet = CharSet.Auto)]
        private static extern int GetWindowLongPtr64(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong", CharSet = CharSet.Auto)]
        private static extern int SetWindowLong32(IntPtr hWnd, int nIndex, uint dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", CharSet = CharSet.Auto)]
        private static extern int SetWindowLongPtr64(IntPtr hWnd, int nIndex, uint dwNewLong);

    }
}
