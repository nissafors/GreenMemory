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

        public void moveFromBoardTo(UIElement to)
        {
            Point posTo = to.TranslatePoint(new Point(0, 0), Application.Current.MainWindow);
            DoubleAnimation animX = new DoubleAnimation();
            animX.From = 0;
            animX.To = (posTo.X - parentPos.X) / scale / (to.RenderSize.Width / this.Width / scale);
            animX.Duration = new Duration(TimeSpan.FromMilliseconds(500));

            DoubleAnimation animY = new DoubleAnimation();
            animY.From = 0;
            animY.To = (posTo.Y - parentPos.Y) / scale / (to.RenderSize.Width / this.Width / scale);
            animY.Duration = new Duration(TimeSpan.FromMilliseconds(500));

            animY.Completed += (sender, eArgs) =>
            {
                this.IsEnabled = false;
                this.Visibility = Visibility.Hidden;
                foreach(Action func in completedMove)
                {
                    func();
                }
            };

            DoubleAnimation scaleAnim = new DoubleAnimation();
            scaleAnim.From = 1;
            scaleAnim.To = to.RenderSize.Width / this.Width / scale;
            scaleAnim.Duration = new Duration(TimeSpan.FromMilliseconds(500));

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
