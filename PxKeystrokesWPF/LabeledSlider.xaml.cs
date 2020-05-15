using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PxKeystrokesWPF
{

    /// <summary>
    /// Interaktionslogik für LabeledSlider.xaml
    /// </summary>
    public partial class LabeledSlider : UserControl
    {

        private bool interalValueUpdate = false;

        public LabeledSlider()
        {
            InitializeComponent();
            Log.e("BIN", "Set Data context in labeled slider");
            layout_root.DataContext = this;

            var dpd = DependencyPropertyDescriptor.FromProperty(ValueProperty, typeof(LabeledSlider));
            dpd.AddValueChanged(this, (object sender, EventArgs e) => {
                Log.e("SLIDER", $"{Title}: Value={Value}");
                ValueToSlider();
            });

            var dpd1 = DependencyPropertyDescriptor.FromProperty(MinimumProperty, typeof(LabeledSlider));
            dpd1.AddValueChanged(this, (object sender, EventArgs e) => { recalcParams(); ValueToSlider(); });

            var dpd2 = DependencyPropertyDescriptor.FromProperty(MaximumProperty, typeof(LabeledSlider));
            dpd2.AddValueChanged(this, (object sender, EventArgs e) => { recalcParams(); ValueToSlider(); });

            var dpd3 = DependencyPropertyDescriptor.FromProperty(HalfWayProperty, typeof(LabeledSlider));
            dpd3.AddValueChanged(this, (object sender, EventArgs e) => { recalcParams(); ValueToSlider(); });

            var dpd4 = DependencyPropertyDescriptor.FromProperty(LogarithmicProperty, typeof(LabeledSlider));
            dpd4.AddValueChanged(this, (object sender, EventArgs e) => { recalcParams(); ValueToSlider();  });

        }


        [Browsable(true)]
        public double HalfWay
        {
            get { return (double)GetValue(HalfWayProperty); }
            set { SetValue(HalfWayProperty, value); }
        }
        public static readonly DependencyProperty HalfWayProperty =
            DependencyProperty.Register(
            "HalfWay", typeof(double),
            typeof(LabeledSlider)
            );

        [Browsable(true)]
        public bool Logarithmic
        {
            get { return (bool)GetValue(LogarithmicProperty); }
            set { SetValue(LogarithmicProperty, value); }
        }
        public static readonly DependencyProperty LogarithmicProperty =
            DependencyProperty.Register(
            "Logarithmic", typeof(bool),
            typeof(LabeledSlider)
            );

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



        
        private double pq(double p, double q) {
            double r1 = -(p / 2);
            double r2 = Math.Sqrt(Math.Pow(p / 2, 2) - q);
            return r1 + r2;
        }

        private double a;
        private double b;
        private double c;

        double m;
        
        private void recalcParams()
        {
            double ymin = Minimum;
            double ymax = Maximum;
            double ysper = Math.Min(1.0, Math.Max(0.0, HalfWay));
            double x1 = slider.Maximum;

            if (ymax == ymin)
            {
                ymax = 10 + ymin;
            }

            double ysmin = ymin * 1.01;
            double ysmax = ((ymin + ymax) / 2) * 0.99;
            double ys = ysmin + ysper * (ysmax - ysmin);
            double d = (ymin - ymax) / (ys - ymin);

            double v1 = pq(d, -1 - d);
            c = 2 * Math.Log(v1) / x1;
            b = (ys - ymin) / (Math.Exp(c * x1 / 2) - 1);
            a = ymin - b;

            // linear
            m = (ymax - ymin) / x1;
            Log.e("SLIDER", $"{Title}: ymax={ymax} ymin={ymin} x1={x1} m={m}");
            if (m == 0)
            {
                m = 0.1;
            }
        }

        private void ValueToSlider()
        {
            if (!interalValueUpdate)
            {
                interalValueUpdate = true;
                double y = Math.Min(Maximum, Math.Max(Minimum, Value));
                if (Logarithmic)
                {
                    slider.Value = Math.Log((y - a) / b) / c;
                }
                else
                {
                    slider.Value = (y - Minimum) / m;
                }
                interalValueUpdate = false;
            }
        }

        private void SliderToValue()
        {
            if (!interalValueUpdate)
            {
                interalValueUpdate = true;
                if (Logarithmic)
                {
                    Value = a + b * Math.Exp(c * slider.Value);
                }
                else
                {
                    Value = Minimum + m * slider.Value;
                    Log.e("SLIDER", $"{Title}: {Value} = {Minimum} {m} * {slider.Value}");
                }
                interalValueUpdate = false;
            }
        }

        public event RoutedPropertyChangedEventHandler<double> ValueChanged;

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SliderToValue();

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
