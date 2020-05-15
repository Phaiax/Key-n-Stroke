using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using System.Windows;
using PxKeystrokesWPF;

namespace PxKeystrokesWPF
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        KeystrokeDisplay myUi;

        [System.STAThreadAttribute()]
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public static void Main()
        {
            App app = new App();
            app.InitializeComponent();
            app.Run();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            //DisableCursorIndicator();
            DisableButtonIndicator();
            //myUi.Close();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            Log.SetTagFilter("OPACITY");

            ImageResources.Init();
            InitSettings();
            InitKeyboardInterception();

            mySettings.PropertyChanged += OnSettingChanged;
            mySettings.HistoryTimeout = 30000;

            //Settings1 settings1 = new Settings1(mySettings);
            //System.Windows.Forms.Integration.ElementHost.EnableModelessKeyboardInterop(settings1);
            //WPFHelper.SetOwner(this, dlg);
            //dlg.ShowDialog();
            //settings1.Show();
            //settings1.ShowDialog();
            mySettings.ButtonIndicator = ButtonIndicatorType.PicsAroundCursor;

            myUi = new KeystrokeDisplay(myKeystrokeConverter, mySettings);
            //myUi.FormClosed += OnUiClosed;
            myUi.Show();
            //this.MainForm = myUi;

            //OnCursorIndicatorSettingChanged();
            OnButtonIndicatorSettingChanged();
        }

        protected override void OnActivated(EventArgs e)
        {

        }

        SettingsStore mySettings;

        private void InitSettings()
        {
            mySettings = new SettingsStore();

            
            mySettings.WindowLocationDefault = new Point(
                System.Windows.SystemParameters.PrimaryScreenWidth - mySettings.WindowSizeDefault.Width - 20,
                System.Windows.SystemParameters.PrimaryScreenHeight - mySettings.WindowSizeDefault.Height);

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
            //DisableCursorIndicator();
            DisableButtonIndicator();
            //ExitThread();
        }

        private void OnSettingChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "EnableCursorIndicator":
                    //OnCursorIndicatorSettingChanged();
                    break;
                case "ButtonIndicator":
                    OnButtonIndicatorSettingChanged();
                    break;
            }
        }

        private void OnButtonIndicatorSettingChanged()
        {
            if (mySettings.ButtonIndicator == ButtonIndicatorType.Disabled)
            {
                DisableButtonIndicator();
            }
            else
            {
                EnableButtonIndicator();
            }
        }

        ButtonIndicator1 myButtons = null;

        private void EnableButtonIndicator()
        {
            if (myButtons != null)
                return;
            Log.e("BI", "EnableButtonIndicator");
            EnableMouseHook();
            myButtons = new ButtonIndicator1(myMouseHook, mySettings);
            myButtons.Show();
        }


        private void DisableButtonIndicator()
        {
            if (myButtons == null)
                return;
            myButtons.Close();
            myButtons = null;
            DisableMouseHookIfNotNeeded();
            Log.e("BI", "DisableButtonIndicator");
        }
        /*
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
            myCursor.Show();
        }



        private void DisableCursorIndicator()
        {
            if (myCursor == null)
                return;
            myCursor.Close();
            myCursor = null;
            DisableMouseHookIfNotNeeded();
            Log.e("CI", "DisableCursorIndicator");
        }*/

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
            if (/*myCursor == null &&*/ myButtons == null)
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
