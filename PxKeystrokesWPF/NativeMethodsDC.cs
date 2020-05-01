using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using PxKeystrokesWPF;

namespace PxKeystrokesUi
{
    // https://docs.microsoft.com/en-us/windows/win32/winmsg/window-features?WT.mc_id=DT-MVP-5003235#layered-windows
    // https://docs.microsoft.com/en-us/windows/win32/gdi/about-device-contexts

    // GDI: Graphics Device Interface
    //      = Some API to paint on screens, printers, ...

    // DC: DeviceContext
    //     - https://docs.microsoft.com/de-de/windows/win32/gdi/about-bitmaps
    //     - Contains a set of graphic objects:
    //        - Bitmaps or Paths or Rectangles or Polygons or Text
    //          with associated Fonts, Brushes, Pens, Color Palettes (logical palettes)
    //        - Each graphic object is contained at most once, and is otherwise defaulted or null
    //        - SelectObject replaces the object of the selected type in the DC
    //              https://docs.microsoft.com/en-us/windows/win32/api/wingdi/nf-wingdi-selectobject
    //              - This function returns the previously selected object of the specified type.
    //                An application should always replace a new object with the original, 
    //                default object after it has finished drawing with the new object.
    //              - An application cannot select a single bitmap into more than one DC at a time.



    // GDI Bitmaps need to be created from .NET Bitmaps.. -> todo: cache or measure
    //      https://docs.microsoft.com/de-de/dotnet/api/system.drawing.bitmap.gethbitmap?view=netframework-4.8
    //      GDI Bitmaps consist of: 
    //          A header that describes the resolution of the device on which the rectangle of pixels was created, the dimensions of the rectangle, the size of the array of bits, and so on.
    //          A logical palette.
    //          An array of bits that defines the relationship between pixels in the bitmapped image and entries in the logical palette.


    // https://docs.microsoft.com/de-de/windows/win32/gdi/bitmaps?redirectedfrom=MSDN

    public class NativeMethodsDC
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="windowHandle">Window Handle</param>
        /// <param name="pos">Position of window</param>
        /// <param name="bitmap">Bitmap to set</param>
        /// <param name="opacity">Better performance with 1.0</param>
        public static void SetBitmapForWindow(IntPtr windowHandle, Point pos, Bitmap bitmap, float opacity)
        {
            // Does this bitmap contain an alpha channel?
            if (bitmap.PixelFormat != PixelFormat.Format32bppArgb)
            {
                throw new ApplicationException("The bitmap must be 32bpp with alpha-channel.");
            }

            // Get device contexts
            IntPtr screenDc = GetDC(IntPtr.Zero);
            IntPtr memDc = CreateCompatibleDC(screenDc);
            IntPtr hBitmap = IntPtr.Zero;
            IntPtr hOldBitmap = IntPtr.Zero;

            opacity = Math.Min(opacity, 1.0f);
            opacity = Math.Max(opacity, 0.0f);

            try
            {
                // Get handle to the new bitmap and select it into the current 
                // device context. (keep the old bitmap because the documentation
                // wants us to restore the Bitmap after drawing)
                hBitmap = bitmap.GetHbitmap(Color.FromArgb(0));
                hOldBitmap = SelectObject(memDc, hBitmap);

                // Set parameters for layered window update.
                SIZE newSize = new SIZE(bitmap.Width, bitmap.Height);
                POINT sourceLocation = new POINT(0, 0);
                POINT newLocation = new POINT(pos.X, pos.Y); // new Point(this.Left, this.Top)
                BLENDFUNCTION blend = new BLENDFUNCTION();
                blend.BlendOp = AC_SRC_OVER;
                blend.BlendFlags = 0;
                blend.SourceConstantAlpha = (byte) (int) (opacity * 255);
                blend.AlphaFormat = AC_SRC_ALPHA;

                Log.e("LAYWIN", "Draw to " + pos.ToString() + " and set size to " + bitmap.Width.ToString() + " " + bitmap.Height.ToString());

                // Update the window.
                UpdateLayeredWindow(
                    windowHandle,    // Handle to the layered window
                    screenDc,        // Handle to the screen DC
                    ref newLocation, // New screen position of the layered window
                    ref newSize,     // New size of the layered window
                    memDc,           // Handle to the layered window surface DC
                    ref sourceLocation, // Location of the layer in the DC
                    0,               // Color key of the layered window
                    ref blend,       // Transparency of the layered window
                    ULW_ALPHA        // Use blend as the blend function
                    );
            }
            finally
            {
                // Release device context.
                ReleaseDC(IntPtr.Zero, screenDc);
                if (hBitmap != IntPtr.Zero)
                {
                    SelectObject(memDc, hOldBitmap);
                    DeleteObject(hBitmap);
                }
                DeleteDC(memDc);
            }
        }


