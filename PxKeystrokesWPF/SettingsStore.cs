using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
//using System.Windows.Forms;

namespace PxKeystrokesWPF
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

    #region Serializable classes for complex types

    interface IGet<T>
    {
        T Get();
    }

    [DataContract]
    public class SerializableFont : IGet<Font>
    {
        [DataMember] public string fontfamily;
        [DataMember] public double size;
        [DataMember] public int style;
        public SerializableFont(Font value)
        {
            fontfamily = value.Name;
            size = (double)value.SizeInPoints;
            style = (int)value.Style;
        }
        public Font Get()
        {
            return new Font(fontfamily, (float)size, (FontStyle)style);
        }
    }

    [DataContract]
    public class SerializablePoint : IGet<Point>
    {
        [DataMember] public int x;
        [DataMember] public int y;
        public SerializablePoint(Point value)
        {
            x = value.X;
            y = value.Y;
        }
        public Point Get()
        {
            return new Point(x, y);
        }
    }

    [DataContract]
    public class SerializableSize : IGet<Size>
    {
        [DataMember] public int width;
        [DataMember] public int height;
        public SerializableSize(Size value)
        {
            width = value.Width;
            height = value.Height;
        }
        public Size Get()
        {
            return new Size(width, height);
        }
    }

    [DataContract]
    public class SerializableColor : IGet<Color>
    {
        [DataMember] public int color;
        public SerializableColor(Color value)
        {
            color = value.ToArgb();
        }
        public Color Get()
        {
            return Color.FromArgb(color);
        }
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

    #region Serializable Settings

    [DataContract]
    class Settings
    {
        [DataMember] public SerializableFont labelFont;
        [DataMember] public SerializableColor textColor;
        [DataMember] public SerializableColor backgroundColor;
        [DataMember] public float opacity;
        [DataMember] public TextAlignent labelTextAlignment;
        [DataMember] public TextDirection labelTextDirection;
        [DataMember] public Style labelAnimation;
        [DataMember] public SerializablePoint windowLocation;
        [DataMember] public SerializableSize windowSize;
        [DataMember] public SerializablePoint panelLocation;
        [DataMember] public SerializableSize panelSize;
        [DataMember] public int lineDistance;
        [DataMember] public int historyLength;
        [DataMember] public int historyTimeout;
        [DataMember] public bool enableHistoryTimeout;
        [DataMember] public bool enableWindowFade;
        [DataMember] public bool enableCursorIndicator;
        [DataMember] public float cursorIndicatorOpacity;
        [DataMember] public SerializableSize cursorIndicatorSize;
        [DataMember] public SerializableColor cursorIndicatorColor;
        [DataMember] public ButtonIndicatorType buttonIndicator;
        [DataMember] public float buttonIndicatorSize;
        [DataMember] public float buttonIndicatorPositionAngle;
        [DataMember] public int buttonIndicatorPositionDistance;
        [DataMember] public bool addButtonEventsToHistory;
        [DataMember] public bool backspaceDeletesText;
    }

    #endregion

    public class SettingsStore
    {

        #region Constructor

        public SettingsStore()
        {
        }

        #endregion

        #region Helpers: Or(a,b)

        private T Or<T>(IGet<T> a, T b)
        {
            if (a != null) return a.Get();
            return b;
        }

        private T Or<T>(T a, T b)
        {
            if (a != null) return a;
            return b;
        }

        #endregion

        #region Settings Accessors

        Settings i = new Settings();

        public Font LabelFontDefault = new Font("Open Sans", 14);
        public Font LabelFont
        {
            get { return Or(i.labelFont, LabelFontDefault); }
            set { i.labelFont = new SerializableFont(value); OnSettingChanged("LabelFont"); }
        }

        public Color TextColorDefault = Color.White;
        public Color TextColor
        {
            get { return Or(i.textColor, TextColorDefault); }
            set { i.textColor = new SerializableColor(value); OnSettingChanged("TextColor"); }
        }

        public Color BackgroundColorDefault = Color.Black;
        public Color BackgroundColor
        {
            get { return Or(i.backgroundColor, BackgroundColorDefault); }
            set { i.backgroundColor = new SerializableColor(value); OnSettingChanged("BackgroundColor"); }
        }

        public float OpacityDefault = 0.78f;
        public float Opacity
        {
            get { return Or(i.opacity, OpacityDefault); }
            set { i.opacity = value; OnSettingChanged("Opacity"); }
        }

        public TextAlignent LabelTextAlignmentDefault = TextAlignent.Left;
        public TextAlignent LabelTextAlignment
        {
            get { return Or(i.labelTextAlignment, LabelTextAlignmentDefault); }
            set { i.labelTextAlignment = value; OnSettingChanged("LabelTextAlignment"); }
        }

        public TextDirection LabelTextDirectionDefault = TextDirection.Down;
        public TextDirection LabelTextDirection
        {
            get { return Or(i.labelTextDirection, LabelTextDirectionDefault); }
            set { i.labelTextDirection = value; OnSettingChanged("LabelTextDirection"); }
        }

        public Style LabelAnimationDefault = Style.Slide;
        public Style LabelAnimation
        {
            get { return Or(i.labelAnimation, LabelAnimationDefault); }
            set { i.labelAnimation = value; OnSettingChanged("LabelAnimation"); }
        }

        public Point WindowLocationDefault = new Point(100, 100);
        public Point WindowLocation
        {
            get { return Or(i.windowLocation, WindowLocationDefault); }
            set { i.windowLocation = new SerializablePoint(value); OnSettingChanged("WindowLocation"); }
        }

        public Size WindowSizeDefault = new Size(316, 193);
        public Size WindowSize
        {
            get { return Or(i.windowSize, WindowSizeDefault); }
            set { i.windowSize = new SerializableSize(value); OnSettingChanged("WindowSize"); }
        }

        public Point PanelLocationDefault = new Point(50, 11);
        public Point PanelLocation
        {
            get { return Or(i.panelLocation, PanelLocationDefault); }
            set { i.panelLocation = new SerializablePoint(value); OnSettingChanged("PanelLocation"); }
        }

        public Size PanelSizeDefault = new Size(226, 135);
        public Size PanelSize
        {
            get { return Or(i.panelSize, PanelSizeDefault); }
            set { i.panelSize = new SerializableSize(value); OnSettingChanged("PanelSize"); }
        }

        public int LineDistanceDefault = 36;
        public int LineDistance
        {
            get { return Or(i.lineDistance, LineDistanceDefault); }
            set { i.lineDistance = value; OnSettingChanged("LineDistance"); }
        }

        public int HistoryLengthDefault = 4;
        public int HistoryLength
        {
            get { return Or(i.historyLength, HistoryLengthDefault); }
            set { i.historyLength = value; OnSettingChanged("HistoryLength"); }
        }

        public int HistoryTimeoutDefault = 10000; // ms
        public int HistoryTimeout
        {
            get { return Or(i.historyTimeout, HistoryTimeoutDefault); }
            set { i.historyTimeout = value; OnSettingChanged("HistoryTimeout"); }
        }

        public bool EnableHistoryTimeoutDefault = true;
        public bool EnableHistoryTimeout
        {
            get { return Or(i.enableHistoryTimeout, EnableHistoryTimeoutDefault); }
            set { i.enableHistoryTimeout = value; OnSettingChanged("EnableHistoryTimeout"); }
        }

        public bool EnableWindowFadeDefault = true;
        public bool EnableWindowFade
        {
            get { return Or(i.enableWindowFade, EnableWindowFadeDefault); }
            set { i.enableWindowFade = value; OnSettingChanged("EnableWindowFade"); }
        }

        public bool EnableCursorIndicatorDefault = true;
        public bool EnableCursorIndicator
        {
            get { return Or(i.enableCursorIndicator, EnableCursorIndicatorDefault); }
            set { i.enableCursorIndicator = value; OnSettingChanged("EnableCursorIndicator"); }
        }

        public float CursorIndicatorOpacityDefault = 0.3f;
        public float CursorIndicatorOpacity
        {
            get { return Or(i.cursorIndicatorOpacity, CursorIndicatorOpacityDefault); }
            set { i.cursorIndicatorOpacity = value; OnSettingChanged("CursorIndicatorOpacity"); }
        }

        public Size CursorIndicatorSizeDefault = new Size(55, 55);
        public Size CursorIndicatorSize
        {
            get { return Or(i.cursorIndicatorSize, CursorIndicatorSizeDefault); }
            set { i.cursorIndicatorSize = new SerializableSize(value); OnSettingChanged("CursorIndicatorSize"); }
        }

        public Color CursorIndicatorColorDefault = Color.FromArgb(-32640);
        public Color CursorIndicatorColor
        {
            get { return Or(i.cursorIndicatorColor, CursorIndicatorColorDefault); }
            set { i.cursorIndicatorColor = new SerializableColor(value); OnSettingChanged("CursorIndicatorColor"); }
        }

        public ButtonIndicatorType ButtonIndicatorDefault = ButtonIndicatorType.PicsAroundCursor;
        public ButtonIndicatorType ButtonIndicator
        {
            get { return Or(i.buttonIndicator, ButtonIndicatorDefault); }
            set { i.buttonIndicator = value; OnSettingChanged("ButtonIndicator"); }
        }

        public float ButtonIndicatorSizeDefault = 0.32f;
        public float ButtonIndicatorSize
        {
            get { return Or(i.buttonIndicatorSize, ButtonIndicatorSizeDefault); }
            set { i.buttonIndicatorSize = value; OnSettingChanged("ButtonIndicatorSize"); }
        }

        public float ButtonIndicatorPositionAngleDefault = 0f;
        public float ButtonIndicatorPositionAngle
        {
            get { return Or(i.buttonIndicatorPositionAngle, ButtonIndicatorPositionAngleDefault); }
            set { i.buttonIndicatorPositionAngle = value; OnSettingChanged("ButtonIndicatorPositionAngle"); }
        }

        public int ButtonIndicatorPositionDistanceDefault = 56;
        public int ButtonIndicatorPositionDistance
        {
            get { return Or(i.buttonIndicatorPositionDistance, ButtonIndicatorPositionDistanceDefault); }
            set { i.buttonIndicatorPositionDistance = value; OnSettingChanged("ButtonIndicatorPositionDistance"); }
        }

        public bool AddButtonEventsToHistoryDefault = false;
        public bool AddButtonEventsToHistory
        {
            get { return Or(i.addButtonEventsToHistory, AddButtonEventsToHistoryDefault); }
            set { i.addButtonEventsToHistory = value; OnSettingChanged("AddButtonEventsToHistory"); }
        }

        public bool BackspaceDeletesTextDefault = true;
        public bool BackspaceDeletesText
        {
            get { return Or(i.backspaceDeletesText, BackspaceDeletesTextDefault); }
            set { i.backspaceDeletesText = value; OnSettingChanged("BackspaceDeletesText"); }
        }

        // Add new settings also to method OnSettingsChangedAll()

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
            OnSettingChanged("EnableWindowFade");
            OnSettingChanged("EnableCursorIndicator");
            OnSettingChanged("CursorIndicatorOpacity");
            OnSettingChanged("CursorIndicatorSize");
            OnSettingChanged("CursorIndicatorColor");
            OnSettingChanged("ButtonIndicator");
            OnSettingChanged("ButtonIndicatorSize");
            OnSettingChanged("ButtonIndicatorPositionAngle");
            OnSettingChanged("ButtonIndicatorPositionDistance");
            OnSettingChanged("AddButtonEventsToHistory");
            OnSettingChanged("BackspaceDeletesText");
        }

        #endregion

        #region SaveAll and LoadAll

        const string ISOLATED_STORAGE_FILE_NAME = "settings";

        bool dirty = true;

        public void SaveAll()
        {
            if (dirty)
            {
                try
                {
                    using (IsolatedStorageFile store =
                            IsolatedStorageFile.GetUserStoreForAssembly())
                    {
                        using (IsolatedStorageFileStream stream =
                                new IsolatedStorageFileStream(ISOLATED_STORAGE_FILE_NAME,
                                                              FileMode.Create,
                                                              store))
                        {
                            DataContractJsonSerializer ser =
                                new DataContractJsonSerializer(typeof(Settings));

                            ser.WriteObject(stream, i);
                        }
                    }
                }
                catch (System.Security.SecurityException sx)
                {
                    System.Windows.MessageBox.Show(sx.Message);
                    throw;
                }
            }
            dirty = false;
        }

        public void LoadAll()
        {
            try
            {
                using (IsolatedStorageFile store =
                        IsolatedStorageFile.GetUserStoreForAssembly())
                {
                    if (store.FileExists(ISOLATED_STORAGE_FILE_NAME))
                    {
                        using (IsolatedStorageFileStream stream =
                                  store.OpenFile(ISOLATED_STORAGE_FILE_NAME, FileMode.Open))
                        {
                            DataContractJsonSerializer ser =
                                new DataContractJsonSerializer(typeof(Settings));

                            i = (Settings) ser.ReadObject(stream);
                        }

                    }
                }
            }
            catch (System.Security.SecurityException sx)
            {
                System.Windows.MessageBox.Show(sx.Message);
                throw;
            }
            dirty = true;
        }

        public void ClearAll()
        {
            i = new Settings();
        }

        #endregion


    }
}
