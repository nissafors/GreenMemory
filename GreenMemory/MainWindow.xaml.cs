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
        private int pickedCard = -1;
        private int rows = 4;
        private int columns = 4;

        public MainWindow()
        {
            InitializeComponent();

            gameModel = new MemoryModel(rows * columns);

            Brush[] br = new Brush[8]{Brushes.LightBlue, Brushes.Blue, Brushes.Yellow, 
                                        Brushes.Green, Brushes.Red, Brushes.Orange, Brushes.Aqua, Brushes.Maroon};
            Grid cardGrid = this.CardGrid as Grid;
            int[] deck = gameModel.GetDeck();

            for (int i = 0; i < rows; ++i)
            {
                cardGrid.RowDefinitions.Add(new RowDefinition());
            }
            
            for (int i = 0; i < columns; ++i)
            {
                cardGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            CardView.BackgroundImage = Brushes.Black;

            for(int ix = 0; ix < deck.Length; ++ix)
            {
                CardView card = new CardView(deck[ix], br[deck[ix]]);
                card.Margin = new Thickness(5);
                Grid.SetColumn(card, (ix % columns));
                Grid.SetRow(card, (ix / rows));
                card.MouseUp += clickCard;
                cardGrid.Children.Add(card);
            }
        }

        void clickCard(object sender, MouseButtonEventArgs e)
        {
            CardView card = sender as CardView;

            if (!card.IsUp())
            {
                card.FlipCard();

                if (pickedCard != -1)
                {
                    int row = Grid.GetRow(card);
                    int column = Grid.GetColumn(card);
                    int? correct = gameModel.PickTwoCards(pickedCard, (row * columns) + column, 1);

                    if (correct != null)
                    {
                        if (gameModel.IsGameOver())
                        {
                            // TODO: Show gameover.
                        }
                        else
                        {
                            //player1.pairs.Content = gameModel.GetScore(1);
                        }
                    }
                    else
                    {
                        // TODO: Add delay
                        card.FlipCard();
                        CardView secondCard = this.CardGrid.Children[pickedCard] as CardView;
                        secondCard.FlipCard();
                    }

                    pickedCard = -1;
                }
                else
                {
                    int row = Grid.GetRow(card);
                    int column = Grid.GetColumn(card);
                    pickedCard = (row * columns) + column;

                }
            }
        }
    }
}
