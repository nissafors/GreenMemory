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
    /// Interaction logic for DummyCard.xaml
    /// </summary>
    public partial class DummyCard : UserControl
    {
        Point parentPos;
        double scale;
        List<Action> completedMove = new List<Action>();

        public void addCompletedMoveListener(Action func)
        {
            completedMove.Add(func);
        }

        public DummyCard()
        {
            InitializeComponent();
        }
        public DummyCard(CardView parentCard, Viewbox viewBox)
        {
            InitializeComponent();

            parentPos = parentCard.TranslatePoint(new Point(0, 0), Application.Current.MainWindow);
            myImage.Fill = parentCard.CardImage;
            
            this.Width = parentCard.myImage.ActualWidth;
            this.Height = parentCard.myImage.ActualHeight;

            var child = VisualTreeHelper.GetChild(viewBox, 0) as ContainerVisual;
            scale = (child.Transform as ScaleTransform).ScaleX;
        }

        // get distance to element
        public double distanceTo(UIElement to)
        {
            Point posTo = to.TranslatePoint(new Point(0, 0), Application.Current.MainWindow);

            Vector distance = posTo - parentPos;
            return distance.Length;
        }
        // animates a move animation and scaling
        public void moveFromBoardTo(UIElement to)
        {
            Point posTo = to.TranslatePoint(new Point(0, 0), Application.Current.MainWindow);

            // Use a vector create a constant speed using 
            Vector distance = posTo - parentPos;
            const double SPEED = 1.28; // testade fram detta värde 
            // t = s / v
            TimeSpan time = TimeSpan.FromMilliseconds(distance.Length / SPEED);
            DoubleAnimation animX = new DoubleAnimation();
            animX.From = 0;
            animX.To = (posTo.X - parentPos.X) / scale / (to.RenderSize.Width / this.Width / scale);
            animX.Duration = new Duration(time);

            DoubleAnimation animY = new DoubleAnimation();
            animY.From = 0;
            animY.To = (posTo.Y - parentPos.Y) / scale / (to.RenderSize.Width / this.Width / scale);
            animY.Duration = new Duration(time);

            animY.Completed += (sender, eArgs) =>
            {
                this.IsEnabled = false;
                this.Visibility = Visibility.Hidden;
                foreach(Action func in completedMove)
                {
                    func();
                }
            };

            // Dessa är s.k easing functioner som gör att hastigheten följer en kurva
            // Då Timspan time beräknas med en linjär formel kanske dessa borde tas bort
            animX.EasingFunction = new PowerEase();
            animY.EasingFunction = new PowerEase();

            DoubleAnimation scaleAnim = new DoubleAnimation();
            scaleAnim.From = 1;
            scaleAnim.To = to.RenderSize.Width / this.Width / scale;
            scaleAnim.Duration = new Duration(time);

            scaleAnim.EasingFunction = new PowerEase();

            TranslateTransform tt = new TranslateTransform();
            ScaleTransform st = new ScaleTransform();

            tt.BeginAnimation(TranslateTransform.XProperty, animX);
            tt.BeginAnimation(TranslateTransform.YProperty, animY);

            st.BeginAnimation(ScaleTransform.ScaleXProperty, scaleAnim);
            st.BeginAnimation(ScaleTransform.ScaleYProperty, scaleAnim);

            TransformGroup tg = new TransformGroup();
            tg.Children.Add(tt);
            tg.Children.Add(st);
            this.RenderTransform = tg;
        }

    }
}
