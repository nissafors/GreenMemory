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
        private double width = 0;
        private double height = 0;
        public DummyCard()
        {
            InitializeComponent();
        }

        public DummyCard(DummyCard parentDummy)
        {
            InitializeComponent();
            //myImage.Fill = parentDummy.myImage.Fill;
            this.Width = parentDummy.myImage.ActualWidth;
            this.Height = parentDummy.myImage.ActualHeight;
        }
        public DummyCard(CardView parentCard)
        {
            InitializeComponent();
            //myImage.Fill = parentCard.myImage.Fill;
            this.Width = parentCard.myImage.ActualWidth;
            this.Height = parentCard.myImage.ActualHeight;;
        }

        public void moveFromBoardTo(UIElement to)
        {
            Point posFrom = this.PointToScreen(new Point(0, 0));
            Point posTo = to.PointToScreen(new Point(0, 0));
            TranslateTransform tt = new TranslateTransform((posTo.X - posFrom.X), (posTo.Y - posFrom.Y));
            DoubleAnimation animX = new DoubleAnimation();
            animX.From = 0;
            animX.To = posTo.X - posFrom.X;
            animX.Duration = new Duration(TimeSpan.FromMilliseconds(1000));

            DoubleAnimation animY = new DoubleAnimation();
            animY.From = 0;
            animY.To = posTo.Y - posFrom.Y;
            animY.Duration = new Duration(TimeSpan.FromMilliseconds(1000));

            animY.Completed += (sender, eArgs) =>
            {
                //this.IsEnabled = false;
                //this.Visibility = Visibility.Hidden;
            };
            tt.BeginAnimation(TranslateTransform.XProperty, animX);
            tt.BeginAnimation(TranslateTransform.YProperty, animY);
            this.RenderTransform = tt;
        }

    }
}
