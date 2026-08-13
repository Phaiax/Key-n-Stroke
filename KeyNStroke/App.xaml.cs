using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing.Printing;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using KeyNStroke;

namespace KeyNStroke
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {

        #region Main()

        [System.STAThreadAttribute()]
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public static void Main()
        {
            App app = new App();
            app.InitializeComponent();
            app.Run();
        }

        #endregion

        #region Init

        SettingsStore mySettings;
        Window welcomeWindow;
        Settings1 settingsWindow;
        DrawWindow drawWindow;

        protected override void OnStartup(StartupEventArgs e)
        {
            Log.SetTagFilter("UPDATE");

            if (Updater.Updater.HandleArgs(e.Args))
            {
                Shutdown();
            }

            InitSettings();
            ImageResources.Init(mySettings.ButtonIndicatorCustomIconsFolder);
            InitKeyboardInterception();

            mySettings.PropertyChanged += OnSettingChanged;
            myKeystrokeConverter.KeystrokeEvent += m_KeystrokeEvent;

            //myUi.FormClosed += OnUiClosed;
            mySettings.CallPropertyChangedForAllProperties();

            makeNotifyIcon();

            if (mySettings.WelcomeOnStartup)
            {
                showWelcomeWindow();
            }
        }

        protected override void OnActivated(EventArgs e)
        {

        }


        private void InitSettings()
        {
            mySettings = new SettingsStore();

            mySettings.WindowLocationDefault = new Point(
                System.Windows.SystemParameters.PrimaryScreenWidth - mySettings.WindowSizeDefault.Width - 20,
                System.Windows.SystemParameters.PrimaryScreenHeight - mySettings.WindowSizeDefault.Height - 40);

            //mySettings.ResetAll(); // test defaults
            mySettings.LoadAll();
        }

        IKeyboardRawEventProvider myKeyboardHook;
        IKeystrokeEventProvider myKeystrokeConverter;

        private void InitKeyboardInterception()
        {
            myKeyboardHook = new KeyboardHook();
            myKeystrokeConverter = new KeystrokeParser(myKeyboardHook);
        }

        #endregion

        #region Closing/Exiting

        private void OnUiClosed(object sender, EventArgs e)
        {
            DisableCursorIndicator();
            DisableButtonIndicator();
            DisableKeystrokeHistory();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            OnUiClosed(this, e);
            this.notifyIcon_main.Visible = false;
        }

        #endregion

        #region Tray Icon

        private System.Windows.Forms.NotifyIcon notifyIcon_main;
        void makeNotifyIcon()
        {
            var _assembly = System.Reflection.Assembly.GetExecutingAssembly();
            
            Stream iconStream = Application.GetResourceStream(new Uri("pack://application:,,,/Key-n-Stroke;component/Resources/app.ico")).Stream;
            var icon = new System.Drawing.Icon(iconStream);

            this.notifyIcon_main = new System.Windows.Forms.NotifyIcon();
            this.notifyIcon_main.BalloonTipText = "xfxfn";
            this.notifyIcon_main.BalloonTipTitle = "xfgn";
            this.notifyIcon_main.Click += new EventHandler(notifyIcon_Click);
            this.notifyIcon_main.Icon = icon;
            this.notifyIcon_main.Visible = true;

        }

        void notifyIcon_Click(object sender, EventArgs e)
        {
            showSettingsWindow();
        }

        #endregion

        #region Settings Window

        public void showSettingsWindow()
        {
            if (settingsWindow == null)
            {
                settingsWindow = new Settings1(mySettings, myKeystrokeConverter);
            }
            if (settingsWindow.WindowState == WindowState.Minimized)
            {
                settingsWindow.WindowState = WindowState.Normal;
            }
            settingsWindow.Show();
            settingsWindow.Activate();
            settingsWindow.Focus();
        }

        public void onSettingsWindowClosed()
        {
            settingsWindow = null;
        }

        #endregion

        #region Welcome Window

        public void showWelcomeWindow()
        {
            if (welcomeWindow == null)
            {
                welcomeWindow = new Welcome(mySettings);
                welcomeWindow.Show();
            }
            else
            {
                welcomeWindow.Activate();
            }
        }

        public void onWelcomeWindowClosed()
        {
            welcomeWindow = null;
        }

        #endregion

        #region Draw Window

        public void showDrawWindow(List<UIElement> drawnFromAnnotate = null, UIElement targetCanvas = null)
        {
            if (drawWindow == null)
            {
                drawWindow = new DrawWindow(mySettings);
            }
            if (drawnFromAnnotate != null)
            {
                // Chụp màn hình nền thực tế của Windows (desktop)
                int screenWidth = (int)SystemParameters.PrimaryScreenWidth;
                int screenHeight = (int)SystemParameters.PrimaryScreenHeight;

                using (System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(screenWidth, screenHeight))
                {
                    using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmp))
                    {
                        g.CopyFromScreen(0, 0, 0, 0, bmp.Size);
                    }
                    
                    // Chuyển Bitmap thành BitmapSource của WPF
                    var handle = bmp.GetHbitmap();
                    try
                    {
                        ImageSource imgSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                            handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                        drawWindow.ImportFromAnnotate(drawnFromAnnotate, imgSource);
                    }
                    finally
                    {
                        DeleteObject(handle);
                    }
                }
            }
            if (drawWindow.WindowState == WindowState.Minimized)
            {
                drawWindow.WindowState = WindowState.Normal;
            }
            drawWindow.Show();
            drawWindow.Activate();
            drawWindow.Focus();
        }

        public void onDrawWindowClosed()
        {
            drawWindow = null;
        }

        #endregion

        #region Shortcut

        public string StandbyShortcut;
        public string ToggleKeystrokeHistoryShortcut;
        public string ToggleUiShortcut;
        public string AnnotateLineShortcut;

        void m_KeystrokeEvent(KeystrokeEventArgs e)
        {
            string pressed = e.ShortcutIdentifier();
            e.raw.preventDefault = e.raw.preventDefault || CheckForTrigger(pressed);
            if (KeystrokeHistoryWindow != null)
            {
                // KeystrokeDisplay uses setting triggers, but we can also handle preventDefault here or let it bubble.
            }
        }

        private bool CheckForTrigger(string pressed)
        {
            if (StandbyShortcut != null && KeystrokeDisplay.ShortcutMatches(StandbyShortcut, pressed))
            {
                mySettings.Standby = !mySettings.Standby;
                return true;
            }
            if (ToggleKeystrokeHistoryShortcut != null && KeystrokeDisplay.ShortcutMatches(ToggleKeystrokeHistoryShortcut, pressed))
            {
                mySettings.EnableKeystrokeHistory = !mySettings.EnableKeystrokeHistory;
                return true;
            }
            if (ToggleUiShortcut != null && KeystrokeDisplay.ShortcutMatches(ToggleUiShortcut, pressed))
            {
                if (settingsWindow != null && settingsWindow.IsVisible && settingsWindow.WindowState != WindowState.Minimized)
                {
                    settingsWindow.Close();
                }
                else
                {
                    showSettingsWindow();
                }
                return true;
            }
            if (AnnotateLineShortcut != null && KeystrokeDisplay.ShortcutMatches(AnnotateLineShortcut, pressed))
            {
                if (!mySettings.EnableAnnotateLine)
                {
                    mySettings.EnableAnnotateLine = true;
                }
                if (AnnotateLineWindow == null)
                {
                    EnableAnnotateLine();
                }
                AnnotateLineWindow?.ToggleDrawingMode();
                return true;
            }
            return false;
        }

        public void SetStandbyShortcut(string shortcut)
        {
            if (KeystrokeDisplay.ValidateShortcutSetting(shortcut))
            {
                StandbyShortcut = shortcut;
            }
            else
            {
                StandbyShortcut = mySettings.StandbyShortcutDefault;
            }
        }

        public void SetToggleKeystrokeHistoryShortcut(string shortcut)
        {
            if (KeystrokeDisplay.ValidateShortcutSetting(shortcut))
            {
                ToggleKeystrokeHistoryShortcut = shortcut;
            }
            else
            {
                ToggleKeystrokeHistoryShortcut = mySettings.ToggleKeystrokeHistoryShortcutDefault;
            }
        }

        public void SetToggleUiShortcut(string shortcut)
        {
            if (KeystrokeDisplay.ValidateShortcutSetting(shortcut))
            {
                ToggleUiShortcut = shortcut;
            }
            else
            {
                ToggleUiShortcut = mySettings.ToggleUiShortcutDefault;
            }
        }

        public void SetAnnotateLineShortcut(string shortcut)
        {
            if (KeystrokeDisplay.ValidateShortcutSetting(shortcut))
            {
                AnnotateLineShortcut = shortcut;
            }
            else
            {
                AnnotateLineShortcut = mySettings.AnnotateLineShortcutDefault;
            }
        }

        #endregion

        #region OnSettingChanged

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
                case "EnableKeystrokeHistory":
                    OnKeystrokeHistorySettingChanged();
                    break;
                case "EnableAnnotateLine":
                    OnAnnotateLineSettingChanged();
                    break;
                case "ButtonIndicatorCustomIconsFolder":
                case "ButtonIndicatorUseCustomIcons":
                    if (mySettings.ButtonIndicatorUseCustomIcons)
                    {
                        ImageResources.ReloadRessources(mySettings.ButtonIndicatorCustomIconsFolder);
                    } else
                    {
                        ImageResources.ReloadRessources(null);
                    }
                    break;
                case "StandbyShortcut":
                    SetStandbyShortcut(mySettings.StandbyShortcut);
                    break;
                case "ToggleKeystrokeHistoryShortcut":
                    SetToggleKeystrokeHistoryShortcut(mySettings.ToggleKeystrokeHistoryShortcut);
                    break;
                case "ToggleUiShortcut":
                    SetToggleUiShortcut(mySettings.ToggleUiShortcut);
                    break;
                case "AnnotateLineShortcut":
                    SetAnnotateLineShortcut(mySettings.AnnotateLineShortcut);
                    break;
                case "GlobalKey":
                    if (myKeyboardHook != null)
                    {
                        myKeyboardHook.GlobalKeyEnabled = mySettings.GlobalKey;
                    }
                    break;
                case "Standby":
                    OnCursorIndicatorSettingChanged();
                    OnButtonIndicatorSettingChanged();
                    OnKeystrokeHistorySettingChanged();
                    OnAnnotateLineSettingChanged();
                    break;
            }
        }

        #endregion

        #region Keystroke History

        KeystrokeDisplay KeystrokeHistoryWindow;
        bool KeystrokeHistoryVisible;

        private void OnKeystrokeHistorySettingChanged()
        {
            if (mySettings.EnableKeystrokeHistory && !mySettings.Standby)
            {
                EnableKeystrokeHistory();
            }
            else
            {
                DisableKeystrokeHistory();
            }
        }

        private void EnableKeystrokeHistory()
        {
            if (KeystrokeHistoryVisible || KeystrokeHistoryWindow != null)
                return;
            KeystrokeHistoryVisible = true; // Prevent Recursive call
            KeystrokeHistoryWindow = new KeystrokeDisplay(myKeystrokeConverter, mySettings);
            KeystrokeHistoryWindow.Show();
        }

        private void DisableKeystrokeHistory()
        {
            KeystrokeHistoryVisible = false;
            if (KeystrokeHistoryWindow == null)
                return;
            KeystrokeHistoryWindow.Close();
            KeystrokeHistoryWindow = null;
        }

        #endregion

        #region Button Indicator

        ButtonIndicator1 ButtonIndicatorWindow = null;

        private void OnButtonIndicatorSettingChanged()
        {
            if (mySettings.ButtonIndicator != ButtonIndicatorType.Disabled && !mySettings.Standby)
            {
                EnableButtonIndicator();
            }
            else
            {
                DisableButtonIndicator();
            }
        }


        private void EnableButtonIndicator()
        {
            if (ButtonIndicatorWindow != null)
                return;
            Log.e("BI", "EnableButtonIndicator");
            EnableMouseHook();
            ButtonIndicatorWindow = new ButtonIndicator1(myMouseHook, myKeystrokeConverter, mySettings);
            ButtonIndicatorWindow.Show();
        }


        private void DisableButtonIndicator()
        {
            if (ButtonIndicatorWindow == null)
                return;
            ButtonIndicatorWindow.Close();
            ButtonIndicatorWindow = null;
            DisableMouseHookIfNotNeeded();
            Log.e("BI", "DisableButtonIndicator");
        }

        #endregion

        #region Cursor Indicator

        CursorIndicator1 CursorIndicatorWindow = null;

        private void OnCursorIndicatorSettingChanged()
        {
            if (mySettings.EnableCursorIndicator && !mySettings.Standby)
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
            if (CursorIndicatorWindow != null)
                return;
            Log.e("CI", "EnableCursorIndicator");
            EnableMouseHook();
            CursorIndicatorWindow = new CursorIndicator1(myMouseHook, mySettings);
            CursorIndicatorWindow.Show();
        }



        private void DisableCursorIndicator()
        {
            if (CursorIndicatorWindow == null)
                return;
            CursorIndicatorWindow.Close();
            CursorIndicatorWindow = null;
            DisableMouseHookIfNotNeeded();
            Log.e("CI", "DisableCursorIndicator");
        }


        #endregion

        #region Annotate Line

        AnnotateLine AnnotateLineWindow;


        private void OnAnnotateLineSettingChanged()
        {
            if (mySettings.EnableAnnotateLine && !mySettings.Standby)
            {
                EnableAnnotateLine();
            }
            else
            {
                DisableAnnotateLine();
            }
        }

        private void EnableAnnotateLine()
        {
            if (AnnotateLineWindow != null)
                return;
            Log.e("AL", "EnableAnnotateLineWindow");
            EnableMouseHook();
            AnnotateLineWindow = new AnnotateLine(myMouseHook, myKeystrokeConverter, mySettings);
            AnnotateLineWindow.Show();
            AnnotateLineWindow.Hide();
        }



        private void DisableAnnotateLine()
        {
            if (AnnotateLineWindow == null)
                return;
            AnnotateLineWindow.Close();
            AnnotateLineWindow = null;
            DisableMouseHookIfNotNeeded();
            Log.e("AL", "DisableAnnotateLineWindow");
        }

        #endregion

        #region Mouse Hook

        IMouseRawEventProvider myMouseHook = null;

        private void EnableMouseHook()
        {
            if (myMouseHook != null)
                return;
            myMouseHook = new MouseHook(mySettings);
        }

        private void DisableMouseHookIfNotNeeded()
        {
            if (CursorIndicatorWindow == null && ButtonIndicatorWindow == null && AnnotateLineWindow == null)
                DisableMouseHook();
        }

        private void DisableMouseHook()
        {
            if (myMouseHook == null)
                return;
            myMouseHook.Dispose();
            myMouseHook = null;
        }

        [System.Runtime.InteropServices.DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        public static extern bool DeleteObject([System.Runtime.InteropServices.In] IntPtr hObject);
        #endregion
    }
}
