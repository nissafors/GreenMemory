using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Threading;
using System.Windows.Threading;

namespace GreenMemory
{
    /// <summary>
    /// Interaction logic for GameWindow.xaml
    /// </summary>
    public partial class GameView : UserControl
    {
        private const int FLIPDELAY = 450; // How long the cards will be shown

        private MemoryModel gameModel;

        private PlayerModel playerOneModel;
        private PlayerModel playerTwoModel;

        private PlayerModel currentPlayerModel;
        private PlayerView currentPlayerView;

        private AIModel aiModel;
        private String aiName;

        private int dummyPairsInPlay = 0;

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
                    aiName="LE CHIFFRE";
                    break;
                // Pokemon
                case 1:
                    bgColor.Color = Color.FromRgb(0xFF, 0xFB, 0x00);
                    aiName = "TEAM ROCKET";
                    break;
                // Nerd
                case 2:
                    bgColor.Color = Color.FromRgb(0xCA, 0x6A, 0x85);
                    aiName = "DEEP THOUGHT";
                    break;
                // Neon
                case 3:
                    bgColor.Color = Color.FromRgb(0x00, 0xF5, 0xFF);
                    aiName = "HAL 9000";
                    break;
                default:
                    bgColor.Color = Color.FromRgb(0x0F, 0x0F, 0x0F);
                    break;

            }
            playerOneView.name.Background = bgColor;
            playerTwoView.name.Background = bgColor;

            settingsWin.imgNewgame.MouseUp += restartGameClick;
            playerOneView.addFadeCompleteListener((Action)checkForAIOrPlayer);
            playerTwoView.addFadeCompleteListener((Action)checkForAIOrPlayer);

            // Initialize sounds and music player
            SoundControl player = SoundControl.Player;
            if (SettingsModel.Music)
                player.playMusic();

            // GO!
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
            focus(playerOneView.name);
            playerOneView.name.SelectAll();
            playerTwoView.name.Text = playerTwoModel.Name;
            playerTwoView.name.IsEnabled = true;
            playerTwoView.setPoints(0);
            playerTwoView.Active = true;

            currentPlayerModel = playerOneModel;
            currentPlayerView = playerOneView;

            if (SettingsModel.AgainstAI)
            {
                playerTwoModel.Name = aiName;
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

            checkForAIOrPlayer();
        }

        /// <summary>
        /// Handler for card click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clickCard(object sender, MouseButtonEventArgs e)
        {
            playerOneView.name.IsEnabled = playerTwoView.name.IsEnabled = false;
            CardView card = sender as CardView;
            
            if (!card.IsUp() && this.gameModel.PickCard(this.getCardIndex(card)))
            {
                if (this.gameModel.TwoCardsPicked)
                {
                    if (this.gameModel.CorrectPair)
                    {
                        // Save current state
                        int firstPickedCard = (int)this.gameModel.FirstCardIndex;
                        int secondPickedCard = (int)this.gameModel.SecondCardIndex;
                        PlayerModel playerModel = currentPlayerModel;
                        PlayerView playerView = currentPlayerView;
                        ++dummyPairsInPlay;

                        // Disable pair and add score to current player
                        this.CardGrid.Children[(int)this.gameModel.FirstCardIndex].IsEnabled = false;
                        this.CardGrid.Children[(int)this.gameModel.SecondCardIndex].IsEnabled = false;
                        currentPlayerModel.AddCollectedPair((int)this.gameModel.FirstCardIndex);

                        card.addFlipListener((Action)(() =>
                        {
                            this.gameModel.ClearPicked();
                            checkForAIOrPlayer();
                            Task.Delay(FLIPDELAY).ContinueWith(_ =>
                            {
                                try
                                {
                                    this.Dispatcher.Invoke((Action)(() =>
                                    {
                                        // Create dummy cards and move them to the players stack
                                        DummyCard firstDummyCard = CreateDummyCardInGrid(firstPickedCard);
                                        DummyCard secondDummyCard = CreateDummyCardInGrid(secondPickedCard);
                                        

                                        if (firstDummyCard.distanceTo(currentPlayerView.myStack) > secondDummyCard.distanceTo(currentPlayerView.myStack))
                                        {
                                            firstDummyCard.addCompletedMoveListener((Action)(() => { updateScore(); checkGameOver(); }));

                                            secondDummyCard.addCompletedMoveListener((Action)(() => { playerView.myStack.Fill = firstDummyCard.myImage.Fill; }));
                                        }
                                        else
                                        {
                                            secondDummyCard.addCompletedMoveListener((Action)(() => { updateScore(); checkGameOver(); }));

                                            firstDummyCard.addCompletedMoveListener((Action)(() => { playerView.myStack.Fill = firstDummyCard.myImage.Fill; }));
                                        }

                                        firstDummyCard.moveFromBoardTo(currentPlayerView.myStack);
                                        secondDummyCard.moveFromBoardTo(currentPlayerView.myStack);

                                        (this.CardGrid.Children[firstPickedCard] as CardView).Visibility = Visibility.Hidden;
                                        (this.CardGrid.Children[secondPickedCard] as CardView).Visibility = Visibility.Hidden;
                                    }));
                                }
                                catch (TaskCanceledException) { }

                            });
                        }));
                        
                    }
                    else
                    {
                        // Disable the cards then switch player
                        CardView firstCard = this.CardGrid.Children[(int)this.gameModel.FirstCardIndex] as CardView;
                        this.CardGrid.IsEnabled = false;

                        card.addFlipListener((Action)(() =>
                        {
                            currentPlayerModel = currentPlayerModel.Equals(playerOneModel) ? playerTwoModel : playerOneModel;
                            currentPlayerView.Active = false;
                            currentPlayerView = currentPlayerView.Equals(playerOneView) ? playerTwoView : playerOneView;

                            this.gameModel.ClearPicked();

                            Task.Delay(FLIPDELAY).ContinueWith(_ =>
                            {
                                try
                                {
                                    this.Dispatcher.Invoke((Action)(() =>
                                    {
                                        card.FlipCard();
                                        firstCard.FlipCard();
                                        card.clearFlipListeners();
                                        currentPlayerView.Active = true;
                                    }));
                                }
                                catch (TaskCanceledException) { }
                            });
                        }));
                    }
                }
                // Flipcard must be done after adding listeners
                card.FlipCard();
            }
        }

