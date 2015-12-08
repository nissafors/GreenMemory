﻿using System;
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
using System.Windows.Threading;
using System.Timers;

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
            newGame();
            ((settings.Content as StackPanel).Children[0] as Button).Click += clickNewGame;
        }

        /// <summary>
        /// Sets up the board for a new game
        /// </summary>
        private void newGame()
        {
            gameModel = new MemoryModel(rows * columns);

            Brush[] br = new Brush[8]{Brushes.LightBlue, Brushes.Blue, Brushes.Yellow, 
                                        Brushes.Green, Brushes.Red, Brushes.Orange, Brushes.Aqua, Brushes.Maroon};

            Grid cardGrid = this.CardGrid as Grid;

            // Clear gameboard
            cardGrid.Children.Clear();
            cardGrid.RowDefinitions.Clear();
            cardGrid.ColumnDefinitions.Clear();
            pickedCard = -1;

            // Set number of rows
            for (int i = 0; i < rows; ++i)
            {
                cardGrid.RowDefinitions.Add(new RowDefinition());
            }

            // Set number of columns
            for (int i = 0; i < columns; ++i)
            {
                cardGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            CardView.BackgroundImage = Brushes.Black;
            int[] deck = gameModel.GetDeck();

            for (int ix = 0; ix < deck.Length; ++ix)
            {
                CardView card = new CardView(deck[ix], br[deck[ix]]);
                card.Margin = new Thickness(5);
                Grid.SetColumn(card, (ix % columns));
                Grid.SetRow(card, (ix / rows));
                card.MouseUp += clickCard;
                cardGrid.Children.Add(card);
            }
        }
        
        void clickNewGame(object sender, RoutedEventArgs e)
        {
            newGame();
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
                            // TODO: Increase score for player.
                        }
                    }
                    else
                    {
                        CardView secondCard = this.CardGrid.Children[pickedCard] as CardView;
                        CardGrid.IsEnabled = false;

                        Task.Delay(400).ContinueWith(_ =>
                        {
                            this.Dispatcher.Invoke((Action)(() =>
                            {
                                card.FlipCard();
                                secondCard.FlipCard();
                                CardGrid.IsEnabled = true;
                            }));
                        });
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