        // Source: MSDN Code Gallery
        // https://code.msdn.microsoft.com/windowsapps/CSWinFormLayeredWindow-23cdc375
        // https://stackoverflow.com/questions/33530623/c-sharp-windows-form-transparent-background-image

        const Int32 WS_EX_LAYERED = 0x80000;
        const Int32 HTCAPTION = 0x02;
        const Int32 WM_NCHITTEST = 0x84;
        const Int32 ULW_ALPHA = 0x02;
        const byte AC_SRC_OVER = 0x00;
        const byte AC_SRC_ALPHA = 0x01;

        [StructLayout(LayoutKind.Sequential)]
        struct POINT
        {
            public Int32 x;
            public Int32 y;

            public POINT(Int32 x, Int32 y)
            { this.x = x; this.y = y; }
        }

        [StructLayout(LayoutKind.Sequential)]
        struct SIZE
        {
            public Int32 cx;
            public Int32 cy;

            public SIZE(Int32 cx, Int32 cy)
            { this.cx = cx; this.cy = cy; }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct ARGB
        {
            public byte Blue;
            public byte Green;
            public byte Red;
            public byte Alpha;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct BLENDFUNCTION
        {
            public byte BlendOp;
            public byte BlendFlags;
            public byte SourceConstantAlpha;
            public byte AlphaFormat;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="hwnd">A handle to a layered window. A layered window is created by 
        /// specifying WS_EX_LAYERED when creating the window with the CreateWindowEx 
        /// function.</param>
        /// <param name="hdcDst">A handle to a DC for the screen. This handle is obtained by 
        /// specifying NULL when calling the function. It is used for palette color matching 
        /// when the window contents are updated. If hdcDst isNULL, the default palette will 
        /// be used.
        ///
        /// If hdcSrc is NULL, hdcDst must be NULL.</param>
        /// <param name="pptDst">A pointer to a structure that specifies the new screen 
        /// position of the layered window. If the current position is not changing,
        /// pptDst can be NULL.</param>
        /// <param name="psize">A pointer to a structure that specifies the new size of 
        /// the layered window. If the size of the window is not changing, psize can be 
        /// NULL. If hdcSrc is NULL, psize must be NULL.</param>
        /// <param name="hdcSrc">A handle to a DC for the surface that defines the layered
        /// window. This handle can be obtained by calling the CreateCompatibleDC function.
        /// If the shape and visual context of the window are not changing, hdcSrc can be
        /// NULL.</param>
        /// <param name="pptSrc">A pointer to a structure that specifies the location of 
        /// the layer in the device context. If hdcSrc is NULL, pptSrc should be 
        /// NULL.</param>
        /// <param name="crKey">A structure that specifies the color key to be used when
        /// composing the layered window.</param>
        /// <param name="pblend">A pointer to a structure that specifies the transparency
        /// value to be used when composing the layered window.</param>
        /// <param name="dwFlags">This parameter can be one of the following values.
        /// - ULW_ALPHA: Use pblend as the blend function. If the display mode is 256 
        ///              colors or less, the effect of this value is the same as the
        ///              effect of ULW_OPAQUE.
        /// - ULW_COLORKEY:	Use crKey as the transparency color.
        /// - ULW_OPAQUE:  Draw an opaque layered window.
        /// - ULW_EX_NORESIZE: Force the UpdateLayeredWindowIndirect function to fail
        ///                    if the current window size does not match the size 
        ///                    in the psize.
        /// If hdcSrc is NULL, dwFlags should be zero.
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is nonzero.
        ///
        /// If the function fails, the return value is zero.To get extended error 
        /// information, call GetLastError.
        /// </returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst,
            ref POINT pptDst, ref SIZE psize, IntPtr hdcSrc, ref POINT pptSrc,
            Int32 crKey, ref BLENDFUNCTION pblend, Int32 dwFlags);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern IntPtr CreateCompatibleDC(IntPtr hDC);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool DeleteDC(IntPtr hdc);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool DeleteObject(IntPtr hObject);

    }
}
