using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using WpfColorFontDialog;

namespace PxKeystrokesWPF
{

    #region Enums for some settings

    public enum TextAlignment
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
    public class SerializableFont : IGet<FontInfo>
    {
        [DataMember] public string family;
        [DataMember] public SerializableColor color;
        [DataMember] public double size;
        [DataMember] public string style;
        [DataMember] public int stretch;
        [DataMember] public int weight;

        public SerializableFont(FontInfo value)
        {
            color = new SerializableColor(value.Color.Brush.Color);
            family = value.Family.Source;
            size = value.Size;
            stretch = value.Stretch.ToOpenTypeStretch();
            if (value.Style == System.Windows.FontStyles.Italic) style = "Italic";
            else if (value.Style == System.Windows.FontStyles.Oblique) style = "Oblique";
            else style = "Normal";
            weight = value.Weight.ToOpenTypeWeight();

        }
        public FontInfo Get()
        {
            System.Windows.FontStyle style = System.Windows.FontStyles.Normal;
            if (this.style == "Italic") style = System.Windows.FontStyles.Italic;
            if (this.style == "Oblique") style = System.Windows.FontStyles.Oblique;

            return new FontInfo(new System.Windows.Media.FontFamily(family),
                                size,
                                style,
                                System.Windows.FontStretch.FromOpenTypeStretch(stretch),
                                System.Windows.FontWeight.FromOpenTypeWeight(weight),
                                new System.Windows.Media.SolidColorBrush(color.Get()));
        }
        public override string ToString()
        {
            return $"{family}, {size}, {style}, stretch={stretch}, weight={weight}, #{color.r:X2}{color.g:X2}{color.b:X2}";
        }
    }

    [DataContract]
    public class SerializablePoint : IGet<System.Windows.Point>
    {
        [DataMember] public double x;
        [DataMember] public double y;
        public SerializablePoint(System.Windows.Point value)
        {
            x = value.X;
            y = value.Y;
        }
        public System.Windows.Point Get()
        {
            return new System.Windows.Point(x, y);
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
    public class SerializableColor : IGet<System.Windows.Media.Color>
    {
        [DataMember] public byte a;
        [DataMember] public byte r;
        [DataMember] public byte g;
        [DataMember] public byte b;

        public SerializableColor(System.Windows.Media.Color value)
        {
            a = value.A;
            r = value.R;
            g = value.G;
            b = value.B;
        }
        public System.Windows.Media.Color Get()
        {
            return System.Windows.Media.Color.FromArgb(a, r, g, b);
        }
    }

    [DataContract]
    public class SerializableColor2 : IGet<Color>
    {
        [DataMember] public int color;
        public SerializableColor2(Color value)
        {
            color = value.ToArgb();
        }
        public Color Get()
        {
            return Color.FromArgb(color);
        }
    }

    #endregion

    #region Serializable Settings

    [DataContract]
    class Settings
    {
        [DataMember] public SerializableFont labelFont = null;
        [DataMember] public SerializableColor2 labelColor = null;
        [DataMember] public SerializableColor2 backgroundColor = null;
        [DataMember] public Nullable<double> opacity = null;
        [DataMember] public Nullable<TextAlignment> labelTextAlignment = null;
        [DataMember] public Nullable<TextDirection> labelTextDirection = null;
        [DataMember] public Nullable<Style> labelAnimation = null;
        [DataMember] public SerializablePoint windowLocation = null;
        [DataMember] public SerializableSize windowSize = null;
        [DataMember] public SerializablePoint panelLocation = null;
        [DataMember] public SerializableSize panelSize = null;
        [DataMember] public Nullable<double> lineDistance = null;
        [DataMember] public Nullable<int> historyLength = null;
        [DataMember] public Nullable<double> historyTimeout = null;
        [DataMember] public Nullable<bool> enableHistoryTimeout = null;
        [DataMember] public Nullable<bool> enableWindowFade = null;
        [DataMember] public Nullable<bool> enableCursorIndicator = null;
        [DataMember] public Nullable<double> cursorIndicatorOpacity = null;
        [DataMember] public Nullable<double> cursorIndicatorSize = null;
        [DataMember] public SerializableColor2 cursorIndicatorColor = null;
        [DataMember] public Nullable<ButtonIndicatorType> buttonIndicator = null;
        [DataMember] public Nullable<double> buttonIndicatorScaling = null;
        [DataMember] public Nullable<double> buttonIndicatorPositionAngle = null;
        [DataMember] public Nullable<double> buttonIndicatorPositionDistance = null;
        [DataMember] public Nullable<bool> addButtonEventsToHistory = null;
        [DataMember] public Nullable<bool> backspaceDeletesText = null;
        [DataMember] public Nullable<bool> periodicTopmost = null;
        [DataMember] public Nullable<bool> enableKeystrokeHistory = null;
        [DataMember] public String keystrokeHistorySettingsModeShortcut = null;
        [DataMember] public Nullable<bool> enableSettingsMode = null;
    }

    #endregion

    public class SettingsStore : INotifyPropertyChanged
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

        private T Or<T>(Nullable<T> a, T b) where T : struct
        {
            if (a.HasValue) return a.Value;
            return b;
        }

        private T Or<T>(T a, T b) where T : class
        {
            if (a != null) return a;
            return b;
        }


        #endregion

        #region Settings Accessors

        Settings i = new Settings();

        public FontInfo LabelFontDefault = new FontInfo(new System.Windows.Media.FontFamily("Calibri"),
                                                        14.0,
                                                        System.Windows.FontStyles.Normal,
                                                        System.Windows.FontStretch.FromOpenTypeStretch(5),
                                                        System.Windows.FontWeight.FromOpenTypeWeight(400),
                                                        new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(255,255,255)));
        // Note: The color from the FontInfo can't be used. If we use a non-pallete (a color without a Name), then
        // The .Color item will read as null and the WpfColorFontDialog is unable to work with this FontInfo
        // So always keep the color in a safe white.
        public FontInfo LabelFont
        {
            get { return Or(i.labelFont, LabelFontDefault); }
            set { i.labelFont = new SerializableFont(value); OnSettingChanged("LabelFont"); }
        }

