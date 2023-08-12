using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hashi_Puzzle_Solver
{
    internal abstract class Node
    {
        public int Row { get; protected set; }
        public int Col { get; protected set; }
    }
}
