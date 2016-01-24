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
        
        public static Bitmap mouse;

        public static void Init()
        {
            try
            {
                _assembly = Assembly.GetExecutingAssembly();
                
                foreach(string i in _assembly.GetManifestResourceNames())
                {
                    Log.e("RES", i);
                }
                _imageStream = _assembly.GetManifestResourceStream("PxKeystrokesUi.Resources.mouse.png");
                mouse = new Bitmap(_imageStream);
            }
            catch
            {
                Log.e("RES", "Error accessing resources!");
            }
        }

         
    }
}
