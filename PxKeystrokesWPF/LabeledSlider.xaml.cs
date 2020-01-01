using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PxKeystrokesWPF
{
    /// <summary>
    /// Interaktionslogik für LabeledSlider.xaml
    /// </summary>
    public partial class LabeledSlider : UserControl
    {
        public LabeledSlider()
        {
            InitializeComponent();
            this.DataContext = this;
        }


   
        [Browsable(true)]
        public String Title
        {
            get { return (String) GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(
            "Title", typeof(String),
            typeof(LabeledSlider)
            );

        [Browsable(true)]
        public int Minimum
        {
            get { return (int)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(int), typeof(LabeledSlider), new PropertyMetadata(0));


        [Browsable(true)]
        public int Maximum
        {
            get { return (int)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(int), typeof(LabeledSlider), new PropertyMetadata(0));


        [Browsable(true)]
        public int Value
        {
            get { return (int)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(int), typeof(LabeledSlider), new PropertyMetadata(0));



        public event RoutedPropertyChangedEventHandler<int> ValueChanged;

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            RoutedPropertyChangedEventArgs<int> eint = null;
            if (e != null)
            {
                eint = new RoutedPropertyChangedEventArgs<int>((int)e.OldValue, (int)e.NewValue, e.RoutedEvent);
            }
            if (ValueChanged != null)
            {
                ValueChanged.Invoke(sender, eint);
            }
        }
    }
}
