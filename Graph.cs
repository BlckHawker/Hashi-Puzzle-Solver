using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Hashi_Puzzle_Solver
{
    internal class Graph
    {
        private Node[,] graph;
        private int Row;
        private int Col;
        private List<NumberNode> NumberNodes;

        public Graph(int row, int col, List<NumberNode> numberNodes) 
        {
            Row = row;
            Col = col;
            NumberNodes = numberNodes;

            graph = new Node[row, col];

            for(int i = 0; i < row; i++) 
            {
                for (int j = 0; j < col; j++)
                {
                    NumberNode possibleNode = null;
                    try
                    {
                        possibleNode = NumberNodes.First(x => x.Row == i && x.Col == j);
                    }
                    catch 
                    {}
                    
                    if (possibleNode != null)
                    {
                        graph[i, j] = possibleNode;
                    }

                    else
                    {
                        graph[i, j] = new BridgeNode(i, j);
                    }
                }
            }
        }

        public void Solve()
        {
            //for each number node that is not finished,
            //figure out all the possible configurations of briges


            //if there is only one configuration set those bridges
            //update other nodes
        }

        private List<BridgeConfiguration> GetPossibleBridges(NumberNode node)
        {
            List<BridgeConfiguration> list = new List<BridgeConfiguration>();

            for (int i = 0; i < 81; i++)
            {
                string bridgeNums = ConvertFromBase10ToBase3(i);
                list.Add(new BridgeConfiguration(int.Parse("" + bridgeNums[0]), int.Parse("" + bridgeNums[1]), int.Parse("" + bridgeNums[2]), int.Parse("" + bridgeNums[3])));
            }

            //elimate any configurations that numbers dont match up to the node
            for (int i = list.Count - 1; i > -1; i--)
            {
                if (list[i].BridgeNum != node.CurrentBridgeNum)
                {
                    list.RemoveAt(i);
                }
            }

            //eliminate any configurations that have walls that lead to out of bounds
            //eliminate any configurations that have walls that lead to another bridge
            for (int i = list.Count - 1; i > -1; i--)
            {
                BridgeConfiguration bc = list[i];

                if (bc.Up != NumberBridge.None)
                {
                    int row = node.Row;

                    //if we are on the edge, remove it
                    if (row == 0)
                    {
                        list.RemoveAt(i);
                        continue;
                    }

                    bool foundOtherNode = false;
                    bool foundOtherBridge = false;

                    while (row != 0 && !foundOtherNode && !foundOtherBridge)
                    {
                        row--;
                        Node currentNode = graph[row, node.Col];
                        if (currentNode is NumberNode)
                        {
                            foundOtherNode = true;
                        }

                        else if(((BridgeNode)currentNode).Bridge != Bridge.None)
                        {
                            foundOtherBridge = true;
                        }
                    }

                    //if we didnt find a node, the we are in the void
                    //if we found another bridge, then we are going to connect, which is invalid
                    if (!foundOtherNode || foundOtherBridge)
                    {
                        list.RemoveAt(i);
                        continue;
                    }
                }

                if (bc.Right != NumberBridge.None)
                {
                    int col = node.Col;

                    //if we are on the edge, remove it
                    if (col == Col - 1)
                    {
                        list.RemoveAt(i);
                        continue;
                    }

                    bool foundOtherNode = false;
                    bool foundOtherBridge = false;

                    while (col != Col - 1 && !foundOtherNode)
                    {
                        col++;
                        Node currentNode = graph[node.Row, col];
                        if (currentNode is NumberNode)
                        {
                            foundOtherNode = true;
                        }

                        else if (((BridgeNode)currentNode).Bridge != Bridge.None)
                        {
                            foundOtherBridge = true;
                        }
                    }

                    //if we didnt find a node, the we are in the void
                    //if we found another bridge, then we are going to connect, which is invalid
                    if (!foundOtherNode || foundOtherBridge)
                    {
                        list.RemoveAt(i);
                        continue;
                    }
                }

                if (bc.Down != NumberBridge.None)
                {
                    int row = node.Row;

                    //if we are on the edge, remove it
                    if (row == Row - 1)
                    {
                        list.RemoveAt(i);
                        continue;
                    }

                    bool foundOtherNode = false;
                    bool foundOtherBridge = false;

                    while (row != Row - 1 && !foundOtherNode)
                    {
                        row++;
                        Node currentNode = graph[row, node.Col];
                        if (currentNode is NumberNode)
                        {
                            foundOtherNode = true;
                        }

                        else if (((BridgeNode)currentNode).Bridge != Bridge.None)
                        {
                            foundOtherBridge = true;
                        }
                    }

                    //if we didnt find a node, the we are in the void
                    //if we found another bridge, then we are going to connect, which is invalid
                    if (!foundOtherNode || foundOtherBridge)
                    {
                        list.RemoveAt(i);
                        continue;
                    }
                }

                if (bc.Left != NumberBridge.None)
                {
                    int col = node.Col;

                    //if we are on the edge, remove it
                    if (col == 0)
                    {
                        list.RemoveAt(i);
                        continue;
                    }

                    bool foundOtherNode = false;
                    bool foundOtherBridge = false;

                    while (col != 0 && !foundOtherNode)
                    {
                        col--;
                        Node currentNode = graph[node.Row, col];
                        if (currentNode is NumberNode)
                        {
                            foundOtherNode = true;
                        }

                        else if (((BridgeNode)currentNode).Bridge != Bridge.None)
                        {
                            foundOtherBridge = true;
                        }
                    }

                    //if we didnt find a node, the we are in the void
                    //if we found another bridge, then we are going to connect, which is invalid
                    if (!foundOtherNode || foundOtherBridge)
                    {
                        list.RemoveAt(i);
                        continue;
                    }
                }
            }

            //eliminate any configurations that have walls that dont match the node's current walls
            for (int i = list.Count - 1; i > -1; i--)
            {
                BridgeConfiguration bc = list[i];

                if (node.Up != NumberBridge.None && node.Up != bc.Up ||
                    node.Right != NumberBridge.None && node.Right != bc.Right ||
                    node.Down != NumberBridge.None && node.Down != bc.Down ||
                    node.Left != NumberBridge.None && node.Left != bc.Left)
                {
                    list.RemoveAt(i);
                }
            }

            return list;
        }

        private class BridgeConfiguration
        {
            public NumberBridge Up { get; }
            public NumberBridge Right { get; }
            public NumberBridge Down { get; }
            public NumberBridge Left { get; }
            public int BridgeNum { get { return (int)Up + (int)Left + (int)Down + (int)Right; } }

            public BridgeConfiguration(int Up, int Right, int Down, int Left)
            {
                this.Up = (NumberBridge)Up;
                this.Right = (NumberBridge)Right;
                this.Down = (NumberBridge)Down;
                this.Left = (NumberBridge)Left;
            }

            public override string ToString()
            {
                return "" + (int)Up + (int)Right + (int)Down + (int)Left;
            }
        }

        public void DrawGraph()
        {
            for (int i = 0; i < Row; i++)
            {
                for (int j = 0; j < Col; j++)
                {
                    Node node = graph[i, j];

                    if (node is NumberNode)
                    {
                        Console.Write(((NumberNode)node).Number + " ");
                    }

                    else
                    {

                        switch (((BridgeNode)node).Bridge)
                        {
                            case Bridge.None:
                                Console.Write("." + " ");
                                break;

                            case Bridge.SingleVert:
                                Console.Write("|" + " ");
                                break;

                            case Bridge.DoubleVert:
                                Console.Write(((char)186) + " ");
                                break;

                            case Bridge.SingleHor:
                                Console.Write("-" + " ");
                                break;

                            case Bridge.DoubleHor:
                                Console.Write(((char)205) + " ");
                                break;
                        }
                    }
                }
                Console.WriteLine();
            }
        }

        private string ConvertFromBase10ToBase3(int num)
        {
            string answer = "";
            do
            {
                int newNum = num / 3;
                answer += "" + num % 3;
                num = newNum;

            } while (num > 0);

            answer = string.Join("", answer.Reverse());

            while (answer.Length != 4) { answer += "0"; }

            return answer; 
        }
    }
}
