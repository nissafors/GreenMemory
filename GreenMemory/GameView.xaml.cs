using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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

        /// <summary>
        /// Default constructor
        /// </summary>
        public GameView()
        {
            InitializeComponent();
            this.Background = new ImageBrush(new BitmapImage(new Uri(SettingsModel.GameviewBackgroundPath, UriKind.Relative)));
            // Set colors
            SolidColorBrush bgColor = new SolidColorBrush();
            switch (SettingsModel.Theme)
            {
                // Poker
                case 0:
                    bgColor.Color = Color.FromRgb(0xFF, 0xFF, 0xFF);
                    break;
                // Pokemon
                case 1:
                    bgColor.Color = Color.FromRgb(0xFF, 0xFB, 0x00);
                    break;
                // Nerd
                case 2:
                    bgColor.Color = Color.FromRgb(0xCA, 0x6A, 0x85);
                    break;
                default:
                    bgColor.Color = Color.FromRgb(0x0F, 0x0F, 0x0F);
                    break;

            }
            playerOneView.name.Background = bgColor;
            playerTwoView.name.Background = bgColor;

            settingsWin.imgNewgame.MouseUp += restartGameClick;
            newGame();
        }

        /// <summary>
        /// Sets up the board for a new game
        /// </summary>
        private void newGame()
        {
            this.gameModel = new MemoryModel(SettingsModel.Rows * SettingsModel.Columns);

            // Clear gameboard
            this.CardGrid.Children.Clear();
            this.CardGrid.RowDefinitions.Clear();
            this.CardGrid.ColumnDefinitions.Clear();
            this.pickedCard = NONEPICKED;

            // Set number of rows
            for (int i = 0; i < SettingsModel.Rows; ++i)
            {
                this.CardGrid.RowDefinitions.Add(new RowDefinition());
            }

            // Set number of columns
            for (int i = 0; i < SettingsModel.Columns; ++i)
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
                Grid.SetColumn(card, (ix % SettingsModel.Columns));
                Grid.SetRow(card, (ix / SettingsModel.Columns));
                card.MouseUp += clickCard;
                card.MouseEnter += mouseEnterCard;
                card.MouseLeave += mouseLeaveCard;
                this.CardGrid.Children.Add(card);
            }

            // Set up players
            playerOneModel = new PlayerModel(SettingsModel.TopPlayerName);
            playerTwoModel = new PlayerModel(SettingsModel.BottomPlayerName);

            playerOneView.name.Text = playerOneModel.Name;
            playerOneView.name.IsEnabled = true;
            playerOneView.setPoints(0);
            playerOneView.Active = true;
            playerTwoView.name.Text = playerTwoModel.Name;
            playerTwoView.name.IsEnabled = true;
            playerTwoView.setPoints(0);
            playerTwoView.Active = false;

            currentPlayerModel = playerOneModel;
            currentPlayerView = playerOneView;

            if (SettingsModel.AgainstAI)
            {
                playerTwoModel.Name = "Deep Thought";
                playerTwoView.name.Text = playerTwoModel.Name;
                playerTwoView.name.IsEnabled = false;

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
                catch (TaskCanceledException) { }
            });
        }

        /// <summary>
        /// Handler for new game button click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clickNewGame(object sender, RoutedEventArgs e)
        {
            /*
            foreach (Object child in CardGrid.Children)
            {
                if(child is CardView)
                {
                    CardView card = child as CardView;
                    if (card.IsUp())
                    {
                        if(card.Visibility != Visibility.Visible)
                        {
                            card.Visibility = Visibility.Visible;
                            card.myImage.StrokeThickness = 0;
                        }
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
            */
            newGame();
        }

        /// <summary>
        /// Returns the grid index for the given card
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        private int getCardIndex(CardView card)
        {
            return (Grid.GetRow(card) * SettingsModel.Columns) + Grid.GetColumn(card);
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
            playerOneView.name.IsEnabled = playerTwoView.name.IsEnabled = false;

            CardView card = sender as CardView;

            if (!card.IsUp())
            {
                card.FlipCard();

                if (this.pickedCard != NONEPICKED)
                {
                    bool isCorrect = this.gameModel.PeekTwoCards(this.pickedCard, getCardIndex(card));

                    if (isCorrect)
                    {
                        int firstPickedCard = this.pickedCard;
                        int secondPickedCard = getCardIndex(card);
                        int currentPlayerIndex; // TODO: Send along to movecards for who will receive points

                        // Wait for flip, then update score and animate cards
                        // TODO: Add listener for cardflip
                        Task.Delay(FLIPDELAY).ContinueWith(_ =>
                        {
                            try
                            {
                                this.Dispatcher.Invoke((Action)(() =>
                                {
                                    card.IsEnabled = false;
                                    this.CardGrid.Children[firstPickedCard].IsEnabled = false;
                                    moveCards(firstPickedCard, secondPickedCard);
                                    }));
                            }
                            catch (TaskCanceledException) { }
                        });
                    }
                    else
                    {
                        currentPlayerModel = currentPlayerModel.Equals(playerOneModel) ? playerTwoModel : playerOneModel;
                        currentPlayerView.Active = false;
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
                                    currentPlayerView.Active = true;
                                    runAI();
                                }));
                            }
                            catch (TaskCanceledException) { }
                        });
                    }

                    this.pickedCard = NONEPICKED;
                }
                else
                {
                    this.pickedCard = getCardIndex(card);
                }
            }
        }

        private void runAI()
        {
            if (SettingsModel.AgainstAI
                && currentPlayerModel.Equals(playerTwoModel)
                && !this.gameModel.IsGameOver())
            {
                // AI:s turn. Wake her up.
                aiModel.WakeUp();
            }
        }

        private void moveCards(int firstCardIndex, int secondCardIndex)
        {
            try
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    // Create and start animation for two DummyCards

                    CardView firstCard = this.CardGrid.Children[firstCardIndex] as CardView;
                    DummyCard firstDummyCard = new DummyCard(firstCard, viewBox);
                    Grid.SetColumn(firstDummyCard, Grid.GetColumn(firstCard));
                    Grid.SetRow(firstDummyCard, Grid.GetRow(firstCard));

                    CardView secondCard = this.CardGrid.Children[secondCardIndex] as CardView;
                    DummyCard secondDummyCard = new DummyCard(secondCard, viewBox);
                    Grid.SetColumn(secondDummyCard, Grid.GetColumn(secondCard));
                    Grid.SetRow(secondDummyCard, Grid.GetRow(secondCard));

                    this.CardGrid.Children.Add(firstDummyCard);
                    this.CardGrid.Children.Add(secondDummyCard);

                    double distC = firstDummyCard.distanceTo(currentPlayerView.myStack);
                    double distC2 = secondDummyCard.distanceTo(currentPlayerView.myStack);

                    if (distC > distC2)
                    {
                        firstDummyCard.addCompletedMoveListener((Action)(() => { moveLong(currentPlayerModel, currentPlayerView, firstCardIndex, secondCardIndex, currentPlayerModel.Score + 1); runAI(); }));
                        secondDummyCard.addCompletedMoveListener((Action)(() => { moveShort(currentPlayerView, firstDummyCard.myImage.Fill); }));
                    }
                    else
                    {
                        secondDummyCard.addCompletedMoveListener((Action)(() => { moveLong(currentPlayerModel, currentPlayerView, firstCardIndex, secondCardIndex, currentPlayerModel.Score + 1); runAI(); }));
                        firstDummyCard.addCompletedMoveListener((Action)(() => { moveShort(currentPlayerView, firstDummyCard.myImage.Fill); }));
                    }

                    firstDummyCard.moveFromBoardTo(currentPlayerView.myStack);
                    secondDummyCard.moveFromBoardTo(currentPlayerView.myStack);

                    firstCard.Visibility = Visibility.Hidden;
                    secondCard.Visibility = Visibility.Hidden;
                }));
            }
            catch (TaskCanceledException) { }
        }

        private void backToSettings(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).ChangeView(MainWindow.View.Settings);
        }

        private void openSettingsWindow(object sender, RoutedEventArgs e)
        {
            settingsWin.Visibility = Visibility.Visible;
        }

        private void restartGameClick(object sender, RoutedEventArgs e)
        {
            settingsWin.Visibility = Visibility.Collapsed;
            gameoverWin.Visibility = Visibility.Collapsed;
            clickNewGame(sender, e);
        }

        private void playerOne_nameChanged(object sender, RoutedEventArgs e)
        {
            playerOneModel.Name = playerOneView.name.Text;
            SettingsModel.TopPlayerName = playerOneModel.Name;
        }

        private void playerTwo_nameChanged(object sender, RoutedEventArgs e)
        {
            playerTwoModel.Name = playerTwoView.name.Text;

            if (!SettingsModel.AgainstAI)
            {
                SettingsModel.BottomPlayerName = playerTwoModel.Name;
            }
        }

        private void moveLong(PlayerModel pModel, PlayerView pView, int card0, int card1, int currentScore)
        {
            this.gameModel.PickTwoCards(card0, card1);
            pModel.AddCollectedPair(pickedCard);
            pView.setPoints(currentScore);
            if (this.gameModel.IsGameOver())
            {
                this.gameoverWin.updateScore(playerOneModel.Score, playerTwoModel.Score);
                this.gameoverWin.Visibility = Visibility.Visible;
            }
        }

        private void moveShort(PlayerView pView, Brush br)
        {
            pView.myStack.Fill = br;
        }
    }
}
