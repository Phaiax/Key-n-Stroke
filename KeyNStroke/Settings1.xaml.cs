﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfColorFontDialog;

namespace KeyNStroke
{
    public class EnumBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //MessageBox.Show("Convert " + value.ToString() + " tt: " + targetType.ToString() + " par " + parameter.ToString());
            return value.Equals(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //MessageBox.Show("Convert Back " + value.ToString() + " tt: " + targetType.ToString() + " par " + parameter.ToString());
            return ((bool)value) ? parameter : Binding.DoNothing;
        }
    }

    public class FloatPercentageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //Log.e("CNV", "Convert " + value.ToString() + " tt: " + targetType.ToString());
            return (int) ((float) value * 100f);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //Log.e("CNV", "Convert Back " + value.ToString() + " tt: " + targetType.ToString());
            return (int)value / 100f;
        }
    }

    public class MediaColorDrawingColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //Log.e("CNV", "Convert " + value.ToString() + " tt: " + targetType.ToString());
            return UIHelper.ToMediaColor((System.Drawing.Color) value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //Log.e("CNV", "Convert Back " + value.ToString() + " tt: " + targetType.ToString());
            return UIHelper.ToDrawingColor((System.Windows.Media.Color) value);
        }
    }

    /// <summary>
    /// Interaktionslogik für Settings1.xaml
    /// </summary>
    public partial class Settings1 : Window
    {
        public Settings1(SettingsStore s, IKeystrokeEventProvider k)
        {
            InitializeComponent();
            settings = s;
            this.k = k;
            Log.e("BIN", "Set Data context in settings window");
            layout_root.DataContext = settings;
            SettingsModeShortcutDefault.Text = settings.KeystrokeHistorySettingsModeShortcutDefault;
            PasswordModeShortcutDefault.Text = settings.KeystrokeHistoryPasswordModeShortcutDefault;
            AnnotateLineShortcutDefault.Text = settings.AnnotateLineShortcutDefault;
            StandbyShortcutDefault.Text = settings.StandbyShortcutDefault;
            s.PropertyChanged += S_PropertyChanged;
            s.CallPropertyChangedForAllProperties();
        }

        SettingsStore settings;
        IKeystrokeEventProvider k;

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            ((App)Application.Current).onSettingsWindowClosed();
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            UrlOpener.OpenGithub();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            settings.SaveAll();
        }

        private void Bn_reset_position_Click(object sender, RoutedEventArgs e)
        {
            settings.PanelLocation = settings.PanelLocationDefault;
            settings.PanelSize = settings.PanelSizeDefault;
            settings.WindowLocation = settings.WindowLocationDefault;
            settings.WindowSize = settings.WindowSizeDefault;

            settings.CallPropertyChangedForAllProperties();
        }

        private void bn_reset_all_Click(object sender, RoutedEventArgs e)
        {
            settings.ResetAll();
        }

        private void Button_close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void button_exit_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void OnButtonTextFontClick(object sender, RoutedEventArgs e)
        {
            ColorFontDialog dialog = new ColorFontDialog(true, true, false);
            dialog.Font = settings.LabelFont;
            dialog.Loaded += FontDialogLoaded;
            if (dialog.ShowDialog() == true)
            {
                FontInfo font = dialog.Font;
                if (font != null)
                {
                    settings.LabelFont = font;
                }
            }
        }

        private void FontDialogLoaded(object sender, RoutedEventArgs e)
        {
            // run after the init funtions of the colordialog itself
            ColorFontDialog dialog = (ColorFontDialog)sender;
            TextBox sampleText = UIHelper.FindChild<TextBox>((DependencyObject)dialog.Content, "txtSampleText");
            sampleText.Background = new SolidColorBrush(UIHelper.ToMediaColor(settings.BackgroundColor));
            sampleText.Foreground = new SolidColorBrush(UIHelper.ToMediaColor(settings.LabelColor));
        }

        #region Shortcut - ChangeResizeMove

        private void Hyperlink_ChangeResizeMoveShortcut(object sender, RoutedEventArgs e)
        {
            ReadShortcut rs = new ReadShortcut(k, "enabling and disabling the move+resize mode for the keystroke history window.");
            rs.ShowDialog();
            if (rs.Shortcut != null)
            {
                settings.KeystrokeHistorySettingsModeShortcut = rs.Shortcut;
            }
        }

        private void Hyperlink_ResetResizeMoveShortcut(object sender, RoutedEventArgs e)
        {
            settings.KeystrokeHistorySettingsModeShortcut = settings.KeystrokeHistorySettingsModeShortcutDefault;
        }

        private void Hyperlink_TriggerResizeMoveShortcut(object sender, RoutedEventArgs e)
        {
            settings.EnableSettingsMode = !settings.EnableSettingsMode;
        }

        #endregion

        #region Shortcut - PasswordMode

        private void Hyperlink_ChangePasswordModeShortcut(object sender, RoutedEventArgs e)
        {
            ReadShortcut rs = new ReadShortcut(k, " toggling password protection mode.");
            rs.ShowDialog();
            if (rs.Shortcut != null)
            {
                settings.KeystrokeHistoryPasswordModeShortcut= rs.Shortcut;
            }
        }

        private void Hyperlink_ResetPasswordModeShortcut(object sender, RoutedEventArgs e)
        {
            settings.KeystrokeHistoryPasswordModeShortcut = settings.KeystrokeHistoryPasswordModeShortcutDefault;
        }

        private void Hyperlink_TriggerPasswordModeShortcut(object sender, RoutedEventArgs e)
        {
            settings.EnablePasswordMode = !settings.EnablePasswordMode;
        }

        #endregion

        #region Shortcut - AnnotateLine

        private void Hyperlink_ChangeAnnotateLineShortcut(object sender, RoutedEventArgs e)
        {
            ReadShortcut rs = new ReadShortcut(k, " triggering line draw.");
            rs.ShowDialog();
            if (rs.Shortcut != null)
            {
                settings.AnnotateLineShortcut = rs.Shortcut;
            }
        }

        private void Hyperlink_ResetAnnotateLineShortcut(object sender, RoutedEventArgs e)
        {
            settings.AnnotateLineShortcut = settings.AnnotateLineShortcutDefault;
        }

        private void Hyperlink_TriggerAnnotateLineShortcut(object sender, RoutedEventArgs e)
        {
            settings.AnnotateLineShortcutTrigger = true;
        }


        #endregion

        #region Shortcut - Toggle Standby

        private void Hyperlink_ChangeStandbyShortcut(object sender, RoutedEventArgs e)
        {
            ReadShortcut rs = new ReadShortcut(k, " toggling standby mode.");
            rs.ShowDialog();
            if (rs.Shortcut != null)
            {
                settings.StandbyShortcut = rs.Shortcut;
            }
        }

        private void Hyperlink_ResetStandbyShortcut(object sender, RoutedEventArgs e)
        {
            settings.StandbyShortcut = settings.StandbyShortcutDefault;
        }

        private void Hyperlink_TriggerStandbyShortcut(object sender, RoutedEventArgs e)
        {
            settings.Standby = !settings.Standby;
        }

        #endregion

        #region Custom Icons


        private void S_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch(e.PropertyName)
            {
                case "ButtonIndicatorUseCustomIcons":
                    if (settings.ButtonIndicatorUseCustomIcons)
                    {
                        textblockCustomIconInfoBuiltin.Visibility = Visibility.Collapsed;
                        textblockCustomIconInfoCustom.Visibility = Visibility.Visible;
                    } 
                    else
                    {
                        textblockCustomIconInfoBuiltin.Visibility = Visibility.Visible;
                        textblockCustomIconInfoCustom.Visibility = Visibility.Collapsed;
                    }
                    break;
            }
        }

        private void OnButtonCustomIconsSelectFolder(object sender, RoutedEventArgs e)
        {
            using (System.Windows.Forms.FolderBrowserDialog dlg = new System.Windows.Forms.FolderBrowserDialog())
            {
                dlg.Description = "Select folder with custom icons";
                dlg.SelectedPath = settings.ButtonIndicatorCustomIconsFolder;
                dlg.ShowNewFolderButton = true;
                System.Windows.Forms.DialogResult result = dlg.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    settings.ButtonIndicatorCustomIconsFolder = dlg.SelectedPath;
                }
            }
        }

        private void OnButtonExportBuiltinIcons(object sender, RoutedEventArgs e)
        {
            using (System.Windows.Forms.FolderBrowserDialog dlg = new System.Windows.Forms.FolderBrowserDialog())
            {
                dlg.Description = "Select folder for export";
                // dlg.SelectedPath = settings.ButtonIndicatorCustomIconsFolder;
                dlg.ShowNewFolderButton = true;
                System.Windows.Forms.DialogResult result = dlg.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    ImageResources.ExportBuiltinRessources(dlg.SelectedPath);
                }
            }
            
        }

        private void OnClickCustomIconsHelp(object sender, RoutedEventArgs e)
        {

        }

        private void OnClickCustomIconsRefresh(object sender, RoutedEventArgs e)
        {
            if (settings.ButtonIndicatorUseCustomIcons)
            {
                ImageResources.ReloadRessources(settings.ButtonIndicatorCustomIconsFolder);
            }
            else
            {
                ImageResources.ReloadRessources(null);
            }
        }




        #endregion

        #region Other Hyperlinks

        private void Hyperlink_WelcomeWindow(object sender, RoutedEventArgs e)
        {
            ((App)Application.Current).showWelcomeWindow();
        }

        private void Hyperlink_DisableStandbyMode(object sender, RoutedEventArgs e)
        {
            settings.Standby = false;
        }
        #endregion

    }
}
