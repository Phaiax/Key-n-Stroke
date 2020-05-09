using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PxKeystrokesWPF
{
    public class NativeMethodsWindow
    {
        public const UInt32 S_OK = 0x00000000;

        /// <summary>
        /// Sets the Topmost property for the window Handle
        /// </summary>
        /// <param name="Handle">The Forms myform.Handle</param>
        public static void SetWindowTopMost(IntPtr Handle) {
            SetWindowPos(Handle, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
        }

        /// <summary>
        /// Sets the Topmost property for the window Handle
        /// </summary>
        /// <param name="Handle">The Forms myform.Handle</param>
        public static void SetWindowPosition(IntPtr Handle, int x, int y)
        {
            SetWindowPos(Handle, HWND_TOPMOST, x, y, 0, 0, SWP_NOSIZE | SWP_NOOWNERZORDER);
        }

        public static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        public static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
        public static readonly IntPtr HWND_TOP = new IntPtr(0);
        public static readonly IntPtr HWND_BOTTOM = new IntPtr(1);
        public const UInt32 SWP_NOSIZE = 0x0001;
        public const UInt32 SWP_NOMOVE = 0x0002;
        public const UInt32 SWP_NOACTIVATE = 0x0010;
        public const UInt32 SWP_NOOWNERZORDER = 0x0200;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        public enum MonitorOptions : uint
        {
            MONITOR_DEFAULTTONULL = 0x00000000,
            MONITOR_DEFAULTTOPRIMARY = 0x00000001,
            MONITOR_DEFAULTTONEAREST = 0x00000002
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr MonitorFromPoint(NativeMethodsMouse.POINT pt, MonitorOptions dwFlags);

        public enum DpiType : uint
        {
            MDT_EFFECTIVE_DPI = 0,
            MDT_ANGULAR_DPI = 1,
            MDT_RAW_DPI = 2,
            MDT_DEFAULT = 3
        }

        [DllImport("Shcore.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern uint GetDpiForMonitor(IntPtr hmonitor, DpiType dpiType, ref uint dpiX, ref uint dpiY);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool GetCursorPos(ref NativeMethodsMouse.POINT lpPoint);

        public enum ProcessDpiAwareness : uint
        {
            PROCESS_DPI_UNAWARE = 0,
            PROCESS_SYSTEM_DPI_AWARE = 1,
            PROCESS_PER_MONITOR_DPI_AWARE = 2
        };

        [DllImport("Shcore.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern uint GetProcessDpiAwareness(IntPtr hprocess, ref ProcessDpiAwareness value);

        /*#define DPI_AWARENESS_CONTEXT_UNAWARE              ((DPI_AWARENESS_CONTEXT)-1)
        #define DPI_AWARENESS_CONTEXT_SYSTEM_AWARE         ((DPI_AWARENESS_CONTEXT)-2)
        #define DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE    ((DPI_AWARENESS_CONTEXT)-3)
        #define DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2 ((DPI_AWARENESS_CONTEXT)-4)
        #define DPI_AWARENESS_CONTEXT_UNAWARE_GDISCALED    ((DPI_AWARENESS_CONTEXT)-5)*/

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr GetThreadDpiAwarenessContext();

        public enum DpiAwareness : uint
        {
            DPI_AWARENESS_INVALID,
            DPI_AWARENESS_UNAWARE,
            DPI_AWARENESS_SYSTEM_AWARE,
            DPI_AWARENESS_PER_MONITOR_AWARE
        };

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern DpiAwareness GetAwarenessFromDpiAwarenessContext(IntPtr hDpiAwarenessContext);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern uint GetDpiFromDpiAwarenessContext(IntPtr hDpiAwarenessContext);

        

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, EnumMonitorsDelegate lpfnEnum, IntPtr dwData);


        delegate bool EnumMonitorsDelegate(IntPtr hMonitor, IntPtr hdcMonitor, ref Rect lprcMonitor, IntPtr dwData);

        [StructLayout(LayoutKind.Sequential)]
        public struct Rect
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        static public List<uint> GetAllUsedDpis()
        {
            List<uint> dpis = new List<uint>();

            EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero,
                delegate (IntPtr hMonitor, IntPtr hdcMonitor, ref Rect lprcMonitor, IntPtr dwData)
                {
                    uint adpiX = 0, adpiY = 0;
                    uint result = GetDpiForMonitor(hMonitor, DpiType.MDT_ANGULAR_DPI, ref adpiX, ref adpiY);
                    if (result == S_OK)
                    {
                        dpis.Add(adpiX);
                    }
                    return true; // continue enumeration
                }, IntPtr.Zero);
            return dpis;
        }

        static public void PrintDpiAwarenessInfo()
        {
            NativeMethodsMouse.POINT cursorPosition = new NativeMethodsMouse.POINT(0, 0);
            GetCursorPos(ref cursorPosition);
            IntPtr monitor = MonitorFromPoint(cursorPosition, MonitorOptions.MONITOR_DEFAULTTONEAREST);

            // Angular DPI seems to work best
            uint edpiX = 0, edpiY = 0;
            NativeMethodsWindow.GetDpiForMonitor(monitor, NativeMethodsWindow.DpiType.MDT_EFFECTIVE_DPI, ref edpiX, ref edpiY);
            uint adpiX = 0, adpiY = 0;
            NativeMethodsWindow.GetDpiForMonitor(monitor, NativeMethodsWindow.DpiType.MDT_ANGULAR_DPI, ref adpiX, ref adpiY);
            uint rdpiX = 0, rdpiY = 0;
            NativeMethodsWindow.GetDpiForMonitor(monitor, NativeMethodsWindow.DpiType.MDT_RAW_DPI, ref rdpiX, ref rdpiY);


            ProcessDpiAwareness dpiawareness = 0;
            GetProcessDpiAwareness(IntPtr.Zero, ref dpiawareness);

            IntPtr hContext = GetThreadDpiAwarenessContext();
            DpiAwareness threaddpiawareness = GetAwarenessFromDpiAwarenessContext(hContext);
            uint threadDpi = GetDpiFromDpiAwarenessContext(hContext);
            
            Log.e("DPI", $"DPI: {edpiX},{edpiY}/{adpiX},{adpiY}/{rdpiX},{rdpiY}, Monitor: {(int)monitor}, Awareness: {dpiawareness}/{threaddpiawareness}/{threadDpi}");
            
        }
    }
}