        private void checkForAIOrPlayer()
        {
            if (!this.gameModel.IsGameOver())
            {
                if (SettingsModel.AgainstAI
                    && currentPlayerModel.Equals(playerTwoModel))
                {
                    // AI:s turn. Wake her up.
                    CardGrid.IsEnabled = false;
                    aiModel.WakeUp();
                }
                else
                {
                    if (!(playerOneView.name.IsKeyboardFocused || playerTwoView.name.IsKeyboardFocused))
                        CardGrid.IsEnabled = true;
                }
            }
            else
            {
                CardGrid.IsEnabled = false;
            }
        }

        private void updateScore()
        {
            playerTwoView.setPoints(playerTwoModel.Score);
            playerOneView.setPoints(playerOneModel.Score);
        }

        private void checkGameOver()
        {
            --dummyPairsInPlay;

            if (this.gameModel.IsGameOver() && dummyPairsInPlay < 1)
            {
                this.gameoverWin.updateScore(playerOneModel.Score, playerTwoModel.Score);
                this.gameoverWin.Visibility = Visibility.Visible;
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
        /// Creates and inserts a dummycard into the grid where the 
        /// specified card is. Returns said dummycard
        /// </summary>
        /// <param name="cardIndex"></param>
        /// <returns></returns>
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
            aiModel.KillThreads();
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

        /// <summary>
        /// Called when the name of the top player is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void topPlayerNameChanged(object sender, RoutedEventArgs e)
        {
            playerOneModel.Name = playerOneView.name.Text;
            SettingsModel.TopPlayerName = playerOneModel.Name;
        }

        /// <summary>
        /// Called when the name of the bottom player is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bottomPlayerNameChanged(object sender, RoutedEventArgs e)
        {
            playerTwoModel.Name = playerTwoView.name.Text;

            if (!SettingsModel.AgainstAI)
            {
                SettingsModel.BottomPlayerName = playerTwoModel.Name;
            }
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
        /// Called when a name textbox of one of the players is clicked. Activate both and
        /// disable cards while editing.
        /// </summary>
        private void playerNameGotKeyboardFocus(object sender, RoutedEventArgs e)
        {
            CardGrid.IsEnabled = false;
            playerOneView.Active = true;
            playerTwoView.Active = true;
        }

        /// <summary>
        /// 
        /// </summary>
        private void settingsWin_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(SettingsModel.AgainstAI)
            {
                if (settingsWin.Visibility == Visibility.Visible)
                    aiModel.Pause = true;
                else
                    aiModel.Pause = false;
            }
            
        }

        private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (playerOneView.name.IsKeyboardFocused || playerTwoView.name.IsKeyboardFocused)
            {
                Keyboard.ClearFocus();
                CardGrid.IsEnabled = true;
                playerOneView.Active = true;
                playerTwoView.Active = false;
                e.Handled = true;
            }
        }

        private void focus(UIElement element)
        {
            if (!element.Focus())
            {
                element.Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(delegate()
                {
                    element.Focus();
                }));
            }
        }
    }
}
