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

        public PxApplicationContext()
        {
            InitSettings();
            InitKeyboardInterception();

            mySettings.settingChanged += OnSettingChanged;

            myUi = new KeystrokesDisplay(myKeystrokeConverter, mySettings);
            myUi.FormClosed += OnUiClosed;
            myUi.Show();
            this.MainForm = myUi;

            OnCursorIndicatorSettingChanged();
        }

        SettingsStore mySettings;

        private void InitSettings()
        {
            mySettings = new SettingsStore();

            Rectangle R = Screen.PrimaryScreen.WorkingArea;
            mySettings.WindowLocationDefault = new Point(R.Right - mySettings.WindowSizeDefault.Width - 20,
                R.Bottom - mySettings.WindowSizeDefault.Height);

            //mySettings.ClearAll(); // test defaults
            mySettings.LoadAll();
        }

        IKeyboardRawEventProvider myKeyboardHook;
        IKeystrokeEventProvider myKeystrokeConverter;

        private void InitKeyboardInterception()
        {
            myKeyboardHook = new KeyboardHook();
            myKeystrokeConverter = new KeystrokeParser(myKeyboardHook);
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

        CursorIndicator myCursor = null;

        private void EnableCursorIndicator()
        {
            if (myCursor != null)
                return;
            Console.WriteLine("EnableCursorIndicator");
            EnableMouseHook();
            myCursor = new CursorIndicator(myMouseHook, mySettings);
            myCursor.FormClosed += myCursor_FormClosed;
            myCursor.Show();
        }

        void myCursor_FormClosed(object sender, FormClosedEventArgs e)
        {
            DisableMouseHook();
        }

        private void DisableCursorIndicator()
        {
            if (myCursor == null)
                return;
            myCursor.Close();
            myCursor = null;
            Console.WriteLine("DisableCursorIndicator");
        }

        IMouseRawEventProvider myMouseHook = null;

        private void EnableMouseHook()
        {
            if (myMouseHook != null)
                DisableMouseHook();
            myMouseHook = new MouseHook();
        }
        
        private void DisableMouseHook()
        {
            if (myMouseHook == null)
                return;
            myMouseHook.Dispose();
            myMouseHook = null;
        }
    }
}
