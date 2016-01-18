using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
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
using System.IO;

namespace GreenMemory
{
    /// <summary>
    /// Interaction logic for GameWindow.xaml
    /// </summary>
    public partial class GameView : UserControl
    {
        private const int NONEPICKED = -1;
        private const int FLIPDELAY = 450;

        private MemoryModel gameModel;

        private PlayerModel playerOneModel;
        private PlayerModel playerTwoModel;
        
        private PlayerModel currentPlayerModel;
        private PlayerView currentPlayerView;

        private AIModel aiModel;
        
        private int pickedCard = NONEPICKED;
        private int numRows = 6;
        private int numColumns = 8;

        /// <summary>
        /// Default constructor
        /// </summary>
        public GameView()
        {
            InitializeComponent();
            numRows = SettingsModel.Rows;
            numColumns = SettingsModel.Columns;
            this.Background = new ImageBrush(new BitmapImage(new Uri(SettingsModel.GameviewBackgroundPath, UriKind.Relative)));

            newGame();
            this.btnNewGame.Click += clickNewGame;
            //((settings.Content as StackPanel).Children[1] as AnimatedButton).Click += clickSettings;
        }

        /// <summary>
        /// Sets up the board for a new game
        /// </summary>
        private void newGame()
        {
            this.gameModel = new MemoryModel(this.numRows * this.numColumns);

            // Clear gameboard
            this.CardGrid.Children.Clear();
            this.CardGrid.RowDefinitions.Clear();
            this.CardGrid.ColumnDefinitions.Clear();
            this.pickedCard = NONEPICKED;

            // Set number of rows
            for (int i = 0; i < this.numRows; ++i)
            {
                this.CardGrid.RowDefinitions.Add(new RowDefinition());
            }

            // Set number of columns
            for (int i = 0; i < this.numColumns; ++i)
            {
                this.CardGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            int[] deck = this.gameModel.GetDeck();
            string[] cardImages = Directory.GetFiles(SettingsModel.CardImagePath);
            Random rand = new Random();
            cardImages = cardImages.OrderBy(x => rand.Next()).ToArray();

            // Set up the cards
            for (int ix = 0; ix < this.gameModel.NumberOfCards; ++ix)
            {
                CardView card = new CardView(cardImages[deck[ix]]);
                Grid.SetColumn(card, (ix % this.numColumns));
                Grid.SetRow(card, (ix / this.numColumns));
                card.MouseUp += clickCard;
                card.MouseEnter += mouseEnterCard;
                card.MouseLeave += mouseLeaveCard;
                this.CardGrid.Children.Add(card);
            }

            // Set up players
            playerOneModel = new PlayerModel("Player One");
            playerTwoModel = new PlayerModel("Player Two");

            playerOneView.name.Content = playerOneModel.Name;
            //playerOneView.pairs.Content = 0;
            playerTwoView.name.Content = playerTwoModel.Name;
           // playerTwoView.pairs.Content = 0;

            currentPlayerModel = playerOneModel;
            currentPlayerView = playerOneView;

            if (SettingsModel.AgainstAI)
            {
                if (aiModel != null)
                    aiModel.KillThreads();

                aiModel = new AIModel(gameModel,
                    this.CardGrid,
                    new Action<object, MouseButtonEventArgs>(clickCard),
                    new Action<object, MouseEventArgs>(mouseEnterCard),
                    new Action<object, MouseEventArgs>(mouseLeaveCard));
            }
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
                try
                {
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        foreach (CardView card in CardGrid.Children)
                        {
                            card.FlipCard();
                        }
                    }));
                }
                catch (TaskCanceledException) {}
            });
        }

        /// <summary>
        /// Handler for new game button click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clickNewGame(object sender, RoutedEventArgs e)
        {
            foreach (Object child in CardGrid.Children)
            {
                if(child is CardView)
                {
                    CardView card = child as CardView;
                    if (card.IsUp())
                    {
                        card.FlipCard();
                    }
                }

            }

            // Delay for cards to be flipped to hidden
            // TODO: Set delay to correct length
            Task.Delay(200).ContinueWith(_ =>
            {
                try
                {
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        newGame();
                    }));
                }
                catch (TaskCanceledException) {}
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

                if (this.pickedCard != NONEPICKED)
                {
                    int? correct = this.gameModel.PickTwoCards(this.pickedCard, getCardIndex(card));

                    if (correct != null)
                    {
                        int tmpPickedCard = this.pickedCard;

                        // Wait for flip, then update score and animate cards
                        Task.Delay(FLIPDELAY).ContinueWith(_ =>
                        {
                            try
                            {
                                this.Dispatcher.Invoke((Action)(() =>
                                {
                                    
                                    card.IsEnabled = false;
                                    this.CardGrid.Children[tmpPickedCard].IsEnabled = false;

                                    // Create and start animation for two DummyCards
                                    DummyCard c = new DummyCard(card, viewBox);
                                    int col = Grid.GetColumn(card);
                                    int row = Grid.GetRow(card);
                                    Grid.SetColumn(c, col);
                                    Grid.SetRow(c, row);

                                    DummyCard c2 = new DummyCard(this.CardGrid.Children[tmpPickedCard] as CardView, viewBox);
                                    col = Grid.GetColumn(this.CardGrid.Children[tmpPickedCard]);
                                    row = Grid.GetRow(this.CardGrid.Children[tmpPickedCard]);

                                    Grid.SetColumn(c2, col);
                                    Grid.SetRow(c2, row);

                                    this.CardGrid.Children.Add(c);
                                    this.CardGrid.Children.Add(c2);

                                    c.addCompletedMoveListener((Action)(() =>
                                    {
                                        currentPlayerView.myStack.Fill = c.myImage.Fill;
                                        currentPlayerModel.AddCollectedPair(pickedCard);
                                        currentPlayerView.pairs.Content = currentPlayerModel.Score;
                                    }));
                                    c.moveFromBoardTo(currentPlayerView.myStack);
                                    c2.moveFromBoardTo(currentPlayerView.myStack);

                                    card.Visibility = Visibility.Hidden;
                                    this.CardGrid.Children[tmpPickedCard].Visibility = Visibility.Hidden;
                                }));
                            }
                            catch (TaskCanceledException) { }
                        });

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
                            try
                            {
                                this.Dispatcher.Invoke((Action)(() =>
                                {
                                    card.FlipCard();
                                    secondCard.FlipCard();
                                }));
                            }
                            catch (TaskCanceledException) {}
                        });
                    }

                    this.pickedCard = NONEPICKED;

                    if (SettingsModel.AgainstAI
                        && currentPlayerModel.Equals(playerTwoModel)
                        && !this.gameModel.IsGameOver())
                    {
                        // AI:s turn. Wake her up.
                        aiModel.WakeUp();
                    }
                }
                else
                {
                    this.pickedCard = getCardIndex(card);
                }
            }
        }
    }
}
