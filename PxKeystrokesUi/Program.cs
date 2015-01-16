using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace PxKeystrokesUi
{
    static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            IKeyboardRawEventProvider myKeyboardHook = new KeyboardHook();
            IKeystrokeEventProvider myKeystrokeConverter = new KeystrokeParser(myKeyboardHook);
            KeystrokesDisplay myForm = new KeystrokesDisplay(myKeystrokeConverter);

            Application.Run(myForm);
        }
    }
}
