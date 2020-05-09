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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace PxKeystrokesWPF
{


    /// <summary>
    /// Interaktionslogik für WindowResizeButton.xaml
    /// </summary>
    public partial class ResizeButton : UserControl
    {

        private Point dragStartCursorPosition;
        private Point lastTickCursorPosition;
        private Point currentCursorPosition;
        private Size dragStartSize;
        private Vector dragStartWindowSizeReal;
        private Vector minSize;

        private Window Window;
        private FrameworkElement CurrentParent;
        private MouseHook mouseHook = null;

        public ResizeButton()
        {
            InitializeComponent();
        }



        public ResizeTarget ResizeTarget
        {
            get { return (ResizeTarget)this.GetValue(ResizeTargetProperty); }
            set { this.SetValue(ResizeTargetProperty, value); }
        }
        public static readonly DependencyProperty ResizeTargetProperty = DependencyProperty.Register(
          "ResizeTarget", typeof(ResizeTarget), typeof(ResizeButton), new PropertyMetadata(ResizeTarget.Window));

        private void resizeWindowButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (mouseHook == null)
            {
                if (ResizeTarget == ResizeTarget.Window)
                {
                    Window = Window.GetWindow(this);
                    dragStartSize = new Size(Window.Width, Window.Height);
                    dragStartWindowSizeReal = PresentationSource.FromVisual(this).CompositionTarget.TransformToDevice.Transform(new Vector(dragStartSize.Width, dragStartSize.Height));
                    minSize = PresentationSource.FromVisual(this).CompositionTarget.TransformToDevice.Transform(new Vector(this.Width, this.Height));
                }
                else
                {
                    CurrentParent = (FrameworkElement) Parent;
                    dragStartSize = new Size(CurrentParent.Width, CurrentParent.Height);
                    minSize = new Vector(this.Width, this.Height);
                }
                mouseHook = new MouseHook();
                mouseHook.MouseEvent += MouseHook_MouseEvent;

                lastTickCursorPosition = currentCursorPosition = dragStartCursorPosition = NativeMethodsMouse.CursorPosition;
            }
        }

        private void MouseHook_MouseEvent(MouseRawEventArgs raw_e)
        {
            if (raw_e.Event == MouseEventType.LBUTTONUP || raw_e.Event == MouseEventType.RBUTTONUP)
            {
                mouseHook.MouseEvent -= MouseHook_MouseEvent;
                mouseHook.Dispose();
                CurrentParent = null;
                Window = null;
                mouseHook = null;
                return;
            }
            else
            {
                Dispatcher.BeginInvoke((Action) tick,
                    DispatcherPriority.Normal);
            }
            
        }

        private void tick()
        {
            currentCursorPosition = NativeMethodsMouse.CursorPosition;

            if (currentCursorPosition != lastTickCursorPosition)
            {
                lastTickCursorPosition = currentCursorPosition;
            
                Vector diffReal = new Vector((currentCursorPosition.X - dragStartCursorPosition.X), (currentCursorPosition.Y - dragStartCursorPosition.Y));
                Vector diffDpiIndependend = PresentationSource.FromVisual(this).CompositionTarget.TransformFromDevice.Transform(diffReal);

                if (Window != null)
                {
                    // This renders once after setting Width and another time after setting Height, causing a visual staircase effect while resizing.
                    //Window.Width = dragStartSize.Width + diffDpiIndependend.X;
                    //Window.Height = dragStartSize.Height + diffDpiIndependend.Y;

                    // As a fix: Use native SetWindowPos() API for smoother resize. The window will automatically redraw itself.
                    NativeMethodsWindow.SetWindowSize(
                        new WindowInteropHelper(Window).Handle,
                        (int)Math.Max(minSize.X, (dragStartWindowSizeReal.X + diffReal.X)),
                        (int)Math.Max(minSize.Y, (dragStartWindowSizeReal.Y + diffReal.Y)));
                }

                if (CurrentParent != null)
                {
                    CurrentParent.Width = Math.Max(minSize.X, dragStartSize.Width + diffDpiIndependend.X);
                    CurrentParent.Height = Math.Max(minSize.Y, dragStartSize.Height + diffDpiIndependend.Y);
                }
            }
        }
    }
    public enum ResizeTarget
    {
        Window,
        Parent
    }

}
