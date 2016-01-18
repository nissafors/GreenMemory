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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GreenMemory
{
    /// <summary>
    /// Interaction logic for AnimatedButton.xaml
    /// </summary>
    public partial class AnimatedButton : UserControl
    {
        private Thickness currentMargin;
        private static int animationDuration = 50;
        // Add databindings so the image & label content in the control can be changed through xaml
        public static readonly DependencyProperty ButtonImageProperty =
            DependencyProperty.Register("ButtonImage", typeof(ImageSource), typeof(AnimatedButton), new UIPropertyMetadata(null));

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(AnimatedButton), new UIPropertyMetadata(null));

        // Enables to use click handler with xaml like a button
        public event RoutedEventHandler Click;
        public ImageSource ButtonImage
        {
            get { return GetValue(ButtonImageProperty) as ImageSource;}
            set { SetValue(ButtonImageProperty, value);}
        }

        public string Text
        {
            get { return GetValue(TextProperty) as string; }
            set { SetValue(TextProperty, value);}
        }

        public AnimatedButton()
        {
            InitializeComponent();
            this.currentMargin = this.myImage.Margin;
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {

            ThicknessAnimation animSize = new ThicknessAnimation();
            animSize.From = this.myImage.Margin;
            animSize.To = new Thickness(0);
            animSize.Duration = new Duration(TimeSpan.FromMilliseconds(animationDuration / 2));
            animSize.FillBehavior = FillBehavior.Stop;

            this.myImage.Margin = new Thickness(0);

            this.myImage.BeginAnimation(MarginProperty, animSize);
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            ThicknessAnimation animSize = new ThicknessAnimation();
            animSize.From = this.myImage.Margin;
            animSize.To = this.currentMargin;
            animSize.Duration = new Duration(TimeSpan.FromMilliseconds(animationDuration / 2));
            animSize.FillBehavior = FillBehavior.Stop;

            this.myImage.Margin = this.currentMargin;

            this.myImage.BeginAnimation(MarginProperty, animSize);
        }

        private void UserControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if(Click != null)
            {
                Click(this, e);
            }
        }
    }
}
