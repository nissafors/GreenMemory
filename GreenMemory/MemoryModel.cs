﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenMemory
{
    class MemoryModel
    {
        public static bool CardsMatch(CardView card1, CardView card2)
        {
            return card1.Id == card2.Id;
        }
    }
}
