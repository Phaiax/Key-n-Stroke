using System;
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

namespace PxKeystrokesWPF
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
            Log.e("CNV", "Convert " + value.ToString() + " tt: " + targetType.ToString());
            return (int) ((float) value * 100f);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Log.e("CNV", "Convert Back " + value.ToString() + " tt: " + targetType.ToString());
            return (int)value / 100f;
        }
    }

    /// <summary>
    /// Interaktionslogik für Settings1.xaml
    /// </summary>
    public partial class Settings1 : Window
    {
        public Settings1(SettingsStore s)
        {
            InitializeComponent();
            settings = s;
            Log.e("BIN", "Set Data context in settings window");
            layout_root.DataContext = settings;
        }

        SettingsStore settings;


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
            Log.e("CNV", $"Alignment: {settings.LabelTextAlignment}" 
                + $" | settings.EnableCursorIndicator={settings.EnableCursorIndicator}" 
                + $" | settings.HistoryTimeout={settings.HistoryTimeout}"
                + $" | slider_history_timeout.Value={slider_history_timeout.Value}");
        }

        private void Button_close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
