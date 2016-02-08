using System;
using System.Collections.Generic;
using System.Linq;
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
        public enum Difficulty { Easy, Medium, Hard }

        private const double AI_LEVEL_EASY = 0.5;
        private const double AI_LEVEL_MEDIUM = 0.1;
        private const double AI_LEVEL_HARD = 0.05;

        private Action<object, MouseButtonEventArgs> cardClickEventHandler;
        private Action<object, MouseEventArgs> mouseEnterCardEventHandler;
        private Action<object, MouseEventArgs> mouseLeaveCardEventHandler;
        private MemoryModel game;
        private Grid cardGrid;
        private volatile Difficulty level;
        private double delta;
        private bool killThreads;
        private int waitHover;
        private int waitAfterClick;
        private volatile bool pauseBool;

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
            Action<object, MouseEventArgs> mouseLeaveCardEventHandler,
            int waitHover,
            int waitAfterClick)
        {
            this.game = game;
            this.cardGrid = cardGrid;
            this.cardClickEventHandler = cardClickEventHandler;
            this.mouseEnterCardEventHandler = mouseEnterCardEventHandler;
            this.mouseLeaveCardEventHandler = mouseLeaveCardEventHandler;
            this.waitHover = waitHover;
            this.waitAfterClick = waitAfterClick;

            // Make sure Level and delta are in sync
            Level = Difficulty.Medium;
            delta = AI_LEVEL_MEDIUM;

            // Read difficulty from settings
            Level = SettingsModel.AILevel;

            // Add handler for level change events
            SettingsModel.AddChangeSettingsListener(levelHandler);
        }

        /// <summary>
        /// Gets or sets this AIModels pause state.
        /// </summary>
        public bool Pause
        {
            get { return pauseBool; }
            set { pauseBool = value; }
        }

        // <summary>
        // Level property.</summary>
        // <value>
        // Get or set AI level using the AIModel.Difficulty enum.</value>
        public Difficulty Level
        {
            get { return level; }
            set
            {
                if (value != level)
                {
                    level = value;
                    switch (level)
                    {
                        case Difficulty.Easy:
                            this.delta = AI_LEVEL_EASY;
                            break;
                        case Difficulty.Hard:
                            this.delta = AI_LEVEL_HARD;
                            break;
                        default:
                            this.delta = AI_LEVEL_MEDIUM;
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Handle level change
        /// </summary>
        /// <param name="type"></param>
        private void levelHandler(SettingsModel.SettingsType type)
        {
            if (type == SettingsModel.SettingsType.AIDifficulty)
                Level = SettingsModel.AILevel;
        }
        
        // <summary>
        // Let all AI related threads run to termination as soon as possible.</summary>
        public void KillThreads()
        {
            killThreads = true;
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

            Thread.Sleep(waitHover);
            if (Pause) pause();
            performMouseActionOnGrid(null, mouseEnterCardEventHandler, firstCard);
            Thread.Sleep(waitHover);
            if (Pause) pause();
            performMouseActionOnGrid(cardClickEventHandler, null, firstCard);
            Thread.Sleep(waitAfterClick);
            if (Pause) pause();
            performMouseActionOnGrid(null, mouseLeaveCardEventHandler, firstCard);
            
            Thread.Sleep(waitHover);
            if (Pause) pause();
            performMouseActionOnGrid(null, mouseEnterCardEventHandler, secondCard);
            Thread.Sleep(waitHover);
            if (Pause) pause();
            performMouseActionOnGrid(cardClickEventHandler, null, secondCard);
            Thread.Sleep(waitAfterClick);
            if (Pause) pause();
            performMouseActionOnGrid(null, mouseLeaveCardEventHandler, secondCard);
        }

        /// <summary>
        /// Loop until Pause property is false or threads are being killed.</summary>
        private void pause()
        {
            while (Pause && !killThreads) ;
        }

        // <summary>
        // Helper method for runAI to do the actual UI thread calls.</summary>
        // <remarks>Exactly one of clickHandler and HoverHandler must be null.</remarks>
        private void performMouseActionOnGrid(Action<object, MouseButtonEventArgs> clickHandler,
            Action<object, MouseEventArgs> hoverHandler,
            int gridIndex)
        {
            if (killThreads)
                return;

            try
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    if (clickHandler != null && hoverHandler == null)
                        clickHandler(cardGrid.Children[gridIndex], null);
                    else if (clickHandler == null && hoverHandler != null)
                        hoverHandler(cardGrid.Children[gridIndex], null);
                    else
                        throw new ArgumentException("performMouseActionOnGrid(): Too many event handlers.");
                }));
            }
            catch (NullReferenceException)
            {
                // This will happen if program was shut down while AI was working.
                return;
            }
        }

        // <summary>
        // Enable or disable cardGrid.</summary>
        private void cardGridEnabled(bool enable)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    cardGrid.IsEnabled = enable;
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
            firstCard = getFirstCardIndex();
            secondCard = getSecondCardIndex(firstCard);
        }

        // <summary>
        // Choose the first card to pick based on history.</summary>
        private int getFirstCardIndex()
        {
            System.Diagnostics.Debug.WriteLine("Delta: " + delta);

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
                            double p = 1D - (double)(i + j) * delta;
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
                    double value = 1D - (double)i * delta;
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
