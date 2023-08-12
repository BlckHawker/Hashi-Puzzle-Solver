using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Hashi_Puzzle_Solver
{
    internal abstract class Node
    {
        public int Row { get; protected set; }
        public int Col { get; protected set; }
    }

    internal class BridgeNode : Node
    {
        public Bridge Bridge { get; set; }
        public BridgeNode(int row, int col)
        {
            Row = row;
            Col = col;
        }
    }

    [Serializable]
    internal class NumberNode : Node
    {
        /// <summary>
        /// The number of bridges needed
        /// </summary>
        public int Number { get; private set; }

        /// <summary>
        /// If the node has the amount of bridges needed
        /// </summary>
        public bool Finished { get { return CurrentBridgeNum == Number; } }

        public int CurrentBridgeNum { get { return (int)Up + (int)Left + (int)Down + (int)Right; } }

        /// <summary>
        /// The type of bridge going up from this node
        /// </summary>
        public NumberBridge Up { get; set; }

        /// <summary>
        /// The type of bridge going right from this node
        /// </summary>
        public NumberBridge Right { get; set; }

        /// <summary>
        /// The type of bridge going down from this node
        /// </summary>
        public NumberBridge Down { get; set; }

        public int PlaceHolderBridgeNum { get { return (int)PlaceHolderUp + (int)PlaceHolderRight + (int)PlaceHolderDown + (int)PlaceHolderLeft; } }


        /// <summary>
        /// The type of bridge going left from this node
        /// </summary>
        public NumberBridge Left { get; set; }

        public NumberBridge PlaceHolderUp { get; set; }

        public NumberBridge PlaceHolderRight { get; set; }

        public NumberBridge PlaceHolderDown { get; set; }

        public NumberBridge PlaceHolderLeft { get; set; }

        /// <summary>
        /// This concstuctor is used to create a node from scratch
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="number"></param>
        [JsonConstructor]
        public NumberNode(int row, int col, int number)
        {
            Row = row;
            Col = col;
            Number = number;
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
    }
}
