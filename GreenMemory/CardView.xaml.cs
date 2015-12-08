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
    /// Interaction logic for CardView.xaml
    /// </summary>
    public partial class CardView : UserControl 
    {
        private static Brush backgroundImage = Brushes.Wheat;
        // animation time in milliseconds for the entire animation
        private static int animationDuration = 200;
        
        private Brush cardImage;
        private Thickness currentMargin;
        bool isUp = false;

        public static Brush BackgroundImage
        {
            get { return backgroundImage; }
            set { backgroundImage = value; }
        }

        public static int AnimationDuration
        {
            get { return animationDuration; }
            set { animationDuration = value; }
        }


        public CardView(Brush cardImage)
        {
            InitializeComponent();
            //this.Background = backgroundImage;
            currentMargin = this.myImage.Margin;
            this.myImage.Fill = backgroundImage;
            this.cardImage = cardImage;
        }

        /// <summary>
        /// Does a flip animation & changes the cardImage
        /// </summary>
        public void FlipCard()
        {
            this.isUp = !this.isUp;

            DoubleAnimation anim0 = new DoubleAnimation();
            anim0.From = this.ActualWidth;
            anim0.To = 0;
            anim0.Duration = new Duration(TimeSpan.FromMilliseconds(animationDuration / 2));
            anim0.FillBehavior = FillBehavior.Stop;

            DoubleAnimation anim1 = new DoubleAnimation();
            anim1.From = 0;
            anim1.To = this.ActualWidth;
            anim1.Duration = new Duration(TimeSpan.FromMilliseconds(animationDuration / 2));
            anim1.FillBehavior = FillBehavior.Stop;

            anim0.Completed += (sender, eArgs) => 
            {
                if (this.myImage.Fill == backgroundImage)
                    this.myImage.Fill = cardImage;
                else
                    this.myImage.Fill = backgroundImage;

                this.myImage.BeginAnimation(WidthProperty, anim1);
            };

            this.myImage.BeginAnimation(WidthProperty, anim0);
        }

        /// <summary>
        /// Let myImage fill the entire control
        /// </summary>
        public void Grow()
        {
            ThicknessAnimation animSize = new ThicknessAnimation();
            animSize.From = this.myImage.Margin;
            animSize.To = new Thickness(0);
            animSize.Duration = new Duration(TimeSpan.FromMilliseconds(animationDuration / 2));
            animSize.FillBehavior = FillBehavior.Stop;
            
            this.myImage.Margin = new Thickness(0);
            
            this.myImage.BeginAnimation(MarginProperty, animSize);
        }

        /// <summary>
        /// Reset myImage to original size
        /// </summary>
        public void Shrink()
        {
            ThicknessAnimation animSize = new ThicknessAnimation();
            animSize.From = this.myImage.Margin;
            animSize.To = this.currentMargin;
            animSize.Duration = new Duration(TimeSpan.FromMilliseconds(animationDuration / 2));
            animSize.FillBehavior = FillBehavior.Stop;
            
            this.myImage.Margin = this.currentMargin;

            this.myImage.BeginAnimation(MarginProperty, animSize);
            
        }
        public bool IsUp()
        {
            return this.isUp;
        }
    }
}
