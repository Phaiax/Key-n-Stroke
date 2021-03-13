using System;
using System.Collections.Generic;
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
    /// <summary>
    /// Interaktionslogik für Welcome.xaml
    /// </summary>
    public partial class Welcome : Window
    {
        SettingsStore settings;

        public Welcome(SettingsStore s)
        {
            InitializeComponent();
            settings = s;
            layout_root.DataContext = settings;

        }

        private void Hyperlink_RequestNavigate_README(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            UrlOpener.OpenGithubREADME();
        }


        private void Hyperlink_RequestNavigate_Issues(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            UrlOpener.OpenGithubIssues();
        }


        private void ButtonSettings_Click(object sender, RoutedEventArgs e)
        {
            ((App)Application.Current).showSettingsWindow();
        }

        private void ButtonCheckForUpdates_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonHideThisWindow_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ButtonExitApplication_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
    }
}
