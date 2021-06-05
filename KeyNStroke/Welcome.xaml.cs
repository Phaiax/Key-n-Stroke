using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Documents;

namespace KeyNStroke
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

            Updater.Updater.Instance.OnUiUpdate += Updater_onUIUpdate;
            Updater.Updater.Instance.OnShutdownRequest += Updater_onShutdownRequest;
            ButtonCheckForUpdates.Click += Updater.Updater.Instance.UiButton_Click;

            Version current = Assembly.GetExecutingAssembly().GetName().Version;
            VersionInfo.Text = current.ToString(3);

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
            Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} Click");
            ((App)Application.Current).showSettingsWindow();
        }

        #region Updater

        List<string> updateDetails;

        private void Updater_onUIUpdate(object sender, Updater.Statemachine.UiUpdateEventArgs e)
        {
            this.Dispatcher.InvokeAsync(() =>
            {
                ButtonCheckForUpdates.Content = e.buttonText;
                ButtonCheckForUpdates.IsEnabled = e.buttonEnabled;
                TextUpdateStatus.Inlines.Clear();
                if (e.details != null && e.details.Count > 0)
                {
                    Hyperlink hl = new Hyperlink(new Run(e.info));
                    hl.NavigateUri = new Uri("none://dummy"); 
                    hl.RequestNavigate += Hyperlink_RequestNavigate_UpdateDetails;
                    TextUpdateStatus.Inlines.Add(hl);
                    updateDetails = e.details;
                } else
                {
                    TextUpdateStatus.Inlines.Add(new Run(e.info));
                    updateDetails = null;
                }
            });
        }

        private void Hyperlink_RequestNavigate_UpdateDetails(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            if (updateDetails != null)
            {
                MessageBox.Show(String.Join("\n", updateDetails), "Updater Info", MessageBoxButton.OK);
            }
        }



        private void Updater_onShutdownRequest(object sender)
        {
            this.Dispatcher.InvokeAsync(() =>
            {
                Application.Current.Shutdown();
            });
        }

        #endregion

        private void ButtonHideThisWindow_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ButtonExitApplication_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void Hyperlink_RequestNavigate_settings(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            ((App)Application.Current).showSettingsWindow();
        }
    }
}
