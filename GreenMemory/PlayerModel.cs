using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenMemory
{
    class PlayerModel
    {
        private string name;
        private int pairs;

        // Constructor
        public PlayerModel()
        {
            init("Player");
        }

        // constructor
        public PlayerModel(string name)
        {
            init(name);
        }

        // Initialize object, called by constructors
        private void init(string name)
        {
            this.name = name;
            this.pairs = 0;
        }

        // Name property
        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        // Pairs property
        public int Pairs
        {
            get { return this.pairs; }
            set { this.pairs = value; }
        }
    }
}
