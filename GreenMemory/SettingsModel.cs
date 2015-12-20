using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenMemory
{
    class SettingsModel
    {
        static public int Rows { get; set; }
        static public int Columns { get; set; }
        static public bool AgainstAI { get; set; }
        static public double AILevel { get; set; } // Lower values = harder to beat. 
    }
}
