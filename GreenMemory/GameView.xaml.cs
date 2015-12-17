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

namespace GreenMemory
{
    /// <summary>
    /// Interaction logic for GameWindow.xaml
    /// </summary>
    public partial class GameWindow : UserControl
    {
        private const int FLIPDELAY = 450;

        private MemoryModel gameModel;

        private PlayerModel playerOneModel;
        private PlayerModel playerTwoModel;
        
        private PlayerModel currentPlayerModel;
        private PlayerView currentPlayerView;
        
        private int pickedCard = -1;
        private int numRows = 6;
        private int numColumns = 8;

        /// <summary>
        /// Default constructor
        /// </summary>
        public GameWindow() : this(4, 4) { }

        public GameWindow(int rows, int columns)
        {
            InitializeComponent();
            Tests.Run();
            numRows = rows;
            numColumns = columns;

            newGame();
            ((settings.Content as StackPanel).Children[0] as Button).Click += clickNewGame;
            ((settings.Content as StackPanel).Children[1] as Button).Click += clickSettings;
        }

        /// <summary>
        /// Sets up the board for a new game
        /// </summary>
        private void newGame()
        {
            this.gameModel = new MemoryModel(this.numRows * this.numColumns);

            // TODO: Change apperance of cards to something other then color.
            Brush[] br = new Brush[8]{Brushes.LightBlue, Brushes.Blue, Brushes.Yellow, 
                                        Brushes.Green, Brushes.Red, Brushes.Orange, Brushes.Aqua, Brushes.Maroon};

            Grid cardGrid = this.CardGrid as Grid;

            // Clear gameboard
            cardGrid.Children.Clear();
            cardGrid.RowDefinitions.Clear();
            cardGrid.ColumnDefinitions.Clear();
            this.pickedCard = -1;

            // Set number of rows
            for (int i = 0; i < this.numRows; ++i)
            {
                cardGrid.RowDefinitions.Add(new RowDefinition());
            }

            // Set number of columns
            for (int i = 0; i < this.numColumns; ++i)
            {
                cardGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            CardView.BackgroundImage = Brushes.Black;
            int[] deck = this.gameModel.GetDeck();

            // Set up the cards
            for (int ix = 0; ix < deck.Length; ++ix)
            {
                CardView card = new CardView(br[deck[ix] % 8]);
                Grid.SetColumn(card, (ix % this.numColumns));
                Grid.SetRow(card, (ix / this.numColumns));
                
                card.MouseUp += clickCard;
                card.MouseEnter += mouseEnterCard;
                card.MouseLeave += mouseLeaveCard;
                cardGrid.Children.Add(card);
            }

            // Set up players
            playerOneModel = new PlayerModel("Player One");
            playerTwoModel = new PlayerModel("Player Two");

            playerOneView.name.Content = playerOneModel.Name;
            playerOneView.pairs.Content = 0;
            playerTwoView.name.Content = playerTwoModel.Name;
            playerTwoView.pairs.Content = 0;

            currentPlayerModel = playerOneModel;
            currentPlayerView = playerOneView;
        }

        /// <summary>
        /// Shows all cards for delay ms. 
        /// </summary>
        /// <param name="delay"></param>
        private void showAll(int delay)
        {
            foreach (CardView card in CardGrid.Children)
            {
                if (!card.IsUp())
                {
                    card.FlipCard();
                }
            }

            Task.Delay(delay).ContinueWith(_ =>
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    foreach (CardView card in CardGrid.Children)
                    {
                        card.FlipCard();
                    }
                }));
            });
        }

        /// <summary>
        /// Handler for new game button click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clickNewGame(object sender, RoutedEventArgs e)
        {
            foreach (CardView card in CardGrid.Children)
            {
                if (card.IsUp())
                {
                    card.FlipCard();
                }
            }

            // Delay for cards to be flipped to hidden
            // TODO: Set delay to correct length
            Task.Delay(200).ContinueWith(_ =>
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    newGame();
                }));
            });
        }

        /// <summary>
        /// Handler for settings button click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clickSettings(object sender, RoutedEventArgs e)
        {
            // TODO: Settingswindow
        }

        /// <summary>
        /// Returns the grid index for the given card
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        private int getCardIndex(CardView card)
        {
            return (Grid.GetRow(card) * this.numColumns) + Grid.GetColumn(card);
        }

        /// <summary>
        /// Called when mouse pointer starts hovering over a card
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mouseEnterCard(object sender, MouseEventArgs e)
        {
            (sender as CardView).Grow();
        }


        /// <summary>
        /// Called when mouse pointer stops hovering over a card
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mouseLeaveCard(object sender, MouseEventArgs e)
        {
            (sender as CardView).Shrink();
        }

        /// <summary>
        /// Handler for card click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void clickCard(object sender, MouseButtonEventArgs e)
        {
            CardView card = sender as CardView;

            if (!card.IsUp())
            {
                card.FlipCard();

                if (this.pickedCard != -1)
                {
                    int? correct = this.gameModel.PickTwoCards(this.pickedCard, getCardIndex(card));

                    if (correct != null)
                    {
                        // TODO: Increase score for player.
                        currentPlayerModel.AddCollectedPair(pickedCard);
                        currentPlayerView.pairs.Content = currentPlayerModel.Score;
                        card.IsEnabled = false;
                        this.CardGrid.Children[this.pickedCard].IsEnabled = false;

                        if (this.gameModel.IsGameOver())
                        {
                            // TODO: Show gameover.
                        }
                    }
                    else
                    {
                        currentPlayerModel = currentPlayerModel.Equals(playerOneModel) ? playerTwoModel : playerOneModel;
                        currentPlayerView = currentPlayerView.Equals(playerOneView) ? playerTwoView : playerOneView;

                        CardView secondCard = this.CardGrid.Children[this.pickedCard] as CardView;

                        Task.Delay(FLIPDELAY).ContinueWith(_ =>
                        {
                            this.Dispatcher.Invoke((Action)(() =>
                            {
                                card.FlipCard();
                                secondCard.FlipCard();
                            }));
                        });
                    }

                    this.pickedCard = -1;
                }
                else
                {
                    this.pickedCard = getCardIndex(card);
                }
            }
        }
    }
}
