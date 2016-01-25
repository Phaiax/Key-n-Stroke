using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PxKeystrokesUi
{

    #region Enums for some settings

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

    #endregion

    #region class SettingChangedEventArgs, delegate

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

    #endregion

    public class SettingsStore
    {

        #region init and reflection helpers

        public SettingsStore()
        {
            Properties = typeof(SettingsStore).GetProperties();
        }

        PropertyInfo[] Properties = { };

        private List<string> GetAllSettingNames()
        {
            List<string> L = new List<string>();
            foreach (var p in Properties)
            {
                L.AddRange(GetSettingNames(p));
            }
            return L;
        }
                
        private bool IsEnum(PropertyInfo p)
        {
            return p.PropertyType.BaseType.Name == "Enum";
        }

        private string GetLoweredSettingName(PropertyInfo p)
        {
            return p.Name.Substring(0, 1).ToLower() + p.Name.Substring(1);
        }

        private List<string> GetSettingNames(PropertyInfo p)
        {
            List<string> L = new List<string>();
            string prefix = GetLoweredSettingName(p);
            if (IsEnum(p))
            {
                L.Add(prefix);
                return L;
            }
            switch (p.PropertyType.Name)
            {
                case "Single": //float
                case "Color":
                case "String":
                case "Boolean":
                case "Double":
                case "Int32":
                    L.Add(prefix);
                    return L;
                case "Font":
                    L.Add(prefix + "-fontfamily");
                    L.Add(prefix + "-size");
                    L.Add(prefix + "-style");
                    return L;
                case "Size":
                    L.Add(prefix + "-height");
                    L.Add(prefix + "-width");
                    return L;
                case "Point":
                    L.Add(prefix + "-x");
                    L.Add(prefix + "-y");
                    return L;
                default:
                    throw new NotImplementedException("Type " + p.PropertyType.Name);
            }
        }

        #endregion


        #region Settings

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

        #endregion


        #region SettingsChangedEvent

        public SettingsChangedEventHandler settingChanged;

        private void OnSettingChanged(string name)
        {
            dirty = true;
            if (this.settingChanged != null)
            {
                this.settingChanged(new SettingsChangedEventArgs(name));
            }
        }

        public void OnSettingChangedAll()
        {
            foreach (PropertyInfo p in Properties)
            {
                OnSettingChanged(p.Name);
            }
            return;
        }

        #endregion


        #region SaveAll and LoadAll

        bool dirty = true;

        public void SaveAll()
        {
            if (dirty)
            {
                foreach (var p in Properties)
                {
                    string name = GetLoweredSettingName(p);
                    FieldInfo privateField = typeof(SettingsStore).GetField(name,
                        BindingFlags.NonPublic | BindingFlags.Instance);

                    string type = IsEnum(p) ? "Int32" : p.PropertyType.Name;
                    MethodInfo saveFunctionForType = typeof(SettingsStore).GetMethod("Save" + type,
                        BindingFlags.NonPublic | BindingFlags.Instance);
                    object userValue = privateField.GetValue(this);
                    saveFunctionForType.Invoke(this, new object[] { name, userValue });
                    Log.e("REFL", "store " + privateField.Name + " = " + userValue.ToString() + " to " + name + " by calling " + saveFunctionForType.Name);
                }
            }
            dirty = false;
        }

        public void LoadAll()
        {
            foreach (var p in Properties)
            {
                string name = GetLoweredSettingName(p);
                object def = typeof(SettingsStore).GetField(p.Name + "Default").GetValue(this);
                FieldInfo f = typeof(SettingsStore).GetField(name,
                    BindingFlags.NonPublic | BindingFlags.Instance);

                string type = IsEnum(p) ? "Int32" : p.PropertyType.Name;
                MethodInfo loadFunctionForType = typeof(SettingsStore).GetMethod("Load" + type,
                    BindingFlags.NonPublic | BindingFlags.Instance);
                object loadedValue = loadFunctionForType.Invoke(this, new object[] { name, def });
                f.SetValue(this, loadedValue);
                Log.e("REFL", f.Name + " = " + loadedValue.ToString() + ", def = " + def.ToString());   
            }

            dirty = true;
        }

        public void ClearAll()
        {
            foreach (var setting in GetAllSettingNames())
            {
                Application.UserAppDataRegistry.DeleteValue(setting, false);
            }
        }

        #endregion


        #region SaveType and LoadType

        // typenames must be typeOf(property).Name

        #region Font

        private void SaveFont(string name, Font value)
        {
            SaveString(name + "-fontfamily", value.Name);
            SaveDouble(name + "-size", (double)value.SizeInPoints);
            SaveInt32(name + "-style", (int)value.Style);
        }

        private Font LoadFont(string name, Font def)
        {
            try
            {
                return new Font(GetString(name + "-fontfamily", def.Name),
                                 (float) LoadDouble(name + "-size", def.SizeInPoints),
                                 (FontStyle) LoadInt32(name + "-style", (int) def.Style));
            }
            catch (Exception e)
            {
                return def;
            }
        }

        #endregion

        #region Point And Size

        private void SaveSize(string name, Size value)
        {
            SaveInt32(name + "-width", value.Width);
            SaveInt32(name + "-height", value.Height);
        }

        private Size LoadSize(string name, Size def)
        {
            try
            {
                return new Size(LoadInt32(name + "-width", def.Width),
                                 LoadInt32(name + "-height", def.Height));
            }
            catch (Exception e)
            {
                return def;
            }
        }

        private void SavePoint(string name, Point value)
        {
            SaveInt32(name + "-x", value.X);
            SaveInt32(name + "-y", value.Y);
        }

        private Point LoadPoint(string name, Point def)
        {
            try
            {
                return new Point(LoadInt32(name + "-x", def.X),
                                 LoadInt32(name + "-y", def.Y));
            }
            catch (Exception)
            {
                return def;
            }
        }

        #endregion

        #region Color

        private void SaveColor(string name, Color value)
        {
            SaveInt32(name, value.ToArgb());
        }

        private Color LoadColor(string name, Color def)
        {
            try
            {
                return Color.FromArgb(LoadInt32(name, def.ToArgb()));
            }
            catch (Exception)
            {
                return def;
            }
        }

        #endregion

        #region float aka Single and Double

        private void SaveSingle(string name, float value)
        {
            SaveDouble(name, (float) value);
        }

        private float LoadSingle(string name, float def)
        {
            return (float) LoadDouble(name, def);
        }


        private void SaveDouble(string name, double value)
        {
            SaveString(name, value.ToString());
        }

        private double LoadDouble(string name, double def)
        {
            try
            {
                return Convert.ToDouble(GetString(name, def.ToString()));
            }
            catch (Exception)
            {
                return def;
            }
        }

        #endregion 

        #region Int32

        private void SaveInt32(string name, int value)
        {
            SaveString(name, value.ToString());
        }

        private int LoadInt32(string name, int def)
        {
            try
            {
                return Convert.ToInt32(GetString(name, def.ToString()));
            }
            catch (Exception)
            {
                return def;
            }
        }

        #endregion

        #region Boolean

        private void SaveBoolean(string name, bool value)
        {
            SaveString(name, value ? "1" : "0");
        }

        private bool LoadBoolean(string name, bool def)
        {
            try
            {
                return GetString(name, def ? "1" : "0") == "0" ? false : true;
            }
            catch (Exception)
            {
                return def;
            }
        }

        #endregion

        #region String

        private void SaveString(string name, string value)
        {
            Application.UserAppDataRegistry.SetValue(name, value, RegistryValueKind.String);
        }

        private string GetString(string name, string def)
        {
            Log.e("SETTINGS", name + " " + (string)Application.UserAppDataRegistry.GetValue(name, def));
            return (string) Application.UserAppDataRegistry.GetValue(name, def);
        }

        #endregion

        #endregion
    }
}
