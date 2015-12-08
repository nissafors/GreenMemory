using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenMemory
{
    // <summary>
    // Models a player in a game of memory.</summary>
    class PlayerModel
    {
        // <summary>
        // Get or set the name of the player.</summary>
        public string Name { get; set; }

        private List<int> pairsCollected;

        // <summary>
        // Construct a PlayerModel. Initialize Name to "Player".</summary>
        public PlayerModel() : this("Player") { }

        // <summary>
        // Construct a PlayerModel.</summary>
        // <param name="name">Set the name of the player.</param>
        public PlayerModel(string name)
        {
            this.Name = name;
            pairsCollected = new List<int>();
        }

        // <summary>
        // Add a card to collected pairs.</summary>
        // <param name="cardValue">The value of the cards in the pair.</param>
        public void AddCollectedPair(int cardValue)
        {
            pairsCollected.Add(cardValue);
        }

        // <summary>
        // Get the card values of all pairs collected by this player.</summary>
        public ReadOnlyCollection<int> GetCollectedPairs
        {
            get { return pairsCollected.AsReadOnly(); }
        }

        // <summary>
        // Get the number of pairs collected.</summary>
        public int Score {
            get { return pairsCollected.Count; }
        }
    }
}
