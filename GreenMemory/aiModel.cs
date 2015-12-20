using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GreenMemory
{
    // <summary>
    // Logic for an AI player in a memory game.</summary>
    class AIModel
    {
        private Action<object, MouseButtonEventArgs> cardClickEventHandler;
        private Action<object, MouseEventArgs> mouseEnterCardEventHandler;
        private Action<object, MouseEventArgs> mouseLeaveCardEventHandler;
        private MemoryModel game;
        private Grid cardGrid;
        private double DELTA = SettingsModel.AILevel;

        // <summary>
        // Construct a new AIModel.</summary>
        // <param name="game">The MemoryModel used in this game.</param>
        // <param name="cardGrid">The Grid holding the cards to pick.</param>
        // <param name="cardIndexQueue">A queue of indexes to cards flipped by any player, produced by the calling <cref="GameView" /> instance.</param>
        // <param name="cardClickEventHandler">Action delegate to the event handler for mouse click on a card.</param>
        // <param name="mouseEnterCardEventHandler">Action delegate to the event handler for when the mouse enters a card.</param>
        // <param name="mouseLeaveCardEventHandler">Action delegate to the event handler for when the mouse leaves a card.</param>
        public AIModel(MemoryModel game,
            Grid cardGrid,
            Action<object, MouseButtonEventArgs> cardClickEventHandler,
            Action<object, MouseEventArgs> mouseEnterCardEventHandler,
            Action<object, MouseEventArgs> mouseLeaveCardEventHandler)
        {
            this.game = game;
            this.cardGrid = cardGrid;
            this.cardClickEventHandler = cardClickEventHandler;
            this.mouseEnterCardEventHandler = mouseEnterCardEventHandler;
            this.mouseLeaveCardEventHandler = mouseLeaveCardEventHandler;
        }

        // <summary>
        // Queue up a new job for AI, thus making it flip two cards.</summary>
        public void WakeUp()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(runAI));
        }

        // <summary>
        // Get moves and call UI thread to perform flips.</summary>
        private void runAI(Object state)
        {
            int firstCard, secondCard;
            getCardsToFlip(out firstCard, out secondCard);
            Thread.Sleep(1000);
            try
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    cardClickEventHandler(cardGrid.Children[firstCard], null);
                }));
            }
            catch (NullReferenceException)
            {
                return;
            }
            Thread.Sleep(500);
            try
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                        cardClickEventHandler(cardGrid.Children[secondCard], null);
                }));
                }
            catch (NullReferenceException)
            {
                return;
            }
        }

        // <summary>
        // Calculate which cards to flip.</summary>
        private void getCardsToFlip(out int firstCard, out int secondCard)
        {
            // Determine the probability that AI will pick a pair based on history
            System.Diagnostics.Debug.Write("\n1: ");
            firstCard = getFirstCardIndex();
            System.Diagnostics.Debug.Write("\n2: ");
            secondCard = getSecondCardIndex(firstCard);
        }

        // <summary>
        // Choose the first card to pick based on history.</summary>
        private int getFirstCardIndex()
        {
            Dictionary<int, double> probabilityDict = new Dictionary<int, double>();
            for (int i = 0; i < game.History.Count; i++)
            {
                for (int j = i + 1; j < game.History.Count; j++)
                {
                    if (probabilityDict.Keys.Contains(game.History.ElementAt(i)) ||
                        probabilityDict.Keys.Contains(game.History.ElementAt(j)))
                    {
                        continue;
                    }

                    if (game.History.ElementAt(i) != game.History.ElementAt(j)
                        && game.GetDeck()[game.History.ElementAt(i)] == game.GetDeck()[game.History.ElementAt(j)])
                    {
                        if (game.GetDeck()[game.History.ElementAt(i)] != -1)
                        {
                            double p = 1D - (double)(i + j) * DELTA;
                            p = p > 0 ? p : 0;
                            probabilityDict.Add(game.History.ElementAt(i), p);
                            break;
                        }
                    }
                } // for j
            } // for i

            return chooseCard(probabilityDict);
        }

        // <summary>
        // Get second card to pick based on first card and history.</summary>
        private int getSecondCardIndex(int firstCardIndex)
        {
            int firstCardValue = game.GetDeck()[firstCardIndex];
            Dictionary<int, double> probabilityDict = new Dictionary<int, double>();

            int i = 0;
            foreach (int index in game.History)
            {
                if (probabilityDict.Keys.Contains(index))
                    continue;

                if (game.GetDeck()[index] == firstCardValue && index != firstCardIndex)
                {
                    int key = index;
                    double value = 1D - (double)i * DELTA;
                    value = value > 0 ? value : 0;
                    probabilityDict.Add(key, value);
                }

                i++;
            }

            return chooseCard(probabilityDict, firstCardIndex);
        }

        // <summary>
        // Choose a card based on probabilities.</summary>
        private int chooseCard(Dictionary<int, double> indexProbabilityPairs, int pickedCardIndex = -1)
        {
            // Sort by probability
            List<KeyValuePair<int, double>> probabilityList;
            probabilityList = indexProbabilityPairs.OrderByDescending(x => x.Value).ToList();

            // Loop through cards and try to choose one.
            Random rand = new Random();
            foreach (KeyValuePair<int, double> indexProbabilityPair in probabilityList)
            {
                int index = indexProbabilityPair.Key;
                double probability = indexProbabilityPair.Value;
                double randNr = rand.NextDouble();
                System.Diagnostics.Debug.Write("P: " + probability + ", 1 - P: " + (1 - probability));
                System.Diagnostics.Debug.Write(", RND" + randNr);
                if (randNr > 1 - probability)
                    System.Diagnostics.Debug.Write(" = Success.");
                else
                    System.Diagnostics.Debug.Write(" = Failed.");
                if (randNr > 1 - probability)
                    return index;
            }

            // Failed to remember any pair: Fall back to randomly pick a card.
            int ix;
            do
            {
                ix = rand.Next(game.NumberOfCards);
            } while (game.CardIsTaken(ix) || ix == pickedCardIndex);

            return ix;
        }
    }
}
