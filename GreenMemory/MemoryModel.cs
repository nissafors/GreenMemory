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
        private int numberOfPlayers;
        private int[] deck;
        private List<int>[] playerPairValues;

        // <summary>
        // Construct a MemoryModel. Initialize number of cards to 16 and number of players to 2.</summary>
        public MemoryModel() : this(16, 2) { }

        // <summary>
        // Construct a MemoryModel. Initialize number of players to 2.</summary>
        // <param name="numberOfCards">How many cards to play with. Must be an even number.</param>
        public MemoryModel(int numberOfCards) : this(numberOfCards, 2) { }

        // <summary>
        // Construct a MemoryModel. Initialize number of cards to 16.</summary>
        // <param name="numberOfPlayers">The number of players participating.</param>
        public MemoryModel(int numberOfPlayers) : this(16, numberOfPlayers) { }

        // <summary>
        // Construct a MemoryModel.</summary>
        // <param name="numberOfCards">How many cards to play with. Must be an even number.</param>
        // <param name="numberOfPlayers">The number of players participating.</param>
        public MemoryModel(int numberOfCards, int numberOfPlayers)
        {
            this.numberOfCards = numberOfCards;
            this.numberOfPlayers = numberOfPlayers;
            playerPairValues = new List<int>[numberOfPlayers];
            deck = new int[numberOfCards];
            shuffleDeck();
        }

        // <summary>
        // Get or set the number of cards.</summary>
        public int NumberOfCards { get; set; }

        // <summary>
        // Get or set the number of players.</summary>
        public int NumberOfPlayers { get; set; }

        // <summary>
        // Get the card values of the shuffled deck.</summary>
        public int[] GetDeck()
        {
            return (int[])deck.Clone();
        }

        // <summary>
        // Get the values of the pairs collected by a player.</summary>
        // <param name="player">The number of the player.</param>
        // <returns>Values of all pairs collected.</returns>
        public ReadOnlyCollection<int> GetScore(int player)
        {
            return playerPairValues[player].AsReadOnly();
        }

        // <summary>
        // Pick two cards from the table.</summary>
        // <param name="index1">The index of the first card picked.</param>
        // <param name="index2">The index of the second card picked.</param>
        // <param name="player">The number of the player who picked the cards.</param>
        // <returns>Returns the value of the two cards if they match and otherwise null.</returns>
        public int? PickTwoCards(int index1, int index2, int player)
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
            deck = deck.OrderBy(x => rand.Next()).ToArray();
        }
    }
}
