using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hashi_Puzzle_Solver
{
    internal class NumberNode : Node
    {
        /// <summary>
        /// The number of bridges needed
        /// </summary>
        public int Number { get; private set; }

        /// <summary>
        /// If the node has the amount of bridges needed
        /// </summary>
        public bool Finished { get; private set; }

        /// <summary>
        /// The type of bridge going up from this node
        /// </summary>
        NumberBridge Up { get; set; }

        /// <summary>
        /// The type of bridge going right from this node
        /// </summary>
        NumberBridge Right { get; set; }

        /// <summary>
        /// The type of bridge going down from this node
        /// </summary>
        NumberBridge Down { get; set; }

        /// <summary>
        /// The type of bridge going left from this node
        /// </summary>
        NumberBridge Left { get; set; }

        public NumberNode(int row, int col, int number)
        {
            Row = row;
            Col = col;
            Number = number;
            Finished = false;
            Up = Right = Down = Left = NumberBridge.None;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>a string that tells the coordiante, number, and bridges currently connected to the cell</returns>
        public override string ToString()
        {
            return $"Node {Row} {Col} has a number of {Number}. Bridges (in NESW order) are: {Up} {Right} {Down} {Left}";
        }

        public void SetUp(NumberBridge bridge)
        {
            Up = bridge;
            int num = CurrentBridgeNum();
            Finished = num == Number;
        }

        public int CurrentBridgeNum()
        {
            return (int)Up + (int)Left + (int)Down + (int)Right;
        }
    }
}
