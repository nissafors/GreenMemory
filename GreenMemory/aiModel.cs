using System;
using System.Collections.Generic;
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

        // <summary>
        // Construct a new AIModel.</summary>
        // <param name="game">The MemoryModel used in this game.</param>
        // <param name="cardGrid">The Grid holding the cards to pick.</param>
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
            Random rand = new Random();
            do
            {
                firstCard = rand.Next(game.NumberOfCards);
            } while (game.CardIsTaken(firstCard));
            do
            {
                secondCard = rand.Next(game.NumberOfCards);
            } while (game.CardIsTaken(secondCard) || secondCard == firstCard);
        }
    }
}
