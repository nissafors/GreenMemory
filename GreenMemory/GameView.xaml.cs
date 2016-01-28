﻿using System;
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
        private const int FLIPDELAY = 450; // How long the cards will be shown

        private MemoryModel gameModel;

        private PlayerModel playerOneModel;
        private PlayerModel playerTwoModel;

        private PlayerModel currentPlayerModel;
        private PlayerView currentPlayerView;

        private AIModel aiModel;

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
            playerTwoView.addFadeCompleteListener((Action)checkForAI);
            playerOneView.addFadeCompleteListener((Action)checkForAI);
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
                        int firstPickedCard = (int)this.gameModel.FirstCardIndex;
                        int secondPickedCard = (int)this.gameModel.SecondCardIndex;
                        PlayerModel playerModel = currentPlayerModel;
                        PlayerView playerView = currentPlayerView;
                        
                        card.addFlipListener((Action)(() =>
                        {
                            // Delay here or not??
                            Task.Delay(FLIPDELAY).ContinueWith(_ =>
                            {
                                try
                                {
                                    this.Dispatcher.Invoke((Action)(() =>
                                    {
                                        DummyCard firstDummyCard = CreateDummyCardInGrid(firstPickedCard);
                                        DummyCard secondDummyCard = CreateDummyCardInGrid(secondPickedCard);
                                        ++dummyPairsInPlay;

                                        if (firstDummyCard.distanceTo(currentPlayerView.myStack) > secondDummyCard.distanceTo(currentPlayerView.myStack))
                                        {
                                            firstDummyCard.addCompletedMoveListener((Action)(() =>
                                            {
                                                removeCards(playerModel, playerView, firstPickedCard, secondPickedCard);
                                                checkForAI();
                                            }));

                                            secondDummyCard.addCompletedMoveListener((Action)(() => { playerView.myStack.Fill = firstDummyCard.myImage.Fill; }));
                                        }
                                        else
                                        {
                                            secondDummyCard.addCompletedMoveListener((Action)(() =>
                                            {
                                                removeCards(playerModel, playerView, firstPickedCard, secondPickedCard);
                                                checkForAI();
                                    }));

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
                        this.gameModel.ClearPicked();
                    }
                    else
                    {
                        CardView firstCard = this.CardGrid.Children[(int)this.gameModel.FirstCardIndex] as CardView;
                        this.CardGrid.IsEnabled = false;

                        card.addFlipListener((Action)(() =>
                        {
                            currentPlayerModel = currentPlayerModel.Equals(playerOneModel) ? playerTwoModel : playerOneModel;
                            currentPlayerView.Active = false;
                            currentPlayerView = currentPlayerView.Equals(playerOneView) ? playerTwoView : playerOneView;
                            currentPlayerView.Active = true;
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

        private void checkForAI()
        {
            if (SettingsModel.AgainstAI
                && currentPlayerModel.Equals(playerTwoModel)
                && !this.gameModel.IsGameOver())
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

        /// <summary>
        /// Removes the specified cards from play 
        /// </summary>
        /// <param name="playerModel"></param>
        /// <param name="playerView"></param>
        /// <param name="firstCardIndex"></param>
        /// <param name="secondCardIndex"></param>
        private void removeCards(PlayerModel playerModel, PlayerView playerView, int firstCardIndex, int secondCardIndex)
        {
            this.CardGrid.Children[firstCardIndex].IsEnabled = false;
            this.CardGrid.Children[secondCardIndex].IsEnabled = false;
            --dummyPairsInPlay;
            playerModel.AddCollectedPair(firstCardIndex);
            playerView.setPoints(playerModel.Score);

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
            ((MainWindow)Application.Current.MainWindow).ChangeView(MainWindow.View.Settings);
        }

        private void openSettingsWindow(object sender, RoutedEventArgs e)
        {
            stopEditingNames();
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
            exitEditMouseCaptureRectangle.Visibility = Visibility.Visible;
            CardGrid.IsEnabled = false;
            playerOneView.Active = true;
            playerTwoView.Active = true;
        }

        private void exitEditMouseCaptureRectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            stopEditingNames();
        }

        private void stopEditingNames()
        {
            if (playerOneView.name.IsKeyboardFocused || playerTwoView.name.IsKeyboardFocused)
            {
                exitEditMouseCaptureRectangle.Visibility = Visibility.Collapsed;
                Keyboard.ClearFocus();
                playerOneView.Active = true;
                playerTwoView.Active = false;
            }
        }
    }
}