        public Color LabelColorDefault = Color.White;
        public Color LabelColor
        {
            get { return Or(i.labelColor, LabelColorDefault); }
            set { i.labelColor = new SerializableColor2(value); OnSettingChanged("LabelColor"); }
        }


        public Color BackgroundColorDefault = Color.FromArgb((int)(0.78*255), 0, 0, 0);
        public Color BackgroundColor
        {
            get { return Or(i.backgroundColor, BackgroundColorDefault); }
            set { i.backgroundColor = new SerializableColor2(value); OnSettingChanged("BackgroundColor"); }
        }

        public double OpacityDefault = 0.78f;
        // replaced by Background Color Alpha channel
        public double Opacity
        {
            get { return Or(i.opacity, OpacityDefault); }
            set { i.opacity = value; OnSettingChanged("Opacity"); }
        }

        public TextAlignment LabelTextAlignmentDefault = TextAlignment.Left;
        public TextAlignment LabelTextAlignment
        {
            get { return Or(i.labelTextAlignment, LabelTextAlignmentDefault); }
            set { i.labelTextAlignment = value; OnSettingChanged("LabelTextAlignment"); }
        }

        public TextDirection LabelTextDirectionDefault = TextDirection.Up;
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

        public System.Windows.Point WindowLocationDefault = new System.Windows.Point(100, 100);
        public System.Windows.Point WindowLocation
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

        public void SetWindowSizeWithoutOnSettingChangedEvent(Size value)
        {
            i.windowSize = new SerializableSize(value);
            dirty = true;
        }

        public System.Windows.Point PanelLocationDefault = new System.Windows.Point(50, 11);
        public System.Windows.Point PanelLocation
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

        public void SetPanelSizeWithoutOnSettingChangedEvent(Size value)
        {
            i.panelSize = new SerializableSize(value);
            dirty = true;
        }

        public double LineDistanceDefault = 36;
        public double LineDistance
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

        public double HistoryTimeoutDefault = 10.0; // seconds
        public double HistoryTimeout
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

        public bool EnableWindowFadeDefault = false;
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

        public double CursorIndicatorOpacityDefault = 0.3;
        public double CursorIndicatorOpacity
        {
            get { return Or(i.cursorIndicatorOpacity, CursorIndicatorOpacityDefault); }
            set { i.cursorIndicatorOpacity = value; OnSettingChanged("CursorIndicatorOpacity");
                Console.WriteLine("CursorIndicatorOpacity " + value); }
        }

