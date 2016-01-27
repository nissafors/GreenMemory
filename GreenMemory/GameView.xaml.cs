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
                // Neon
                case 3:
                    bgColor.Color = Color.FromRgb(0x00, 0xF5, 0xFF);
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
                    new Action<object, MouseEventArgs>(mouseLeaveCard),
                    300,
                    500);
            }
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
                        int currentPlayerIndex = currentPlayerModel.Equals(playerOneModel) ? 1 : 2;

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
                                    moveCards(firstPickedCard, secondPickedCard, currentPlayerIndex);
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
                                    checkForAI();
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

        private void checkForAI()
        {
            if (SettingsModel.AgainstAI
                && currentPlayerModel.Equals(playerTwoModel)
                && !this.gameModel.IsGameOver())
            {
                // AI:s turn. Wake her up.
                aiModel.WakeUp();
            }
        }

        private void moveCards(int firstCardIndex, int secondCardIndex, int playerIndex)
        {
            PlayerView playerView = playerIndex == 1 ? playerOneView : playerTwoView;
            PlayerModel playerModel = playerIndex == 1 ? playerOneModel : playerTwoModel;

            this.Dispatcher.Invoke((Action)(() =>
            {
                DummyCard firstDummyCard = CreateDummyCardInGrid(firstCardIndex);
                DummyCard secondDummyCard = CreateDummyCardInGrid(secondCardIndex);

                double distC = firstDummyCard.distanceTo(currentPlayerView.myStack);
                double distC2 = secondDummyCard.distanceTo(currentPlayerView.myStack);

                if (distC > distC2)
                {
                    firstDummyCard.addCompletedMoveListener((Action)(() =>
                    {
                        removeCards(playerModel, playerView, firstCardIndex, secondCardIndex);
                        checkForAI();
                    }));

                    secondDummyCard.addCompletedMoveListener((Action)(() => { playerView.myStack.Fill = firstDummyCard.myImage.Fill; }));
                }
                else
                {
                    secondDummyCard.addCompletedMoveListener((Action)(() =>
                    {
                        removeCards(playerModel, playerView, firstCardIndex, secondCardIndex);
                        checkForAI();
                    }));
                    firstDummyCard.addCompletedMoveListener((Action)(() => { playerView.myStack.Fill = firstDummyCard.myImage.Fill; }));
                }

                firstDummyCard.moveFromBoardTo(currentPlayerView.myStack);
                secondDummyCard.moveFromBoardTo(currentPlayerView.myStack);

                (this.CardGrid.Children[firstCardIndex] as CardView).Visibility = Visibility.Hidden;
                (this.CardGrid.Children[secondCardIndex] as CardView).Visibility = Visibility.Hidden;
            }));
        }

        private DummyCard CreateDummyCardInGrid(int cardIndex)
        {
            CardView card = this.CardGrid.Children[cardIndex] as CardView;
            DummyCard dummyCard = new DummyCard(card, viewBox);
            Grid.SetColumn(dummyCard, Grid.GetColumn(card));
            Grid.SetRow(dummyCard, Grid.GetRow(card));
            this.CardGrid.Children.Add(dummyCard);

            return dummyCard;
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
            newGame();
        }

        private void topPlayerNameChanged(object sender, RoutedEventArgs e)
        {
            playerOneModel.Name = playerOneView.name.Text;
            SettingsModel.TopPlayerName = playerOneModel.Name;
        }

        private void bottomPlayerNameChanged(object sender, RoutedEventArgs e)
        {
            playerTwoModel.Name = playerTwoView.name.Text;

            if (!SettingsModel.AgainstAI)
            {
                SettingsModel.BottomPlayerName = playerTwoModel.Name;
            }
        }

        private void removeCards(PlayerModel playerModel, PlayerView playerView, int firstCardIndex, int secondCardIndex)
        {
            this.gameModel.PickTwoCards(firstCardIndex, secondCardIndex);
            playerModel.AddCollectedPair(pickedCard);
            playerView.setPoints(playerModel.Score);

            if (this.gameModel.IsGameOver())
            {
                this.gameoverWin.updateScore(playerOneModel.Score, playerTwoModel.Score);
                this.gameoverWin.Visibility = Visibility.Visible;
            }
        }
    }
}
