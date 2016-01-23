using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace PxKeystrokesUi
{


    class PxApplicationContext : ApplicationContext
    {
        KeystrokesDisplay myUi;
        SettingsStore mySettings;
        IKeyboardRawEventProvider myKeyboardHook;
        IKeystrokeEventProvider myKeystrokeConverter;
        IMouseRawEventProvider myMouseHook;

        public PxApplicationContext()
        {
            LoadSettings();
            InitKeyboardAndMouseInterception();

            myUi = new KeystrokesDisplay(myKeystrokeConverter, mySettings);
            myUi.FormClosed += OnUiClosed;
            myUi.Show();
            this.MainForm = myUi;
        }

        private void LoadSettings()
        {
            mySettings = new SettingsStore();

            Rectangle R = Screen.PrimaryScreen.WorkingArea;
            mySettings.WindowLocationDefault = new Point(R.Right - mySettings.WindowSizeDefault.Width - 20,
                R.Bottom - mySettings.WindowSizeDefault.Height);

            //mySettings.ClearAll(); // test defaults
            mySettings.LoadAll();
        }

        private void InitKeyboardAndMouseInterception()
        {
            myKeyboardHook = new KeyboardHook();
            myKeystrokeConverter = new KeystrokeParser(myKeyboardHook);

            myMouseHook = new MouseHook();
        }

        private void OnUiClosed(object sender, EventArgs e)
        {
            DisableMouseHighlight();
            ExitThread();
        }

        private void ActivateMouseHighlight()
        {

        }

        private void DisableMouseHighlight()
        {

        }
    }
}
