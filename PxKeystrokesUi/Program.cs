using System;
using System.Collections.Generic;
using System.Drawing;
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

            SettingsStore mySettings = new SettingsStore();

            Rectangle R = Screen.PrimaryScreen.WorkingArea;
            mySettings.WindowLocationDefault = new Point(R.Right - mySettings.WindowSizeDefault.Width - 20,
                R.Bottom - mySettings.WindowSizeDefault.Height);

            //mySettings.ClearAll(); // test defaults
            mySettings.LoadAll();

            KeystrokesDisplay myForm = new KeystrokesDisplay(myKeystrokeConverter, mySettings);

            Application.Run(myForm);
        }
    }
}
