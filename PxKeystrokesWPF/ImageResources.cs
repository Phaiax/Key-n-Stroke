using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using PxKeystrokesWPF;

namespace PxKeystrokesWPF
{
    public class ImageResources
    {
        static Assembly _assembly;

        private struct BitmapCollection
        {
            public Bitmap BMouse;
            public Bitmap BLeft;
            public Bitmap BRight;
            public Bitmap BMiddle;
            public Bitmap BLeftDouble;
            public Bitmap BRightDouble;
            public Bitmap BWheelUp;
            public Bitmap BWheelDown;
        }

        static Dictionary<uint, BitmapCollection> ScaledByDpi;
        static BitmapCollection Orig;

        public static void Init()
        {
            try
            {
                _assembly = Assembly.GetExecutingAssembly();

                foreach(string i in _assembly.GetManifestResourceNames())
                {
                    Log.e("RES", i);
                }
                Orig.BMouse = new Bitmap(_assembly.GetManifestResourceStream("PxKeystrokesWPF.Resources.mouse.png"));
                Orig.BLeft = new Bitmap(_assembly.GetManifestResourceStream("PxKeystrokesWPF.Resources.mouse_left.png"));
                Orig.BRight = new Bitmap(_assembly.GetManifestResourceStream("PxKeystrokesWPF.Resources.mouse_right.png"));
                Orig.BMiddle = new Bitmap(_assembly.GetManifestResourceStream("PxKeystrokesWPF.Resources.mouse_middle.png"));
                Orig.BLeftDouble = new Bitmap(_assembly.GetManifestResourceStream("PxKeystrokesWPF.Resources.mouse_left_double.png"));
                Orig.BRightDouble = new Bitmap(_assembly.GetManifestResourceStream("PxKeystrokesWPF.Resources.mouse_right_double.png"));
                Orig.BWheelUp = new Bitmap(_assembly.GetManifestResourceStream("PxKeystrokesWPF.Resources.mouse_wheel_up.png"));
                Orig.BWheelDown = new Bitmap(_assembly.GetManifestResourceStream("PxKeystrokesWPF.Resources.mouse_wheel_down.png"));

            }
            catch
            {
                Log.e("RES", "Error accessing resources!");
            }

            ApplyScalingFactor(1.0f);
        }

        static float appliedScalingFactor = -1f;

        public static void ApplyScalingFactor(float scalingfactor)
        {
            scalingfactor = Math.Min(10f, Math.Max(0.1f, scalingfactor));

            if (appliedScalingFactor != scalingfactor)
            {
                var newByDpi = new Dictionary<uint, BitmapCollection>();
                
                List<uint> dpis = NativeMethodsWindow.GetAllUsedDpis();

                foreach (uint dpi in dpis)
                {
                    newByDpi.Add(dpi, CreateScaledBitmapCollection(scalingfactor, dpi));
                }

                appliedScalingFactor = scalingfactor;
                ScaledByDpi = newByDpi;
            }
        }

        private static BitmapCollection CreateScaledBitmapCollection(float scalingFactor, uint dpi)
        {
            BitmapCollection scaled = new BitmapCollection
            {
                BMouse = Scale(scalingFactor, Orig.BMouse, dpi),
                BLeft = Scale(scalingFactor, Orig.BLeft, dpi),
                BRight = Scale(scalingFactor, Orig.BRight, dpi),
                BMiddle = Scale(scalingFactor, Orig.BMiddle, dpi),
                BLeftDouble = Scale(scalingFactor, Orig.BLeftDouble, dpi),
                BRightDouble = Scale(scalingFactor, Orig.BRightDouble, dpi),
                BWheelUp = Scale(scalingFactor, Orig.BWheelUp, dpi),
                BWheelDown = Scale(scalingFactor, Orig.BWheelDown, dpi)
            };

            return scaled;
        }

