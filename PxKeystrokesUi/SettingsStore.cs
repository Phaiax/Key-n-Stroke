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
        public Font LabelFontDefault = new Font("Arial", 25);
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
        public float OpacityDefault = 0.9f;
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
        public Size WindowSizeDefault = new Size(284, 227);
        public Size WindowSize
        {
            get { return windowSize; }
            set { windowSize = value; OnSettingChanged("WindowSize"); }
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
                SaveColor("backgroundColor", backgroundColor);

                SaveDouble("opacity", opacity);

                SaveInt("labelTextAlignment", (int)labelTextAlignment);
                SaveInt("labelTextDirection", (int)labelTextDirection);
                SaveInt("labelAnimation", (int)labelAnimation);
                SaveInt("windowLocation-x", windowLocation.X);
                SaveInt("windowLocation-y", windowLocation.Y);
                SaveInt("windowSize-width", windowSize.Width);
                SaveInt("windowSize-height", windowSize.Height);

                System.Diagnostics.Debug.WriteLine(String.Format("Save X: {0}", windowLocation.X));

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

            System.Diagnostics.Debug.WriteLine(String.Format("Load X: {0}", x));
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

        private void SaveString(string name, string value)
        {
            Application.UserAppDataRegistry.SetValue(name, value, RegistryValueKind.String);
        }

        private string GetString(string name, string def)
        {
            return (string) Application.UserAppDataRegistry.GetValue(name, def);
        }
    }
}
