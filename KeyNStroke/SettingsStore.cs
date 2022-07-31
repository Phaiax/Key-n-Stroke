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

namespace KeyNStroke
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

    public enum KeystrokeMethodEnum
    {
        [Description("Text mode")]
        TextMode = 1,
        [Description("Text mode (Backspace can delete text)")]
        TextModeBackspaceCanDeleteText = 2,
        [Description("Shortcut mode (no letters and no shift)")]
        ShortcutModeNoText = 3,
        [Description("Shortcut mode (with letters and shift)")]
        ShortcutModeWithText = 4
    }

    public static class Extensions
    {
        public static bool IsTextMode(this KeystrokeMethodEnum method)
        {
            return method == KeystrokeMethodEnum.TextMode || method == KeystrokeMethodEnum.TextModeBackspaceCanDeleteText;
        }
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
        [DataMember] public Nullable<bool> enableTextOverSymbol = null;
        [DataMember] public Nullable<double> cursorIndicatorOpacity = null;
        [DataMember] public Nullable<double> cursorIndicatorSize = null;
        [DataMember] public SerializableColor2 cursorIndicatorColor = null;
        [DataMember] public Nullable<bool> cursorIndicatorFlashOnClick = null;
        [DataMember] public SerializableColor2 cursorIndicatorClickColor = null;
        [DataMember] public Nullable<bool> cursorIndicatorHideIfCustomCursor = null;
        [DataMember] public Nullable<bool> cursorIndicatorDrawEdge = null;
        [DataMember] public SerializableColor2 cursorIndicatorEdgeColor = null;
        [DataMember] public Nullable<double> cursorIndicatorEdgeStrokeThickness = null;
        [DataMember] public Nullable<ButtonIndicatorType> buttonIndicator = null;
        [DataMember] public Nullable<double> buttonIndicatorScaling = null;
        [DataMember] public Nullable<double> buttonIndicatorPositionAngle = null;
        [DataMember] public Nullable<double> buttonIndicatorPositionDistance = null;
        [DataMember] public Nullable<bool> buttonIndicatorShowModifiers = null;
        [DataMember] public Nullable<bool> buttonIndicatorUseCustomIcons = null;
        [DataMember] public string buttonIndicatorCustomIconsFolder = null;
        [DataMember] public Nullable<bool> addButtonEventsToHistory = null;
        [DataMember] public Nullable<bool> backspaceDeletesText = null; // replaced by keystrokeMethod
        [DataMember] public Nullable<bool> periodicTopmost = null;
        [DataMember] public Nullable<bool> enableKeystrokeHistory = null;
        [DataMember] public String keystrokeHistorySettingsModeShortcut = null;
        [DataMember] public Nullable<bool> enableSettingsMode = null;
        [DataMember] public String keystrokeHistoryPasswordModeShortcut = null;
        [DataMember] public Nullable<bool> enablePasswordMode = null;
        [DataMember] public Nullable<KeystrokeMethodEnum> keystrokeMethod = null;
        [DataMember] public Nullable<bool> enableAnnotateLine = null;
        [DataMember] public SerializableColor2 annotateLineColor = null;
        [DataMember] public String annotateLineShortcut = null;
        [DataMember] public String standbyShortcut = null;
        [DataMember] public Nullable<bool> startInStandby = null;
        [DataMember] public Nullable<bool> welcomeOnStartup = null;


    }

    #endregion

    public class SettingsStore : INotifyPropertyChanged
    {

        #region Constructor

        public SettingsStore()
        {
            const string SETTINGS_PATH = "Key-n-Stroke/settings.json";
            string appldatapath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            configpath = Path.Combine(appldatapath, SETTINGS_PATH);
        }

        string configpath;

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

        public bool EnableTextOverSymbolDefault = false;
        public bool EnableTextOverSymbol
        {
            get { return Or(i.enableTextOverSymbol, EnableTextOverSymbolDefault); }
            set { i.enableTextOverSymbol = value; OnSettingChanged("EnableTextOverSymbol"); }
        }

        public double CursorIndicatorOpacityDefault = 0.3;
        public double CursorIndicatorOpacity
        {
            get { return Or(i.cursorIndicatorOpacity, CursorIndicatorOpacityDefault); }
            set { i.cursorIndicatorOpacity = value; OnSettingChanged("CursorIndicatorOpacity"); }
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

        public bool CursorIndicatorFlashOnClickDefault = true;
        public bool CursorIndicatorFlashOnClick
        {
            get { return Or(i.cursorIndicatorFlashOnClick, CursorIndicatorFlashOnClickDefault); }
            set { i.cursorIndicatorFlashOnClick = value; OnSettingChanged("CursorIndicatorFlashOnClick"); }
        }

        public Color CursorIndicatorClickColorDefault = Color.FromArgb(-32640);
        public Color CursorIndicatorClickColor
        {
            get { return Or(i.cursorIndicatorClickColor, CursorIndicatorClickColorDefault); }
            set { i.cursorIndicatorClickColor = new SerializableColor2(value); OnSettingChanged("CursorIndicatorClickColor"); }
        }

        public bool CursorIndicatorDrawEdgeDefault = true;
        public bool CursorIndicatorDrawEdge
        {
            get { return Or(i.cursorIndicatorDrawEdge, CursorIndicatorDrawEdgeDefault); }
            set { i.cursorIndicatorDrawEdge = value; OnSettingChanged("CursorIndicatorDrawEdge"); }
        }

        public Color CursorIndicatorEdgeColorDefault = Color.Black;
        public Color CursorIndicatorEdgeColor
        {
            get { return Or(i.cursorIndicatorEdgeColor, CursorIndicatorEdgeColorDefault); }
            set { i.cursorIndicatorEdgeColor = new SerializableColor2(value); OnSettingChanged("CursorIndicatorEdgeColor"); }
        }

        public double CursorIndicatorEdgeStrokeThicknessDefault = 2;
        public double CursorIndicatorEdgeStrokeThickness
        {
            get { return Or(i.cursorIndicatorEdgeStrokeThickness, CursorIndicatorEdgeStrokeThicknessDefault); }
            set { i.cursorIndicatorEdgeStrokeThickness = value; OnSettingChanged("CursorIndicatorEdgeStrokeThickness"); }
        }

        public bool CursorIndicatorHideIfCustomCursorDefault = false;
        public bool CursorIndicatorHideIfCustomCursor
        {
            get { return Or(i.cursorIndicatorHideIfCustomCursor, CursorIndicatorHideIfCustomCursorDefault); }
            set { i.cursorIndicatorHideIfCustomCursor = value; OnSettingChanged("CursorIndicatorHideIfCustomCursor"); }
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

        public bool ButtonIndicatorShowModifiersDefault = true;
        public bool ButtonIndicatorShowModifiers
        {
            get { return Or(i.buttonIndicatorShowModifiers, ButtonIndicatorShowModifiersDefault); }
            set { i.buttonIndicatorShowModifiers = value; OnSettingChanged("ButtonIndicatorShowModifiers"); }
        }

        public bool ButtonIndicatorUseCustomIconsDefault = false;
        public bool ButtonIndicatorUseCustomIcons
        {
            get { return Or(i.buttonIndicatorUseCustomIcons, ButtonIndicatorUseCustomIconsDefault); }
            set { i.buttonIndicatorUseCustomIcons = value; OnSettingChanged("ButtonIndicatorUseCustomIcons"); }
        }

        public string ButtonIndicatorCustomIconsFolderDefault = "";
        public string ButtonIndicatorCustomIconsFolder
        {
            get { return Or(i.buttonIndicatorCustomIconsFolder, ButtonIndicatorCustomIconsFolderDefault); }
            set { i.buttonIndicatorCustomIconsFolder = value; OnSettingChanged("ButtonIndicatorCustomIconsFolder"); }
        }

        public bool AddButtonEventsToHistoryDefault = false;
        public bool AddButtonEventsToHistory
        {
            get { return Or(i.addButtonEventsToHistory, AddButtonEventsToHistoryDefault); }
            set { i.addButtonEventsToHistory = value; OnSettingChanged("AddButtonEventsToHistory"); }
        }

        public bool BackspaceDeletesText
        {
            get { return KeystrokeMethod == KeystrokeMethodEnum.TextModeBackspaceCanDeleteText; }
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

        public String KeystrokeHistoryPasswordModeShortcutDefault = "RightCtrl + F11";
        public String KeystrokeHistoryPasswordModeShortcut
        {
            get { return Or(i.keystrokeHistoryPasswordModeShortcut, KeystrokeHistoryPasswordModeShortcutDefault); }
            set { i.keystrokeHistoryPasswordModeShortcut = value; OnSettingChanged("KeystrokeHistoryPasswordModeShortcut"); }
        }

        public bool EnablePasswordModeDefault = false;
        public bool EnablePasswordMode
        {
            get { return Or(i.enablePasswordMode, EnablePasswordModeDefault); }
            set { i.enablePasswordMode = value; OnSettingChanged("EnablePasswordMode"); }
        }

        public KeystrokeMethodEnum KeystrokeMethodDefault = KeystrokeMethodEnum.TextModeBackspaceCanDeleteText;
        public KeystrokeMethodEnum KeystrokeMethod
        {
            get
            {
                // Update compatability: Take over setting from depreciated backspaceDeletesText
                if (!i.keystrokeMethod.HasValue && i.backspaceDeletesText.HasValue)
                {
                    if (i.backspaceDeletesText.Value)
                    {
                        return KeystrokeMethodEnum.TextModeBackspaceCanDeleteText;
                    }
                    else
                    {
                        return KeystrokeMethodEnum.TextMode;
                    }
                }
                return Or(i.keystrokeMethod, KeystrokeMethodDefault);
            }
            set { i.keystrokeMethod = value; OnSettingChanged("KeystrokeMethod"); }
        }

        public bool EnableAnnotateLineDefault = false;
        public bool EnableAnnotateLine
        {
            get { return Or(i.enableAnnotateLine, EnableAnnotateLineDefault); }
            set { i.enableAnnotateLine = value; OnSettingChanged("EnableAnnotateLine"); }
        }

        public Color AnnotateLineColorDefault = Color.FromArgb(0xFF, 0x7E, 0xEF, 0x84);
        public Color AnnotateLineColor
        {
            get { return Or(i.annotateLineColor, AnnotateLineColorDefault); }
            set { i.annotateLineColor = new SerializableColor2(value); OnSettingChanged("AnnotateLineColor"); }
        }

        public String AnnotateLineShortcutDefault = "RightCtrl + F10";
        public String AnnotateLineShortcut
        {
            get { return Or(i.annotateLineShortcut, AnnotateLineShortcutDefault); }
            set { i.annotateLineShortcut = value; OnSettingChanged("AnnotateLineShortcut"); }
        }

        public bool AnnotateLineShortcutTrigger
        {
            set { OnSettingChanged("AnnotateLineShortcutTrigger"); }
        }

        public String StandbyShortcutDefault = "RightCtrl + F9";
        public String StandbyShortcut
        {
            get { return Or(i.standbyShortcut, StandbyShortcutDefault); }
            set { i.standbyShortcut = value; OnSettingChanged("StandbyShortcut"); }
        }

        public bool StartInStandbyDefault = false;
        public bool StartInStandby
        {
            get { return Or(i.startInStandby, StartInStandbyDefault); }
            set { i.startInStandby = value; OnSettingChanged("StartInStandby"); }
        }

        public bool standby = true;
        public bool Standby
        {
            get { return standby; }
            set { standby = value; OnSettingChanged("Standby"); }
        }

        public bool WelcomeOnStartupDefault = true;
        public bool WelcomeOnStartup
        {
            get { return Or(i.welcomeOnStartup, WelcomeOnStartupDefault); }
            set { i.welcomeOnStartup = value; OnSettingChanged("WelcomeOnStartup"); }
        }

        // Add new settings also to method CallPropertyChangedForAllProperties()

        #endregion

        #region SettingsChangedEvent

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnSettingChanged(string property)
        {
            dirty = true;
            var watch = System.Diagnostics.Stopwatch.StartNew();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
            watch.Stop();
            Log.e("SETTING", $"elapsedMs = {watch.ElapsedMilliseconds}");
        }

        public void CallPropertyChangedForAllProperties()
        {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs("LabelFont"));
                PropertyChanged(this, new PropertyChangedEventArgs("LabelColor"));
                PropertyChanged(this, new PropertyChangedEventArgs("BackgroundColor"));
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
                PropertyChanged(this, new PropertyChangedEventArgs("EnableTextOverSymbol"));
                PropertyChanged(this, new PropertyChangedEventArgs("CursorIndicatorOpacity"));
                PropertyChanged(this, new PropertyChangedEventArgs("CursorIndicatorSize"));
                PropertyChanged(this, new PropertyChangedEventArgs("CursorIndicatorColor"));
                PropertyChanged(this, new PropertyChangedEventArgs("CursorIndicatorFlashOnClick"));
                PropertyChanged(this, new PropertyChangedEventArgs("CursorIndicatorClickColor"));
                PropertyChanged(this, new PropertyChangedEventArgs("CursorIndicatorEdgeColor"));
                PropertyChanged(this, new PropertyChangedEventArgs("CursorIndicatorDrawEdge"));
                PropertyChanged(this, new PropertyChangedEventArgs("CursorIndicatorEdgeStrokeThickness"));
                PropertyChanged(this, new PropertyChangedEventArgs("CursorIndicatorHideIfCustomCursor"));
                PropertyChanged(this, new PropertyChangedEventArgs("ButtonIndicator"));
                PropertyChanged(this, new PropertyChangedEventArgs("ButtonIndicatorScalingPercentage"));
                PropertyChanged(this, new PropertyChangedEventArgs("ButtonIndicatorPositionAngle"));
                PropertyChanged(this, new PropertyChangedEventArgs("ButtonIndicatorPositionDistance"));
                PropertyChanged(this, new PropertyChangedEventArgs("ButtonIndicatorShowModifiers"));
                PropertyChanged(this, new PropertyChangedEventArgs("ButtonIndicatorUseCustomIcons"));
                PropertyChanged(this, new PropertyChangedEventArgs("ButtonIndicatorCustomIconsFolder"));
                PropertyChanged(this, new PropertyChangedEventArgs("AddButtonEventsToHistory"));
                // PropertyChanged(this, new PropertyChangedEventArgs("BackspaceDeletesText"));
                PropertyChanged(this, new PropertyChangedEventArgs("PeriodicTopmost"));
                PropertyChanged(this, new PropertyChangedEventArgs("EnableKeystrokeHistory"));
                PropertyChanged(this, new PropertyChangedEventArgs("KeystrokeHistorySettingsModeShortcut"));
                PropertyChanged(this, new PropertyChangedEventArgs("EnableSettingsMode"));
                PropertyChanged(this, new PropertyChangedEventArgs("KeystrokeHistoryPasswordModeShortcut"));
                PropertyChanged(this, new PropertyChangedEventArgs("EnablePasswordMode"));
                PropertyChanged(this, new PropertyChangedEventArgs("KeystrokeMethod"));
                PropertyChanged(this, new PropertyChangedEventArgs("EnableAnnotateLine"));
                PropertyChanged(this, new PropertyChangedEventArgs("AnnotateLineColor"));
                PropertyChanged(this, new PropertyChangedEventArgs("AnnotateLineShortcut"));
                PropertyChanged(this, new PropertyChangedEventArgs("StandbyShortcut"));
                PropertyChanged(this, new PropertyChangedEventArgs("StartInStandby"));
                PropertyChanged(this, new PropertyChangedEventArgs("Standby"));
                PropertyChanged(this, new PropertyChangedEventArgs("WelcomeOnStartup"));

            }
        }

        #endregion

        #region SaveAll and LoadAll


        bool dirty = false;

        public void SaveAll()
        {
            if (dirty)
            {
                try
                {
                    if (!Directory.Exists(Path.GetDirectoryName(configpath)))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(configpath));
                    }
                    using (var stream = new FileStream(configpath, FileMode.Create, FileAccess.Write))
                    {
                        DataContractJsonSerializer ser =
                            new DataContractJsonSerializer(typeof(Settings));

                        ser.WriteObject(stream, i);
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
                if (File.Exists(configpath))
                {
                    using (var stream = new FileStream(configpath, FileMode.Open, FileAccess.Read))
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
            catch (System.Security.SecurityException sx)
            {
                System.Windows.MessageBox.Show(sx.Message);
                throw;
            }
            i.enableSettingsMode = false;
            standby = StartInStandby;
            dirty = false;
        }

        public void ResetAll()
        {
            i = new Settings();
            dirty = true;
            SaveAll();
            standby = false;
            CallPropertyChangedForAllProperties();
        }

        #endregion

        public override string ToString()
        {

            return $@"LabelFont:                       {LabelFont}
BackgroundColor:                 {BackgroundColor}
LabelTextAlignment:              {LabelTextAlignment}
LabelTextDirection:              {LabelTextDirection}
LabelAnimation:                  {LabelAnimation}
LabelColor:                      {LabelColor}
WindowLocation:                  {WindowLocation}
WindowSize:                      {WindowSize}
PanelLocation:                   {PanelLocation}
PanelSize:                       {PanelSize}
LineDistance:                    {LineDistance}
HistoryLength:                   {HistoryLength}
HistoryTimeout:                  {HistoryTimeout}
EnableHistoryTimeout:            {EnableHistoryTimeout}
EnableWindowFade:                {EnableWindowFade}
EnableCursorIndicator:           {EnableCursorIndicator}
EnableTextOverSymbol:            {EnableTextOverSymbol}
CursorIndicatorOpacity:          {CursorIndicatorOpacity}
CursorIndicatorSize:             {CursorIndicatorSize}
CursorIndicatorColor:            {CursorIndicatorColor}
CursorIndicatorFlashOnClick:     {CursorIndicatorFlashOnClick}
CursorIndicatorClickColor:       {CursorIndicatorClickColor}
CursorIndicatorDrawEdge:         {CursorIndicatorDrawEdge}
CursorIndicatorEdgeColor:        {CursorIndicatorEdgeColor}
CursorIndicatorEdgeStrokeThickness: {CursorIndicatorEdgeStrokeThickness}
CursorIndicatorHideIfCustomCursor: {CursorIndicatorHideIfCustomCursor}
ButtonIndicator:                 {ButtonIndicator}
buttonIndicatorScalingPercentage:{ButtonIndicatorScaling}
ButtonIndicatorPositionAngle:    {ButtonIndicatorPositionAngle}
ButtonIndicatorPositionDistance: {ButtonIndicatorPositionDistance}
ButtonIndicatorShowModifiers:    {ButtonIndicatorShowModifiers}
ButtonIndicatorUseCustomIcons:   {ButtonIndicatorUseCustomIcons}
ButtonIndicatorCustomIconsFolder:{ButtonIndicatorCustomIconsFolder}
AddButtonEventsToHistory:        {AddButtonEventsToHistory}
BackspaceDeletesText:            {BackspaceDeletesText}
PeriodicTopmost:                 {PeriodicTopmost}
EnableKeystrokeHistory:          {EnableKeystrokeHistory}
KeystrokeHistorySettingsModeShortcut: {KeystrokeHistorySettingsModeShortcut}
EnableSettingsMode:             {EnableSettingsMode}
KeystrokeHistoryPasswordModeShortcut: {KeystrokeHistoryPasswordModeShortcut}
EnablePasswordMode:             {EnablePasswordMode}
KeystrokeMethod:                {KeystrokeMethod}
EnableAnnotateLine:             {EnableAnnotateLine}
AnnotateLineColor:              {AnnotateLineColor}
AnnotateLineShortcut:           {AnnotateLineShortcut}
StandbyShortcut:                {StandbyShortcut}
StartInStandby:                 {StartInStandby}
Standby:                        {Standby}
WelcomeOnStartup:               {WelcomeOnStartup}
";
        }

    }
}