        private static Bitmap Scale(float scalingFactor, Bitmap original, uint dpi)
        {
            var dpiScale = (float)dpi / 96.0f;
            scalingFactor *= dpiScale;
            var scaledWidth = (int)(original.Width * scalingFactor);
            Log.e("DPI", $"Scale: DPI: {dpi}, dpiScaleFactor:{dpiScale}, scaledWidth:{scaledWidth}, totalScalingFactor:{scalingFactor}");
            var scaledHeight = (int)(original.Height * scalingFactor);
            var scaledBitmap = new Bitmap(scaledWidth, scaledHeight, PixelFormat.Format32bppArgb);
            // Draw original image onto new bitmap and interpolate
            Graphics graph = Graphics.FromImage(scaledBitmap);
            graph.InterpolationMode = InterpolationMode.High;
            graph.CompositingQuality = CompositingQuality.HighQuality;
            graph.SmoothingMode = SmoothingMode.AntiAlias;
            graph.DrawImage(original, new Rectangle(0, 0, scaledWidth, scaledHeight));
            return scaledBitmap;
        }

        public struct ComposeOptions
        {
            public uint dpi;
            public bool addBMouse;
            public bool addBLeft;
            public bool addBRight;
            public bool addBMiddle;
            public bool addBLeftDouble;
            public bool addBRightDouble;
            public bool addBWheelUp;
            public bool addBWheelDown;

            public static bool operator ==(ComposeOptions first, ComposeOptions second)
            {
                return Equals(first, second);
            }

            public static bool operator !=(ComposeOptions first, ComposeOptions second)
            {
                return !(first == second);
            }

            public override string ToString()
            {
                String s = "";
                if (addBMouse) s += "Mouse ";
                if (addBLeft) s += "Left ";
                if (addBRight) s += "Right ";
                if (addBMiddle) s += "Middle ";
                if (addBLeftDouble) s += "LeftDouble ";
                if (addBRightDouble) s += "RightDouble ";
                if (addBWheelUp) s += "WheelUp ";
                if (addBWheelDown) s += "WheelDown ";
                return s;
            }

            public override bool Equals(object obj)
            {
                return base.Equals(obj);
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }

        static ComposeOptions lastComposeOptions;
        static Bitmap lastComposedBitmap;


        public static Bitmap Compose(ComposeOptions c) {

            if (lastComposedBitmap != null && (c == lastComposeOptions))
            {
                return lastComposedBitmap;
            }



            var byDpi = ScaledByDpi; // take reference to prevent datarace with update/replace logic on scaling factor change
            if (!byDpi.ContainsKey(c.dpi))
            {
                byDpi.Add(c.dpi, CreateScaledBitmapCollection(appliedScalingFactor, c.dpi));
            }

            BitmapCollection scaled = byDpi[c.dpi];
            Log.e("DPI", $"COMPOSE! {c}, {scaled.BMouse.Size.Width}");

            var targetBitmap = new Bitmap(scaled.BMouse.Size.Width, scaled.BMouse.Size.Height, PixelFormat.Format32bppArgb);
            Graphics graph = Graphics.FromImage(targetBitmap);

            if (c.addBMouse) graph.DrawImageUnscaled(scaled.BMouse, 0, 0);
            if (c.addBLeft) graph.DrawImageUnscaled(scaled.BLeft, 0, 0);
            if (c.addBRight) graph.DrawImageUnscaled(scaled.BRight, 0, 0);
            if (c.addBMiddle) graph.DrawImageUnscaled(scaled.BMiddle, 0, 0);
            if (c.addBLeftDouble) graph.DrawImageUnscaled(scaled.BLeftDouble, 0, 0);
            if (c.addBRightDouble) graph.DrawImageUnscaled(scaled.BRightDouble, 0, 0);
            if (c.addBWheelUp) graph.DrawImageUnscaled(scaled.BWheelUp, 0, 0);
            if (c.addBWheelDown) graph.DrawImageUnscaled(scaled.BWheelDown, 0, 0);

            lastComposeOptions = c;
            lastComposedBitmap = targetBitmap;
            return targetBitmap;
        }
    }
}
