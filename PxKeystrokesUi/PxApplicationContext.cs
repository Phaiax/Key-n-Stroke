using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using PxKeystrokesWPF;
using System.ComponentModel;

namespace PxKeystrokesUi
{


    class PxApplicationContext : ApplicationContext
    {
        KeystrokesDisplay myUi;

        public PxApplicationContext()
        {
            Log.SetTagFilter("POS|DPI");

            Application.ApplicationExit += Application_ApplicationExit;

            ImageResources.Init();
            InitSettings();
            InitKeyboardInterception();

            mySettings.PropertyChanged += OnSettingChanged;
            mySettings.HistoryTimeout = 30000;

            //PxKeystrokesWPF.Settings1 settings1 = new PxKeystrokesWPF.Settings1(mySettings);
            //System.Windows.Forms.Integration.ElementHost.EnableModelessKeyboardInterop(settings1);
            //WPFHelper.SetOwner(this, dlg);
            //dlg.ShowDialog();
            //settings1.ShowDialog();

            myUi = new KeystrokesDisplay(myKeystrokeConverter, mySettings);
            myUi.FormClosed += OnUiClosed;
            //myUi.Show();
            this.MainForm = myUi;

            OnCursorIndicatorSettingChanged();
            OnButtonIndicatorSettingChanged();
        }

        SettingsStore mySettings;

        private void InitSettings()
        {
            mySettings = new SettingsStore();

            Rectangle R = Screen.PrimaryScreen.WorkingArea;
            mySettings.WindowLocationDefault = new System.Windows.Point(R.Right - mySettings.WindowSizeDefault.Width - 20,
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
            DisableButtonIndicator();
            ExitThread();
        }

        void Application_ApplicationExit(object sender, EventArgs e)
        {
            DisableCursorIndicator();
            DisableButtonIndicator();
            myUi.Close();
        }

        private void OnSettingChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "EnableCursorIndicator":
                    OnCursorIndicatorSettingChanged();
                    break;
                case "ButtonIndicator":
                    OnButtonIndicatorSettingChanged();
                    break;
            }
        }

        private void OnButtonIndicatorSettingChanged()
        {
            if ( mySettings.ButtonIndicator == ButtonIndicatorType.Disabled)
            {
                DisableButtonIndicator();
            }
            else
            {
                EnableButtonIndicator();
            }
        }

        //ButtonIndicator2 myButtons = null;

        private void EnableButtonIndicator()
        {
            /*if (myButtons != null)
                return;
            Log.e("BI", "EnableButtonIndicator");
            EnableMouseHook();
            myButtons = new ButtonIndicator2(myMouseHook, mySettings);
            myButtons.Show();*/
        }


        private void DisableButtonIndicator()
        {
            /*if (myButtons == null)
                return;
            myButtons.Close();
            myButtons = null;
            DisableMouseHookIfNotNeeded();
            Log.e("BI", "DisableButtonIndicator");*/
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
            Log.e("CI", "EnableCursorIndicator");
            EnableMouseHook();
            myCursor = new CursorIndicator(myMouseHook, mySettings);
            myCursor.FormClosed += myCursor_FormClosed;
            myCursor.Show();
        }

        void myCursor_FormClosed(object sender, FormClosedEventArgs e)
        {
        }

        private void DisableCursorIndicator()
        {
            if (myCursor == null)
                return;
            myCursor.Close();
            myCursor = null;
            DisableMouseHookIfNotNeeded();
            Log.e("CI", "DisableCursorIndicator");
        }

        IMouseRawEventProvider myMouseHook = null;

        private void EnableMouseHook()
        {
            if (myMouseHook != null)
                return;
                //DisableMouseHook();
            myMouseHook = new MouseHook();
        }

        private void DisableMouseHookIfNotNeeded()
        {
            if (myCursor == null /*&& myButtons == null*/)
                DisableMouseHook();
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
