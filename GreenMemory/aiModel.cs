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
    class AIModel
    {
        private Action<object, MouseButtonEventArgs> cardClickEventHandler;
        private MemoryModel game;
        private Grid cardGrid;

        public AIModel(MemoryModel game, Grid cardGrid, Action<object, MouseButtonEventArgs> cardClickEventHandler)
        {
            this.game = game;
            this.cardGrid = cardGrid;
            this.cardClickEventHandler = cardClickEventHandler;
        }

        public void WakeUp()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(runAI));
        }

        private void runAI(Object state)
        {
            Random rand = new Random();
            int firstCard, secondCard;
            do {
                firstCard = rand.Next(game.NumberOfCards);
            } while (game.CardIsTaken(firstCard));
            do {
                secondCard = rand.Next(game.NumberOfCards);
            } while (game.CardIsTaken(secondCard) || secondCard == firstCard);
            Thread.Sleep(1000);
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                try
                {
                    cardClickEventHandler(cardGrid.Children[firstCard], null);
                }
                catch (NullReferenceException)
                {
                    return;
                }
            }));
            Thread.Sleep(500);
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                try
                {
                    cardClickEventHandler(cardGrid.Children[secondCard], null);
                }
                catch (NullReferenceException)
                {
                    return;
                }
            }));
        }
    }
}
