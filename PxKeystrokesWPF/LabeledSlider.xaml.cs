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
            Log.e("BIN", "Set Data context in labeled slider");
            layout_root.DataContext = this;
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
        public double Minimum
        {
            get { return (double) GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(double), typeof(LabeledSlider), new PropertyMetadata(0.0));


        [Browsable(true)]
        public double Maximum
        {
            get { return (double) GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(double), typeof(LabeledSlider), new PropertyMetadata(0.0));


        [Browsable(true)]
        public double Value
        {
            get { return (double) GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(LabeledSlider), new PropertyMetadata(0.0));



        public event RoutedPropertyChangedEventHandler<double> ValueChanged;

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            RoutedPropertyChangedEventArgs<double> eint = null;
            if (e != null)
            {
                eint = new RoutedPropertyChangedEventArgs<double>(e.OldValue, e.NewValue, e.RoutedEvent);
            }
            if (ValueChanged != null)
            {
                ValueChanged.Invoke(sender, eint);
            }
        }
    }
}
