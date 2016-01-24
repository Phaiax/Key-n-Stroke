using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PxKeystrokesUi
{

    public enum TextAlignent
    {
        Left,
        Right,
        Center
    }

    public enum TextDirection
    {
        Up,
        Down
    }

    public enum Style
    {
        NoAnimation,
        Slide
    }

    public enum ButtonIndicatorType
    {
        Disabled,
        PicsAroundCursor
    }

    public delegate void SettingsChangedEventHandler(SettingsChangedEventArgs e);

    public class SettingsChangedEventArgs
    {
        private string name;
        public string Name
        {
            get { return name; }
        }

        public SettingsChangedEventArgs(string name)
        {
            this.name = name;
        }
    }

    public class SettingsStore
    {
        private Font labelFont;
        public Font LabelFontDefault = new Font("Open Sans", 14);
        public Font LabelFont
        {
            get { return labelFont; }
            set { labelFont = value; OnSettingChanged("LabelFont"); }
        }

        private Color textColor;
        public Color TextColorDefault = Color.White;
        public Color TextColor
        {
            get { return textColor; }
            set { textColor = value; OnSettingChanged("TextColor"); }
        }

        private Color backgroundColor;
        public Color BackgroundColorDefault = Color.Black;
        public Color BackgroundColor
        {
            get { return backgroundColor; }
            set { backgroundColor = value; OnSettingChanged("BackgroundColor"); }
        }

        private float opacity;
        public float OpacityDefault = 0.78f;
        public float Opacity
        {
            get { return opacity; }
            set { opacity = value; OnSettingChanged("Opacity"); }
        }

        private TextAlignent labelTextAlignment;
        public TextAlignent LabelTextAlignmentDefault = TextAlignent.Left;
        public TextAlignent LabelTextAlignment
        {
            get { return labelTextAlignment; }
            set { labelTextAlignment = value; OnSettingChanged("LabelTextAlignment"); }
        }

        private TextDirection labelTextDirection;
        public TextDirection LabelTextDirectionDefault = TextDirection.Down;
        public TextDirection LabelTextDirection
        {
            get { return labelTextDirection; }
            set { labelTextDirection = value; OnSettingChanged("LabelTextDirection"); }
        }

        private Style labelAnimation;
        public Style LabelAnimationDefault = Style.Slide;
        public Style LabelAnimation
        {
            get { return labelAnimation; }
            set { labelAnimation = value; OnSettingChanged("LabelAnimation"); }
        }

        private Point windowLocation;
        public Point WindowLocationDefault = new Point(100, 100);
        public Point WindowLocation
        {
            get { return windowLocation; }
            set { windowLocation = value; OnSettingChanged("WindowLocation"); }
        }

        private Size windowSize;
        public Size WindowSizeDefault = new Size(316, 193);
        public Size WindowSize
        {
            get { return windowSize; }
            set { windowSize = value; OnSettingChanged("WindowSize"); }
        }

        private Point panelLocation;
        public Point PanelLocationDefault = new Point(50, 11);
        public Point PanelLocation
        {
            get { return panelLocation; }
            set { panelLocation = value; OnSettingChanged("PanelLocation"); }
        }

        private Size panelSize;
        public Size PanelSizeDefault = new Size(226, 135);
        public Size PanelSize
        {
            get { return panelSize; }
            set { panelSize = value; OnSettingChanged("PanelSize"); }
        }

        private int lineDistance;
        public int LineDistanceDefault = 36;
        public int LineDistance
        {
            get { return lineDistance; }
            set { lineDistance = value; OnSettingChanged("LineDistance"); }
        }

        private int historyLength;
        public int HistoryLengthDefault = 4;
        public int HistoryLength
        {
            get { return historyLength; }
            set { historyLength = value; OnSettingChanged("HistoryLength"); }
        }

        private int historyTimeout;
        public int HistoryTimeoutDefault = 10000; // ms
        public int HistoryTimeout
        {
            get { return historyTimeout; }
            set { historyTimeout = value; OnSettingChanged("HistoryTimeout"); }
        }

        private bool enableHistoryTimeout;
        public bool EnableHistoryTimeoutDefault = true;
        public bool EnableHistoryTimeout
        {
            get { return enableHistoryTimeout; }
            set { enableHistoryTimeout = value; OnSettingChanged("EnableHistoryTimeout"); }
        }

        private bool enableCursorIndicator;
        public bool EnableCursorIndicatorDefault = true;
        public bool EnableCursorIndicator
        {
            get { return enableCursorIndicator; }
            set { enableCursorIndicator = value; OnSettingChanged("EnableCursorIndicator"); }
        }

        private float cursorIndicatorOpacity;
        public float CursorIndicatorOpacityDefault = 0.3f;
        public float CursorIndicatorOpacity
        {
            get { return cursorIndicatorOpacity; }
            set { cursorIndicatorOpacity = value; OnSettingChanged("CursorIndicatorOpacity"); }
        }

        private Size cursorIndicatorSize;
        public Size CursorIndicatorSizeDefault = new Size(55, 55);
        public Size CursorIndicatorSize
        {
            get { return cursorIndicatorSize; }
            set { cursorIndicatorSize = value; OnSettingChanged("CursorIndicatorSize"); }
        }

        private Color cursorIndicatorColor;
        public Color CursorIndicatorColorDefault = Color.FromArgb(-32640);
        public Color CursorIndicatorColor
        {
            get { return cursorIndicatorColor; }
            set { cursorIndicatorColor = value; OnSettingChanged("CursorIndicatorColor"); }
        }

        private ButtonIndicatorType buttonIndicator;
        public ButtonIndicatorType ButtonIndicatorDefault = ButtonIndicatorType.PicsAroundCursor;
        public ButtonIndicatorType ButtonIndicator
        {
            get { return buttonIndicator; }
            set { buttonIndicator = value; OnSettingChanged("ButtonIndicator"); }
        }

        private float buttonIndicatorSize;
        public float ButtonIndicatorSizeDefault = 0.32f;
        public float ButtonIndicatorSize
        {
            get { return buttonIndicatorSize; }
            set { buttonIndicatorSize = value; OnSettingChanged("ButtonIndicatorSize"); }
        }

        private float buttonIndicatorPositionAngle;
        public float ButtonIndicatorPositionAngleDefault = 0f;
        public float ButtonIndicatorPositionAngle
        {
            get { return buttonIndicatorPositionAngle; }
            set { buttonIndicatorPositionAngle = value; OnSettingChanged("ButtonIndicatorPositionAngle"); }
        }

        private int buttonIndicatorPositionDistance;
        public int ButtonIndicatorPositionDistanceDefault = 56;
        public int ButtonIndicatorPositionDistance
        {
            get { return buttonIndicatorPositionDistance; }
            set { buttonIndicatorPositionDistance = value; OnSettingChanged("ButtonIndicatorPositionDistance"); }
        }

        private bool addButtonEventsToHistory;
        public bool AddButtonEventsToHistoryDefault = false;
        public bool AddButtonEventsToHistory
        {
            get { return addButtonEventsToHistory; }
            set { addButtonEventsToHistory = value; OnSettingChanged("AddButtonEventsToHistory"); }
        }

        private bool backspaceDeletesText;
        public bool BackspaceDeletesTextDefault = true;
        public bool BackspaceDeletesText
        {
            get { return backspaceDeletesText; }
            set { backspaceDeletesText = value; OnSettingChanged("BackspaceDeletesText"); }
        }


        public SettingsChangedEventHandler settingChanged;
        private void OnSettingChanged(string name)
        {
            dirty = true;
            if (this.settingChanged != null)
            {
                this.settingChanged(new SettingsChangedEventArgs(name));
            }
        }



        bool dirty = true;

        public void SaveAll()
        {
            if (dirty)
            {
                SaveString("labelFont-fontfamily", labelFont.Name);
                SaveDouble("labelFont-size", (double)labelFont.SizeInPoints);
                SaveInt("labelFont-style", (int)labelFont.Style);

                SaveColor("textColor", textColor);
                SaveDouble("cursorIndicatorOpacity", cursorIndicatorOpacity);

                SaveDouble("opacity", opacity);

                SaveInt("labelTextAlignment", (int)labelTextAlignment);
                SaveInt("labelTextDirection", (int)labelTextDirection);
                SaveInt("labelAnimation", (int)labelAnimation);
                SaveInt("windowLocation-x", windowLocation.X);
                SaveInt("windowLocation-y", windowLocation.Y);
                SaveInt("windowSize-width", windowSize.Width);
                SaveInt("windowSize-height", windowSize.Height);

                SaveInt("panelLocation-x", panelLocation.X);
                SaveInt("panelLocation-y", panelLocation.Y);
                SaveInt("panelSize-width", panelSize.Width);
                SaveInt("panelSize-height", panelSize.Height);

                SaveInt("lineDistance", lineDistance);
                SaveInt("historyLength", historyLength);
                SaveInt("historyTimeout", historyTimeout);
                SaveBool("enableHistoryTimeout", enableHistoryTimeout);

                SaveBool("enableCursorIndicator", enableCursorIndicator);
                SaveDouble("cursorIndicatorOpacity", cursorIndicatorOpacity);
                SaveColor("cursorIndicatorColor", cursorIndicatorColor);
                SaveInt("cursorIndicatorSize-width", cursorIndicatorSize.Width);
                SaveInt("cursorIndicatorSize-height", cursorIndicatorSize.Height);
                SaveInt("buttonIndicator", (int) buttonIndicator);
                SaveDouble("buttonIndicatorPositionAngle", buttonIndicatorPositionAngle);
                SaveInt("buttonIndicatorPositionDistance", buttonIndicatorPositionDistance);
                SaveDouble("buttonIndicatorSize", buttonIndicatorSize);
                SaveBool("addButtonEventsToHistoryDefault", addButtonEventsToHistory);

                SaveBool("backspaceDeletesText", backspaceDeletesText);
            }
            dirty = false;
        }

        public void LoadAll()
        {
            labelFont = new Font(GetString("labelFont-fontfamily", LabelFontDefault.Name),
                                 (float) GetDouble("labelFont-size", LabelFontDefault.SizeInPoints),
                                 (FontStyle) GetInt("labelFont-style", (int)LabelFontDefault.Style));

            textColor = GetColor("textColor", TextColorDefault);
            backgroundColor = GetColor("backgroundColor", BackgroundColorDefault);

            opacity = (float) GetDouble("opacity", OpacityDefault);

            labelTextAlignment = (TextAlignent) GetInt("labelTextAlignment", (int) LabelTextAlignmentDefault);
            labelTextDirection = (TextDirection)GetInt("labelTextDirection", (int)LabelTextDirectionDefault);
            labelAnimation = (Style)GetInt("labelAnimation", (int)LabelAnimationDefault);

            int x = GetInt("windowLocation-x", WindowLocationDefault.X);
            int y = GetInt("windowLocation-y", WindowLocationDefault.Y);
            windowLocation = new Point(x, y);
            int width = GetInt("windowSize-width", WindowSizeDefault.Width);
            int height = GetInt("windowSize-height", WindowSizeDefault.Height);
            windowSize = new Size(width, height);

            x = GetInt("panelLocation-x", PanelLocationDefault.X);
            y = GetInt("panelLocation-y", PanelLocationDefault.Y);
            panelLocation = new Point(x, y);
            width = GetInt("panelSize-width", PanelSizeDefault.Width);
            height = GetInt("panelSize-height", PanelSizeDefault.Height);
            panelSize = new Size(width, height);

            lineDistance = GetInt("lineDistance", LineDistanceDefault);
            historyLength = GetInt("historyLength", HistoryLengthDefault);
            historyTimeout = GetInt("historyTimeout", HistoryTimeoutDefault);
            enableHistoryTimeout = GetBool("enableHistoryTimeout", EnableHistoryTimeoutDefault);

            enableCursorIndicator = GetBool("enableCursorIndicator", EnableCursorIndicatorDefault);
            cursorIndicatorOpacity = (float) GetDouble("cursorIndicatorOpacity", CursorIndicatorOpacityDefault);
            CursorIndicatorColor = GetColor("cursorIndicatorColor", CursorIndicatorColorDefault);
            width = GetInt("cursorIndicatorSize-width", CursorIndicatorSizeDefault.Width);
            height = GetInt("cursorIndicatorSize-height", CursorIndicatorSizeDefault.Height);
            cursorIndicatorSize = new Size(width, height);
            
            buttonIndicator = (ButtonIndicatorType)GetInt("buttonIndicator", (int)ButtonIndicatorDefault);
            buttonIndicatorPositionAngle = (float)GetDouble("buttonIndicatorPositionAngle", ButtonIndicatorPositionAngleDefault);
            buttonIndicatorPositionDistance = GetInt("buttonIndicatorPositionDistance", ButtonIndicatorPositionDistanceDefault);
            buttonIndicatorSize = (float) GetDouble("buttonIndicatorSize", ButtonIndicatorSizeDefault);
            addButtonEventsToHistory = GetBool("addButtonEventsToHistoryDefault", AddButtonEventsToHistoryDefault);

            backspaceDeletesText = GetBool("backspaceDeletesText", BackspaceDeletesTextDefault);
            
            dirty = true;
        }

        public void ClearAll()
        {
            Application.UserAppDataRegistry.DeleteValue("labelFont-fontfamily", false);
            Application.UserAppDataRegistry.DeleteValue("labelFont-size", false);
            Application.UserAppDataRegistry.DeleteValue("labelFont-style", false);
            Application.UserAppDataRegistry.DeleteValue("textColor", false);
            Application.UserAppDataRegistry.DeleteValue("backgroundColor", false);
            Application.UserAppDataRegistry.DeleteValue("opacity", false);
            Application.UserAppDataRegistry.DeleteValue("labelTextAlignment", false);
            Application.UserAppDataRegistry.DeleteValue("labelTextDirection", false);
            Application.UserAppDataRegistry.DeleteValue("labelAnimation", false);
            Application.UserAppDataRegistry.DeleteValue("windowLocation-x", false);
            Application.UserAppDataRegistry.DeleteValue("windowLocation-y", false);
            Application.UserAppDataRegistry.DeleteValue("windowSize-width", false);
            Application.UserAppDataRegistry.DeleteValue("windowSize-height", false);
            Application.UserAppDataRegistry.DeleteValue("panelLocation-x", false);
            Application.UserAppDataRegistry.DeleteValue("panelLocation-y", false);
            Application.UserAppDataRegistry.DeleteValue("panelSize-width", false);
            Application.UserAppDataRegistry.DeleteValue("panelSize-height", false);
            Application.UserAppDataRegistry.DeleteValue("lineDistance", false);
            Application.UserAppDataRegistry.DeleteValue("historyLength", false);
            Application.UserAppDataRegistry.DeleteValue("historyTimeout", false);
            Application.UserAppDataRegistry.DeleteValue("enableHistoryTimeout", false);
            Application.UserAppDataRegistry.DeleteValue("enableCursorIndicator", false);
            Application.UserAppDataRegistry.DeleteValue("cursorIndicatorOpacity", false);
            Application.UserAppDataRegistry.DeleteValue("cursorIndicatorColor", false);
            Application.UserAppDataRegistry.DeleteValue("cursorIndicatorSize-width", false);
            Application.UserAppDataRegistry.DeleteValue("cursorIndicatorSize-height", false);
            Application.UserAppDataRegistry.DeleteValue("buttonIndicator", false);
            Application.UserAppDataRegistry.DeleteValue("buttonIndicatorPositionAngle", false);
            Application.UserAppDataRegistry.DeleteValue("buttonIndicatorPositionDistance", false);
            Application.UserAppDataRegistry.DeleteValue("buttonIndicatorSize", false);
            Application.UserAppDataRegistry.DeleteValue("addButtonEventsToHistory", false);
            Application.UserAppDataRegistry.DeleteValue("backspaceDeletesText", false);
        }

        public void OnSettingChangedAll()
        {
            OnSettingChanged("LabelFont");
            OnSettingChanged("TextColor");
            OnSettingChanged("BackgroundColor");
            OnSettingChanged("Opacity");
            OnSettingChanged("LabelTextAlignment");
            OnSettingChanged("LabelTextDirection");
            OnSettingChanged("LabelAnimation");
            OnSettingChanged("WindowLocation");
            OnSettingChanged("WindowSize");
            OnSettingChanged("PanelLocation");
            OnSettingChanged("PanelSize");
            OnSettingChanged("LineDistance");
            OnSettingChanged("HistoryLength");
            OnSettingChanged("HistoryTimeout");
            OnSettingChanged("EnableHistoryTimeout");
            OnSettingChanged("EnableCursorIndicator");
            OnSettingChanged("CursorIndicatorOpacity");
            OnSettingChanged("CursorIndicatorSize");
            OnSettingChanged("CursorIndicatorColor");
            OnSettingChanged("ButtonIndicator");
            OnSettingChanged("ButtonIndicatorPositionAngle");
            OnSettingChanged("ButtonIndicatorPositionDistance");
            OnSettingChanged("ButtonIndicatorSize");
            OnSettingChanged("AddButtonEventsToHistory");
            OnSettingChanged("BackspaceDeletesText");
        }

        private void SaveColor(string name, Color value)
        {
            SaveInt(name, value.ToArgb());
        }

        private Color GetColor(string name, Color def)
        {
            try
            {
                return Color.FromArgb(GetInt(name, def.ToArgb()));
            }
            catch (Exception e)
            {
                return def;
            }
        }

        private void SaveDouble(string name, double value)
        {
            SaveString(name, value.ToString());
        }

        private double GetDouble(string name, double def)
        {
            try
            {
                return Convert.ToDouble(GetString(name, def.ToString()));
            }
            catch (Exception e)
            {
                return def;
            }
        }

        private void SaveInt(string name, int value)
        {
            SaveString(name, value.ToString());
        }

        private int GetInt(string name, int def)
        {
            try
            {
                return Convert.ToInt32(GetString(name, def.ToString()));
            }
            catch (Exception e)
            {
                return def;
            }
        }

        private void SaveBool(string name, bool value)
        {
            SaveString(name, value ? "1" : "0");
        }

        private bool GetBool(string name, bool def)
        {
            try
            {
                return GetString(name, def ? "1" : "0") == "0" ? false : true;
            }
            catch (Exception e)
            {
                return def;
            }
        }

        private void SaveString(string name, string value)
        {
            Application.UserAppDataRegistry.SetValue(name, value, RegistryValueKind.String);
        }

        private string GetString(string name, string def)
        {
            Log.e("SETTINGS", name + " " + (string)Application.UserAppDataRegistry.GetValue(name, def));
            return (string) Application.UserAppDataRegistry.GetValue(name, def);
        }
    }
}
