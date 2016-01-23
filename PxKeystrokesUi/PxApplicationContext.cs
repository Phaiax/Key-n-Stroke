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
            InitSettings();
            InitKeyboardAndMouseInterception();

            mySettings.settingChanged += OnSettingChanged;

            myUi = new KeystrokesDisplay(myKeystrokeConverter, mySettings);
            myUi.FormClosed += OnUiClosed;
            myUi.Show();
            this.MainForm = myUi;

            OnCursorIndicatorSettingChanged();
        }

        private void InitSettings()
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
            DisableCursorIndicator();
            ExitThread();
        }

        private void OnSettingChanged(SettingsChangedEventArgs e)
        {
            if (e.Name == "EnableCursorIndicator")
            {
                OnCursorIndicatorSettingChanged();
            }
        }

        private void OnCursorIndicatorSettingChanged()
        {
            if (mySettings.EnableCursorIndicator)
            {
                EnableCursorIndicator();
            }
            else
            {
                DisableCursorIndicator();
            }
        }

        private void EnableCursorIndicator()
        {
            Console.WriteLine("EnableCursorIndicator");
        }

        private void DisableCursorIndicator()
        {
            Console.WriteLine("DisableCursorIndicator");
        }
    }
}