        public double CursorIndicatorSizeDefault = 55;
        public double CursorIndicatorSize
        {
            get { return Or(i.cursorIndicatorSize, CursorIndicatorSizeDefault); }
            set { i.cursorIndicatorSize = value; OnSettingChanged("CursorIndicatorSize"); }
        }

        public Color CursorIndicatorColorDefault = Color.FromArgb(-32640);
        public Color CursorIndicatorColor
        {
            get { return Or(i.cursorIndicatorColor, CursorIndicatorColorDefault); }
            set { i.cursorIndicatorColor = new SerializableColor2(value); OnSettingChanged("CursorIndicatorColor"); }
        }

        public ButtonIndicatorType ButtonIndicatorDefault = ButtonIndicatorType.PicsAroundCursor;
        public ButtonIndicatorType ButtonIndicator
        {
            get { return Or(i.buttonIndicator, ButtonIndicatorDefault); }
            set { i.buttonIndicator = value; OnSettingChanged("ButtonIndicator"); }
        }

        public double ButtonIndicatorScalingDefault = 1.0f;
        public double ButtonIndicatorScaling
        {
            get { return Or(i.buttonIndicatorScaling, ButtonIndicatorScalingDefault); }
            set { i.buttonIndicatorScaling = value; OnSettingChanged("ButtonIndicatorScaling"); }
        }

        public double ButtonIndicatorPositionAngleDefault = 0f;
        public double ButtonIndicatorPositionAngle
        {
            get { return Or(i.buttonIndicatorPositionAngle, ButtonIndicatorPositionAngleDefault); }
            set { i.buttonIndicatorPositionAngle = value; OnSettingChanged("ButtonIndicatorPositionAngle"); }
        }

        public double ButtonIndicatorPositionDistanceDefault = 56;
        public double ButtonIndicatorPositionDistance
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

        public bool PeriodicTopmostDefault = true;
        public bool PeriodicTopmost
        {
            get { return Or(i.periodicTopmost, PeriodicTopmostDefault); }
            set { i.periodicTopmost = value; OnSettingChanged("PeriodicTopmost"); }
        }

        public bool EnableKeystrokeHistoryDefault = true;
        public bool EnableKeystrokeHistory
        {
            get { return Or(i.enableKeystrokeHistory, EnableKeystrokeHistoryDefault); }
            set { i.enableKeystrokeHistory = value; OnSettingChanged("EnableKeystrokeHistory"); }
        }

        public String KeystrokeHistorySettingsModeShortcutDefault = "RightCtrl + F12";
        public String KeystrokeHistorySettingsModeShortcut
        {
            get { return Or(i.keystrokeHistorySettingsModeShortcut, KeystrokeHistorySettingsModeShortcutDefault); }
            set { i.keystrokeHistorySettingsModeShortcut = value; OnSettingChanged("KeystrokeHistorySettingsModeShortcut"); }
        }

        public bool EnableSettingsModeDefault = false;
        public bool EnableSettingsMode
        {
            get { return Or(i.enableSettingsMode, EnableSettingsModeDefault); }
            set { i.enableSettingsMode = value; OnSettingChanged("EnableSettingsMode"); }
        }

        // Add new settings also to method CallPropertyChangedForAllProperties()

        #endregion

        #region SettingsChangedEvent

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnSettingChanged(string property)
        {
            dirty = true;
            var watch = System.Diagnostics.Stopwatch.StartNew();
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            watch.Stop();
            Log.e("SETTING", $"elapsedMs = {watch.ElapsedMilliseconds}");
        }

