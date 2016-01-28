using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace GreenMemory
{
    // <summary>
    // Models a game of memory.</summary>
    class MemoryModel
    {
        private const int REMOVED = -1;

        private int numberOfCards;
        private int[] deck;
        private List<int> history;

        public bool TwoCardsPicked
        {
            get
            {
                return (FirstCardIndex != null && this.SecondCardIndex != null);
            }
        }
        public bool CorrectPair
        {
            get
            {
                if(this.TwoCardsPicked)
                {
                    return this.deck[(int)this.FirstCardIndex] == this.deck[(int)this.SecondCardIndex];
                }

                return false;
            }
        }
        public int? FirstCardIndex { get; private set; }
        public int? SecondCardIndex { get; private set; }
       

        // <summary>
        // Construct a MemoryModel. Initialize number of cards to 16.</summary>
        public MemoryModel() : this(16) { }

        // <summary>
        // Construct a MemoryModel.</summary>
        // <param name="numberOfCards">How many cards to play with. Must be an even number.</param>
        // <exception cref="ArgumentException">Thrown when numberOfCards is not an even number.</exception>
        public MemoryModel(int numberOfCards) {
            if (numberOfCards % 2 != 0)
            {
                throw new ArgumentException("numberOfCards", "Must be an even number.");
            }
            this.FirstCardIndex = null;
            this.SecondCardIndex = null;
            this.numberOfCards = numberOfCards;
            history = new List<int>();
            deck = new int[numberOfCards];
            shuffleDeck();
        }

        // <summary>
        // Get the number of cards in the deck.</summary>
        public int NumberOfCards {
            get { return numberOfCards; }
        }

        // <summary>
        // Get the card values of the shuffled deck.</summary>
        public int[] GetDeck()
        {
            return (int[])deck.Clone();
        }

        // <summary>
        // Get current game history.</summary>
        // <returns>Returns a collection of the indexes of all cards drawn with the most recent on top.</returns>
        public ReadOnlyCollection<int> History {
            get { return history.AsReadOnly(); }
        }

        /// <summary>
        /// Saves the picked card unless two cards are already picked.
        /// </summary>
        /// <param name="index"></param>
        /// <returns>True if succesfully saved card otherwise false</returns>
        public bool PickCard(int index)
        {
            // Error check
            if (deck[index] == REMOVED)
            {
                throw new ArgumentException("Argument was index to an already removed card.");
            }

            if (this.SecondCardIndex != null)
            {
                return false;
            }
            else if(this.FirstCardIndex != null)
            {
                this.SecondCardIndex = index;
            }
            else
            {
                this.FirstCardIndex = index;
            }

            return true;
        }

        /// <summary>
        /// Clears the picked cards and removes them from the deck if they are a pair
        /// </summary>
        public void ClearPicked()
        {
            history.Insert(0, (int)this.FirstCardIndex);
            history.Insert(0, (int)this.SecondCardIndex);

            if (this.CorrectPair)
            {
                deck[(int)this.FirstCardIndex] = deck[(int)this.SecondCardIndex] = REMOVED;
            }

            this.FirstCardIndex = this.SecondCardIndex = null;
        }

        // <summary>
        // Pick two cards from the table.</summary>
        // <param name="index1">The index of the first card picked.</param>
        // <param name="index2">The index of the second card picked.</param>
        // <param name="player">The number of the player who picked the cards.</param>
        // <returns>Returns the value of the two cards if they match and otherwise null.</returns>
        // <exception cref="ArgumentException">Thrown when index1 or index2 is an index of an already removed card</exception>
        public int? PickTwoCards(int index1, int index2)
        {
            // Error check
            if (deck[index1] == REMOVED || deck[index2] == REMOVED)
            {
                throw new ArgumentException("Argument was index to an already removed card.");
            }

            // Does cards match?
            if (deck[index1] == deck[index2])
            {
                // Player got this pair
                int cardVal = deck[index1];
                // Remove cards from table
                deck[index1] = deck[index2] = REMOVED;
                return cardVal;
            }

            return null;
        }

        public bool PeekTwoCards(int index1, int index2)
        {
            // Error check
            if (deck[index1] == REMOVED || deck[index2] == REMOVED)
            {
                throw new ArgumentException("Argument was index to an already removed card.");
            }
            // Does cards match?
            return deck[index1] == deck[index2];
        }

        // <summar>
        // Examine deck to find out if the card at given index is taken.</summary>
        // <param name="index">The index number of the card to examine.</param>
        // <returns>Returns true if the card no longer is in the deck.</returns>
        public bool CardIsTaken(int index)
        {
            return deck[index] == -1;
        }

        // <summary>
        // Return true if all cards are removed from the table.</summary>
        // <returns>Returns true if all pairs have been collected.</returns>
        public bool IsGameOver()
        {
            foreach (int value in deck)
            {
                if (value != REMOVED)
                {
                    return false;
                }
            }

            return true;
        }


        // Create deck with value pairs ranging from 0 to numberOfCards / 2 and shuffle the deck.
        private void shuffleDeck() {
            for (int i = 0; i < numberOfCards; i += 2)
            {
                deck[i] = deck[i + 1] = i / 2;
            }

            Random rand = new Random();
            //deck = deck.OrderBy(x => rand.Next()).ToArray();
        }
    }
}
