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
        private static ImageBrush backgroundImage;
        // animation time in milliseconds for the entire animation
        private static int animationDuration = 200;
        
        private ImageBrush cardImage;
        private Thickness currentMargin;
        bool isUp = false;

        private List<Action> flipListeners = new List<Action>();

        public static void UpdateBackground()
        {
            backgroundImage = new ImageBrush(new BitmapImage(new Uri(System.IO.Path.Combine(SettingsModel.CardImagePath, "Backside\\Backside.png"), UriKind.Relative)));
        }

        public static int AnimationDuration
        {
            get { return animationDuration; }
            set { animationDuration = value; }
        }
        /// <summary>
        /// Adds a callback that activates when a flip animation is completed
        /// </summary>
        /// <param name="f"></param>
        public void addFlipListener(Action f)
        {
            flipListeners.Add(f);
        }

        /// <summary>
        /// UNTESTED!!! remove a specific listener
        /// </summary>
        /// <param name="f"></param>
        public void removeFlipListener(Action f)
        {
            flipListeners.Remove(f);
        }

        /// <summary>
        /// Remove all listeners
        /// </summary>
        public void clearFlipListeners()
        {
            flipListeners.Clear();
        }

        public ImageBrush CardImage
        {
            get { return cardImage; }
        }

        public CardView(string cardImage)
        {
            if (backgroundImage == null)
            {
                UpdateBackground();
            }
            this.cardImage = new ImageBrush(new BitmapImage(new Uri(cardImage, UriKind.Relative)));
            InitializeComponent();

            currentMargin = this.myImage.Margin;
            this.myImage.Fill = backgroundImage;
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
                if (this.isUp)
                    this.myImage.Fill = cardImage;
                else
                    this.myImage.Fill = backgroundImage;

                this.myImage.BeginAnimation(WidthProperty, anim1);
            };

            // call all listeners
            anim1.Completed += (sender, eArgs) =>
                {
                    foreach (Action f in flipListeners)
                        f();
                };

            this.myImage.BeginAnimation(WidthProperty, anim0);
            SoundControl.Player.playSound(SoundControl.SoundType.Flip);
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

        public void MoveTo(UIElement to)
        {
            Point posFrom = this.PointToScreen(new Point(0, 0));
            Point posTo = to.PointToScreen(new Point(0, 0));
            TranslateTransform tt = new TranslateTransform(posTo.X - posFrom.X, posTo.Y - posFrom.Y);
            DoubleAnimation animX = new DoubleAnimation();
            animX.From = 0;
            animX.To = posTo.X - posFrom.X;
            animX.Duration = new Duration(TimeSpan.FromMilliseconds(animationDuration * 2));

            DoubleAnimation animY = new DoubleAnimation();
            animY.From = 0;
            animY.To = posTo.Y - posFrom.Y;
            animY.Duration = new Duration(TimeSpan.FromMilliseconds(animationDuration * 2));

            animY.Completed += (sender, eArgs) =>
                {
                    this.IsEnabled = false;
                    this.Visibility = Visibility.Hidden;
                };
            tt.BeginAnimation(TranslateTransform.XProperty, animX);
            tt.BeginAnimation(TranslateTransform.YProperty, animY);
            this.RenderTransform = tt;
        }
        public bool IsUp()
        {
            return this.isUp;
        }
    }
}
