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

namespace KeyNStroke
{
    /// <summary>
    /// Interaktionslogik für ReadShortcut.xaml
    /// </summary>
    public partial class ReadShortcut : Window
    {
        IKeystrokeEventProvider k;

        string shortcut;
        public string Shortcut
        {
            get { return shortcut; }
        }

        public ReadShortcut(IKeystrokeEventProvider k, string purpose)
        {
            InitializeComponent();
            this.k = k;
            this.run_purpose.Text = purpose;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.k.KeystrokeEvent += k_KeystrokeEvent;

        }

        void k_KeystrokeEvent(KeystrokeEventArgs e)
        {
            string idf = e.ShortcutIdentifier();
            if (idf != null)
            {
                shortcut = idf;
                Close();
            }
        }
    }
}
