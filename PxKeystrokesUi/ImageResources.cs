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

namespace PxKeystrokesUi
{
    class ImageResources
    {
        static Assembly _assembly;


        public static Bitmap BMouse;
        public static Bitmap BLeft;
        public static Bitmap BRight;
        public static Bitmap BMiddle;
        public static Bitmap BLeftDouble;
        public static Bitmap BRightDouble;
        public static Bitmap BWheelUp;
        public static Bitmap BWheelDown;

        static Bitmap OrigBMouse;
        static Bitmap OrigBLeft;
        static Bitmap OrigBRight;
        static Bitmap OrigBMiddle;
        static Bitmap OrigBLeftDouble;
        static Bitmap OrigBRightDouble;
        static Bitmap OrigBWheelUp;
        static Bitmap OrigBWheelDown;

        public static void Init()
        {
            try
            {
                _assembly = Assembly.GetExecutingAssembly();

                foreach(string i in _assembly.GetManifestResourceNames())
                {
                    Log.e("RES", i);
                }
                OrigBMouse = new Bitmap(_assembly.GetManifestResourceStream("PxKeystrokesUi.Resources.mouse.png"));
                OrigBLeft = new Bitmap(_assembly.GetManifestResourceStream("PxKeystrokesUi.Resources.mouse_left.png"));
                OrigBRight = new Bitmap(_assembly.GetManifestResourceStream("PxKeystrokesUi.Resources.mouse_right.png"));
                OrigBMiddle = new Bitmap(_assembly.GetManifestResourceStream("PxKeystrokesUi.Resources.mouse_middle.png"));
                OrigBLeftDouble = new Bitmap(_assembly.GetManifestResourceStream("PxKeystrokesUi.Resources.mouse_left_double.png"));
                OrigBRightDouble = new Bitmap(_assembly.GetManifestResourceStream("PxKeystrokesUi.Resources.mouse_right_double.png"));
                OrigBWheelUp = new Bitmap(_assembly.GetManifestResourceStream("PxKeystrokesUi.Resources.mouse_wheel_up.png"));
                OrigBWheelDown = new Bitmap(_assembly.GetManifestResourceStream("PxKeystrokesUi.Resources.mouse_wheel_down.png"));

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
            if (appliedScalingFactor != scalingfactor)
            {
                BMouse = Scale(scalingfactor, OrigBMouse);
                BLeft = Scale(scalingfactor, OrigBLeft);
                BRight = Scale(scalingfactor, OrigBRight);
                BMiddle = Scale(scalingfactor, OrigBMiddle);
                BLeftDouble = Scale(scalingfactor, OrigBLeftDouble);
                BRightDouble = Scale(scalingfactor, OrigBRightDouble);
                BWheelUp = Scale(scalingfactor, OrigBWheelUp);
                BWheelDown = Scale(scalingfactor, OrigBWheelDown);

                appliedScalingFactor = scalingfactor;
            }
        }

        private static Bitmap Scale(float scalingFactor, Bitmap original)
        {
            var scaledWidth = (int)(original.Width * scalingFactor);
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
        }

        static ComposeOptions lastComposeOptions;
        static Bitmap lastComposedBitmap;


        public static Bitmap Compose(ComposeOptions c) {

            if (lastComposedBitmap != null && (c == lastComposeOptions))
            {
                return lastComposedBitmap;
            }


            Log.e("RES", "COMPOSE! " + c.ToString());


            var targetBitmap = new Bitmap(BMouse.Size.Width, BMouse.Size.Height, PixelFormat.Format32bppArgb);
            Graphics graph = Graphics.FromImage(targetBitmap);

            if (c.addBMouse) graph.DrawImageUnscaled(BMouse, 0, 0);
            if (c.addBLeft) graph.DrawImageUnscaled(BLeft, 0, 0);
            if (c.addBRight) graph.DrawImageUnscaled(BRight, 0, 0);
            if (c.addBMiddle) graph.DrawImageUnscaled(BMiddle, 0, 0);
            if (c.addBLeftDouble) graph.DrawImageUnscaled(BLeftDouble, 0, 0);
            if (c.addBRightDouble) graph.DrawImageUnscaled(BRightDouble, 0, 0);
            if (c.addBWheelUp) graph.DrawImageUnscaled(BWheelUp, 0, 0);
            if (c.addBWheelDown) graph.DrawImageUnscaled(BWheelDown, 0, 0);

            lastComposeOptions = c;
            lastComposedBitmap = targetBitmap;
            return targetBitmap;
        }
    }
}
