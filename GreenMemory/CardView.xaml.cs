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
            this.Background = backgroundImage;
            this.cardImage = cardImage;
        }

        public void FlipCard()
        {
            this.isUp = !this.isUp;

            DoubleAnimation anim0 = new DoubleAnimation();
            anim0.From = this.ActualWidth;
            anim0.To = 0;
            anim0.Duration = new Duration(TimeSpan.FromMilliseconds(animationDuration / 2));

            DoubleAnimation anim1 = new DoubleAnimation();
            anim1.From = 0;
            anim1.To = this.ActualWidth;
            anim1.Duration = new Duration(TimeSpan.FromMilliseconds(animationDuration / 2));

            anim0.Completed += (sender, eArgs) => 
            {
                if (this.Background == backgroundImage)
                    this.Background = cardImage;
                else
                    this.Background = backgroundImage;

                this.BeginAnimation(WidthProperty, anim1);
            };

            this.BeginAnimation(WidthProperty, anim0);

        }

        public bool IsUp()
        {
            return this.isUp;
        }
    }
}