        public void CallPropertyChangedForAllProperties()
        {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs("LabelFont"));
                PropertyChanged(this, new PropertyChangedEventArgs("LabelColor"));
                PropertyChanged(this, new PropertyChangedEventArgs("BackgroundColor"));
                PropertyChanged(this, new PropertyChangedEventArgs("Opacity"));
                PropertyChanged(this, new PropertyChangedEventArgs("LabelTextAlignment"));
                PropertyChanged(this, new PropertyChangedEventArgs("LabelTextDirection"));
                PropertyChanged(this, new PropertyChangedEventArgs("LabelAnimation"));
                PropertyChanged(this, new PropertyChangedEventArgs("WindowLocation"));
                PropertyChanged(this, new PropertyChangedEventArgs("WindowSize"));
                PropertyChanged(this, new PropertyChangedEventArgs("PanelLocation"));
                PropertyChanged(this, new PropertyChangedEventArgs("PanelSize"));
                PropertyChanged(this, new PropertyChangedEventArgs("LineDistance"));
                PropertyChanged(this, new PropertyChangedEventArgs("HistoryLength"));
                PropertyChanged(this, new PropertyChangedEventArgs("HistoryTimeout"));
                PropertyChanged(this, new PropertyChangedEventArgs("EnableHistoryTimeout"));
                PropertyChanged(this, new PropertyChangedEventArgs("EnableWindowFade"));
                PropertyChanged(this, new PropertyChangedEventArgs("EnableCursorIndicator"));
                PropertyChanged(this, new PropertyChangedEventArgs("CursorIndicatorOpacity"));
                PropertyChanged(this, new PropertyChangedEventArgs("CursorIndicatorSize"));
                PropertyChanged(this, new PropertyChangedEventArgs("CursorIndicatorColor"));
                PropertyChanged(this, new PropertyChangedEventArgs("ButtonIndicator"));
                PropertyChanged(this, new PropertyChangedEventArgs("ButtonIndicatorScalingPercentage"));
                PropertyChanged(this, new PropertyChangedEventArgs("ButtonIndicatorPositionAngle"));
                PropertyChanged(this, new PropertyChangedEventArgs("ButtonIndicatorPositionDistance"));
                PropertyChanged(this, new PropertyChangedEventArgs("AddButtonEventsToHistory"));
                PropertyChanged(this, new PropertyChangedEventArgs("BackspaceDeletesText"));
                PropertyChanged(this, new PropertyChangedEventArgs("PeriodicTopmost"));
                PropertyChanged(this, new PropertyChangedEventArgs("EnableKeystrokeHistory"));
                PropertyChanged(this, new PropertyChangedEventArgs("KeystrokeHistorySettingsModeShortcut"));
                PropertyChanged(this, new PropertyChangedEventArgs("EnableSettingsMode"));
            }
        }

        #endregion

        #region SaveAll and LoadAll

        const string ISOLATED_STORAGE_FILE_NAME = "settings";

        bool dirty = false;

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

                            try
                            {
                                i = (Settings)ser.ReadObject(stream);
                            } catch
                            {
                                Log.e("SETTINGS", "Could not load settings, use default.");
                                i = new Settings();
                            }
                        }

                    }
                }
            }
            catch (System.Security.SecurityException sx)
            {
                System.Windows.MessageBox.Show(sx.Message);
                throw;
            }
            i.enableSettingsMode = false;
            dirty = false;
        }

        public void ResetAll()
        {
            i = new Settings();
            dirty = true;
            SaveAll();
            CallPropertyChangedForAllProperties();
        }

        #endregion

        public override string ToString()
        {

            return $@"LabelFont:                       {LabelFont.ToString()}
BackgroundColor:                 {BackgroundColor.ToString()}
Opacity:                         {Opacity}
LabelTextAlignment:              {LabelTextAlignment.ToString()}
LabelTextDirection:              {LabelTextDirection.ToString()}
LabelAnimation:                  {LabelAnimation.ToString()}
LabelColor:                      {LabelColor.ToString()}
WindowLocation:                  {WindowLocation.ToString()}
WindowSize:                      {WindowSize.ToString()}
PanelLocation:                   {PanelLocation.ToString()}
PanelSize:                       {PanelSize.ToString()}
LineDistance:                    {LineDistance}
HistoryLength:                   {HistoryLength}
HistoryTimeout:                  {HistoryTimeout}
EnableHistoryTimeout:            {EnableHistoryTimeout}
EnableWindowFade:                {EnableWindowFade}
EnableCursorIndicator:           {EnableCursorIndicator}
CursorIndicatorOpacity:          {CursorIndicatorOpacity}
CursorIndicatorSize:             {CursorIndicatorSize.ToString()}
CursorIndicatorColor:            {CursorIndicatorColor.ToString()}
ButtonIndicator:                 {ButtonIndicator.ToString()}
buttonIndicatorScalingPercentage:{ButtonIndicatorScaling}
ButtonIndicatorPositionAngle:    {ButtonIndicatorPositionAngle}
ButtonIndicatorPositionDistance: {ButtonIndicatorPositionDistance}
AddButtonEventsToHistory:        {AddButtonEventsToHistory}
BackspaceDeletesText:            {BackspaceDeletesText}
PeriodicTopmost:                 {PeriodicTopmost}
EnableKeystrokeHistory:          {EnableKeystrokeHistory}";
        }

    }
}
