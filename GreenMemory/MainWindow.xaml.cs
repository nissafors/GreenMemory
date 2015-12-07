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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;

namespace GreenMemory
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MemoryModel gameModel;

        public MainWindow()
        {
            InitializeComponent();

            gameModel = new MemoryModel(16);

            //TODO: Fill CardGrid with CardView
            Brush[] br = new Brush[8]{Brushes.LightBlue, Brushes.Blue, Brushes.Yellow, 
                                        Brushes.Green, Brushes.Red, Brushes.Orange, Brushes.Aqua, Brushes.Maroon};
            UniformGrid cardGrid = this.CardGrid as UniformGrid;
            CardView.BackgroundImage = Brushes.Black;
            cardGrid.Rows = 6;
            cardGrid.Columns = 6;

            for(int ix = 0; ix < 36; ++ix)
            {
                CardView card = new CardView(ix % 16, br[ix % 8]);
                card.Margin = new Thickness(5);
                card.MouseUp += clickCard;
                cardGrid.Children.Add(card);
            }
        }

        void clickCard(object sender, MouseButtonEventArgs e)
        {
            CardView card = sender as CardView;
            card.FlipCard();
        }
    }
}
