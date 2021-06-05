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
            public Bitmap MCtrl;
            public Bitmap MWin;
            public Bitmap MAlt;
            public Bitmap MShift;
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
                Orig.MCtrl = new Bitmap(_assembly.GetManifestResourceStream("PxKeystrokesWPF.Resources.mouse_modifier_ctrl.png"));
                Orig.MWin = new Bitmap(_assembly.GetManifestResourceStream("PxKeystrokesWPF.Resources.mouse_modifier_win.png"));
                Orig.MAlt = new Bitmap(_assembly.GetManifestResourceStream("PxKeystrokesWPF.Resources.mouse_modifier_alt.png"));
                Orig.MShift = new Bitmap(_assembly.GetManifestResourceStream("PxKeystrokesWPF.Resources.mouse_modifier_shift.png"));
            }
            catch
            {
                Log.e("RES", "Error accessing resources!");
            }

            ApplyScalingFactor(1.0f);
        }

        public static void ExportBuiltinRessources(string exportFolder)
        {
            string[] filenames = { "mouse.svg",
                                   "mouse.png",
                                   "mouse_left.png",
                                   "mouse_right.png",
                                   "mouse_middle.png",
                                   "mouse_left_double.png",
                                   "mouse_right_double.png",
                                   "mouse_wheel_up.png",
                                   "mouse_wheel_down.png",
                                   "mouse_modifier_ctrl.png",
                                   "mouse_modifier_win.png",
                                   "mouse_modifier_alt.png",
                                   "mouse_modifier_shift.png" };
            foreach (string png_name in filenames)
            {
                try
                {

                    string target = Path.Combine(exportFolder, png_name);
                    if (File.Exists(target))
                    {
                        var result = MessageBox.Show($"Overwrite {png_name}?", $"File already exists", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning, MessageBoxResult.Cancel);
                        if (result == MessageBoxResult.Cancel)
                        {
                            return;
                        }
                        else if (result == MessageBoxResult.No)
                        {
                            continue;
                        }
                        else
                        {
                            // Yes -> Overwrite
                        }
                    }
                    using (var fs = new FileStream(target, FileMode.Create, FileAccess.Write))
                    {
                        _assembly.GetManifestResourceStream($"PxKeystrokesWPF.Resources.{png_name}").CopyTo(fs);
                    }
                }
                catch (Exception)
                {

                }
            }
        }
        static double appliedScalingFactor = -1.0;

        public static void ApplyScalingFactor(double scalingfactor)
        {
            scalingfactor = Math.Min(2f, Math.Max(0.1f, scalingfactor));

            if (appliedScalingFactor != scalingfactor)
            {
                var newByDpi = new Dictionary<uint, BitmapCollection>();
                
                List<uint> dpis = NativeMethodsWindow.GetAllUsedDpis();

                foreach (uint dpi in dpis)
                {
                    newByDpi.Add(dpi, CreateScaledBitmapCollection((float)scalingfactor, dpi));
                }

                appliedScalingFactor = scalingfactor;
                ScaledByDpi = newByDpi;
                lastComposedBitmap = null; // Force regeneration of Image
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
                BWheelDown = Scale(scalingFactor, Orig.BWheelDown, dpi),
                MCtrl = Scale(scalingFactor, Orig.MCtrl, dpi),
                MWin = Scale(scalingFactor, Orig.MWin, dpi),
                MAlt = Scale(scalingFactor, Orig.MAlt, dpi),
                MShift = Scale(scalingFactor, Orig.MShift, dpi)
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
            public bool addMCtrl;
            public bool addMAlt;
            public bool addMWin;
            public bool addMShift;

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
                if (addMCtrl) s += "Ctrl ";
                if (addMAlt) s += "Alt ";
                if (addMWin) s += "Win ";
                if (addMShift) s += "Shift ";
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
                byDpi.Add(c.dpi, CreateScaledBitmapCollection((float) appliedScalingFactor, c.dpi));
            }

            BitmapCollection scaled = byDpi[c.dpi];
            Log.e("DPI", $"COMPOSE! {c.dpi}, {c}, {scaled.BMouse.Size.Width}");

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
            if (c.addMCtrl) graph.DrawImageUnscaled(scaled.MCtrl, 0, 0);
            if (c.addMWin) graph.DrawImageUnscaled(scaled.MWin, 0, 0);
            if (c.addMAlt) graph.DrawImageUnscaled(scaled.MAlt, 0, 0);
            if (c.addMShift) graph.DrawImageUnscaled(scaled.MShift, 0, 0);

            lastComposeOptions = c;
            lastComposedBitmap = targetBitmap;
            return targetBitmap;
        }
    }
}
