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
        private Thread oThread;

        public AIModel(MemoryModel game, Grid cardGrid, Action<object, MouseButtonEventArgs> cardClickEventHandler)
        {
            this.game = game;
            this.cardGrid = cardGrid;
            this.cardClickEventHandler = cardClickEventHandler;
        }

        public void WakeUp()
        {
            if (oThread != null)
                System.Diagnostics.Debug.WriteLine(oThread.ThreadState.ToString());
            oThread = new Thread(new ThreadStart(runAI));
            oThread.Start();
            oThread.Name = "Started";
            System.Diagnostics.Debug.WriteLine(oThread.Name);
            //Task.Delay(1000).ContinueWith(_ =>
            //{
            //    Application.Current.Dispatcher.Invoke(new Action(() =>
            //    {
            //        oThread.Abort();
            //    }));
            //});
        }

        private void runAI()
        {
            Random rand = new Random();
            int firstCard, secondCard;
            do {
                firstCard = rand.Next(game.NumberOfCards);
            } while (game.CardIsTaken(firstCard));
            do {
                secondCard = rand.Next(game.NumberOfCards);
            } while (game.CardIsTaken(secondCard) || secondCard == firstCard);
            int[] deck = new int[16];
            deck = game.GetDeck();
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                cardClickEventHandler(cardGrid.Children[firstCard], null);
            }));
            Thread.Sleep(500);
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                cardClickEventHandler(cardGrid.Children[secondCard], null);
            }));

            //Application.Current.Dispatcher.Invoke(new Action(() =>
            //{
            //    System.Windows.MessageBox.Show("Terminating?");
            //}));
        }
    }
}
