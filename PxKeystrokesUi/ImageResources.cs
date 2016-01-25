using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using System.Drawing;
		
namespace PxKeystrokesUi
{
    class ImageResources
    {
        static Assembly _assembly;
        static Stream _imageStream;

        public static Bitmap BMouse;
        public static Bitmap BLeft;
        public static Bitmap BRight;
        public static Bitmap BMiddle;
        public static Bitmap BLeftDouble;
        public static Bitmap BRightDouble;
        public static Bitmap BWheel;
        public static Bitmap BWheelUp;
        public static Bitmap BWheelDown;

        public static void Init()
        {
            try
            {
                _assembly = Assembly.GetExecutingAssembly();
                
                foreach(string i in _assembly.GetManifestResourceNames())
                {
                    Log.e("RES", i);
                }
                BMouse = new Bitmap(_assembly.GetManifestResourceStream("PxKeystrokesUi.Resources.mouse.png"));
                BLeft = new Bitmap(_assembly.GetManifestResourceStream("PxKeystrokesUi.Resources.mouse_left.png"));
                BRight = new Bitmap(_assembly.GetManifestResourceStream("PxKeystrokesUi.Resources.mouse_right.png"));
                BMiddle = new Bitmap(_assembly.GetManifestResourceStream("PxKeystrokesUi.Resources.mouse_middle.png"));
                BLeftDouble = new Bitmap(_assembly.GetManifestResourceStream("PxKeystrokesUi.Resources.mouse_left_double.png"));
                BRightDouble = new Bitmap(_assembly.GetManifestResourceStream("PxKeystrokesUi.Resources.mouse_right_double.png"));
                BWheelUp = new Bitmap(_assembly.GetManifestResourceStream("PxKeystrokesUi.Resources.mouse_wheel_up.png"));
                BWheelDown = new Bitmap(_assembly.GetManifestResourceStream("PxKeystrokesUi.Resources.mouse_wheel_down.png"));

            }
            catch
            {
                Log.e("RES", "Error accessing resources!");
            }
        }

         
    }
}
