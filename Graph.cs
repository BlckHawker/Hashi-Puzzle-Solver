﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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
            bool foundLogic; //tells if a logical move

            //for each number node that is not finished,
            //figure out all the possible configurations of briges
            do
            {
                Console.WriteLine("\n====Starting logic over====\n");
                foundLogic = false;
                List<NumberNode> unfinishedNodes = GetUnfinishedNodes();

                for (int i = 0; i < unfinishedNodes.Count; i++)
                {
                    NumberNode unfinishedNode = unfinishedNodes[i];

                    if (unfinishedNode.Row == 5 && unfinishedNode.Col == 4)
                    {
                        string a = "1";
                    }

                    Console.WriteLine(unfinishedNode);

                    //if the node happen to become finished via previous deductions, skip it
                    if (unfinishedNode.Finished)
                    {
                        continue;
                    }

                    List<BridgeConfiguration> bridgeConfigurations = GetPossibleBridges(unfinishedNode);

                    Console.WriteLine($"Bridge configurations are {string.Join(", ", bridgeConfigurations.Select(x => x.ToString()))}\n");

                    //if there is only one configuration set those bridges
                    if (bridgeConfigurations.Count == 1)
                    {
                        BridgeConfiguration bc = bridgeConfigurations[0];

                        
                        //update other nodes
                        if (bc.Up != NumberBridge.None)
                        {
                            SetUp(unfinishedNode, bc.Up);
                        }

                        //update other nodes
                        if (bc.Right != NumberBridge.None)
                        {
                            SetRight(unfinishedNode, bc.Right);
                        }

                        //update other nodes
                        if (bc.Down != NumberBridge.None)
                        {
                            SetDown(unfinishedNode, bc.Down);
                        }

                        //update other nodes
                        if (bc.Left != NumberBridge.None)
                        {
                            SetLeft(unfinishedNode, bc.Left);
                        }

                        foundLogic = true;

                        Console.WriteLine($"Setting this node's briges as {bc}");
                        DrawGraph();
                        break;
                    }

                }
            } while (foundLogic);
           
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <returns>true if there is at least 1 bridge in each cardinal direction</returns>
        private bool Corner4(NumberNode node)
        {
            return node.Number > 6;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>-1 if it can't be deduced that there is at least 1 single line in each direction. Otherwise returns the number direction that doesn't a line Up (0) Right (1) Down (2) Left(3)</returns>
        private int Corner3(NumberNode node)
        {
            if (node.Number > 6)
            {
                return -1;
            }

            if (VoidUp(node) == -1)
            {
                return 0;
            }

            if(VoidRight(node) == -1) 
            {
                return 1;
            }


            if (VoidDown(node) == -1)
            {
                return 2;
            }

            if (VoidDown(node) == -1)
            {
                return 3;
            }

            return -1;
        }

        private List<BridgeConfiguration> GetPossibleBridges(NumberNode node)
        {

            List<BridgeConfiguration> list = new List<BridgeConfiguration>();

            for (int i = 0; i < 81; i++)
            {
                string bridgeNums = ConvertFromBase10ToBase3(i);
                list.Add(new BridgeConfiguration(int.Parse("" + bridgeNums[0]), int.Parse("" + bridgeNums[1]), int.Parse("" + bridgeNums[2]), int.Parse("" + bridgeNums[3])));
            }

            list = RemoveDifferentNumbers(list, node);
            list = RemoveOutOfBoundsUp(list, node);
            list = RemoveOutOfBoundsRight(list, node);
            list = RemoveOutOfBoundsDown(list, node);
            list = RemoveOutOfBoundsLeft(list, node);
            list = RemoveConflictBridges(list, node);
            return list;
        }


        #region Eliminating Bridge Configuration
        /// <summary>
        /// Removes any Bridge configurations that don't add up to the desired number
        /// </summary>
        /// <param name="list"></param>
        /// <param name="node"></param>
        /// <returns>a list of Bridge Configuration left that satify this condition</returns>
        private List<BridgeConfiguration> RemoveDifferentNumbers(List<BridgeConfiguration> list, NumberNode node)
        {
            return list.Where(x => x.BridgeNum == node.Number).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>the row of the number node that was found go up. If none was found, returns -1</returns>
        private int VoidUp(NumberNode node)
        {
            int row = node.Row;

            //we need to find another node, in order to not go out of bounds
            while (row != 0)
            {
                row--;
                Node current = graph[row, node.Col];
                if (current is NumberNode)
                {
                    return row;
                }
                
                //if a bridge (that has not be drawn by this node) is in the way, then this is invalid
                else if (node.Up != NumberBridge.None && node.PlaceHolderUp != NumberBridge.None && ((BridgeNode)current).Bridge != Bridge.None)
                {
                    return -1;
                }
            }

            return -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>the row of the number node that was found go right. If none was found, returns -1</returns>
        private int VoidRight(NumberNode node)
        {
            int col = node.Col;
            while (col != Col - 1)
            {
                col++;
                Node current = graph[node.Row, col];
                if (current is NumberNode)
                {
                    return col;
                }

                //if a bridge (that has not be drawn by this node) is in the way, then this is invalid
                else if (node.Right != NumberBridge.None && node.PlaceHolderRight != NumberBridge.None && ((BridgeNode)current).Bridge != Bridge.None)
                {
                    return -1;
                }
            }
            return -1;
        }

        /// 
        /// </summary>
        /// <returns>the row of the number node that was found go down. If none was found, returns -1</returns>
        private int VoidDown(NumberNode node)
        {
            int row = node.Row;

            //we need to find another node, in order to not go out of bounds
            while (row != Row - 1)
            {
                row++;
                Node current = graph[row, node.Col];
                if (graph[row, node.Col] is NumberNode)
                {
                    return row;
                }

                //if a bridge (that has not be drawn by this node) is in the way, then this is invalid
                else if (node.Down != NumberBridge.None && node.PlaceHolderDown != NumberBridge.None && ((BridgeNode)current).Bridge != Bridge.None)
                {
                    return -1;
                }
            }
            return -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>the row of the number node that was found go left. If none was found, returns -1</returns>
        private int VoidLeft(NumberNode node)
        {
            int col = node.Col;
            while (col != 0)
            {
                col--;
                Node current = graph[node.Row, col];
                if (current is NumberNode)
                {
                    return col;
                }

                //if a bridge (that has not be drawn by this node) is in the way, then this is invalid
                else if (node.Left != NumberBridge.None && node.PlaceHolderLeft != NumberBridge.None && ((BridgeNode)current).Bridge != Bridge.None)
                {
                    return -1;
                }
            }
            return -1;
        }

        /// <summary>
        /// Removes any Bridge configurations that have bridges that go up out of bounds or run into other bridges
        /// </summary>
        /// <returns>a list of Bridge Configuration left that satify this condition</returns>
        private List<BridgeConfiguration> RemoveOutOfBoundsUp(List<BridgeConfiguration> list, NumberNode node)
        {
            List<BridgeConfiguration> currentList = new List<BridgeConfiguration>();

            foreach (BridgeConfiguration b in list)
            {
                if (b.Up == NumberBridge.None)
                {
                    currentList.Add(b);
                    continue;
                }

                if (VoidUp(node) != -1)
                {
                    currentList.Add(b);
                }
            }

            return currentList;
        }

        /// <summary>
        /// Removes any Bridge configurations that have bridges that go right out of bounds or run into other bridges
        /// </summary>
        /// <returns>a list of Bridge Configuration left that satify this condition</returns>
        private List<BridgeConfiguration> RemoveOutOfBoundsRight(List<BridgeConfiguration> list, NumberNode node)
        {
            List<BridgeConfiguration> currentList = new List<BridgeConfiguration>();

            foreach (BridgeConfiguration b in list)
            {
                if (b.Right == NumberBridge.None)
                {
                    currentList.Add(b);
                    continue;
                }

                if (VoidRight(node) != -1)
                {
                    currentList.Add(b);
                }
            }

            return currentList;
        }

        /// <summary>
        /// Removes any Bridge configurations that have bridges that go down out of bounds or run into other bridges
        /// </summary>
        /// <returns>a list of Bridge Configuration left that satify this condition</returns>
        private List<BridgeConfiguration> RemoveOutOfBoundsDown(List<BridgeConfiguration> list, NumberNode node)
        {
            List<BridgeConfiguration> currentList = new List<BridgeConfiguration>();

            if (node.Row == 0 && node.Col == 4)
            {
                int a = 1;
            }

            foreach (BridgeConfiguration b in list)
            {
                if (b.ToString() == "0211" || b.ToString() == "0112")
                {
                    int a = 1;
                }

                if (b.Down == NumberBridge.None)
                {
                    currentList.Add(b);
                    continue;
                }

                if (VoidDown(node) != -1)
                {
                    currentList.Add(b);
                }
            }

            return currentList;
        }

        /// <summary>
        /// Removes any Bridge configurations that have bridges that go left out of bounds or run into other bridges
        /// </summary>
        /// <returns>a list of Bridge Configuration left that satify this condition</returns>
        private List<BridgeConfiguration> RemoveOutOfBoundsLeft(List<BridgeConfiguration> list, NumberNode node)
        {
            List<BridgeConfiguration> currentList = new List<BridgeConfiguration>();

            foreach (BridgeConfiguration b in list)
            {
                if (b.Left == NumberBridge.None)
                {
                    currentList.Add(b);
                    continue;
                }

                if (VoidLeft(node) != -1)
                {
                    currentList.Add(b);
                }
            }

            return currentList;
        }


        /// <summary>
        /// Removes Bridge Configurations that conflict with already known bridges
        /// </summary>
        /// <param name="list"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        private List<BridgeConfiguration> RemoveConflictBridges(List<BridgeConfiguration> list, NumberNode node)
        {
            if (node.Up != NumberBridge.None)
            {
                list = list.Where(x => x.Up == node.Up).ToList();
            }

            if (node.Right != NumberBridge.None)
            {
                list = list.Where(x => x.Right == node.Right).ToList();
            }

            if (node.Down != NumberBridge.None)
            {
                list = list.Where(x => x.Down == node.Down).ToList();
            }

            if (node.Left != NumberBridge.None)
            {
                list = list.Where(x => x.Left == node.Left).ToList();
            }

            return list;
        }

        /// <summary>
        /// Get rid of bridge configurations that force 1s to connect to other 1s
        /// </summary>
        /// <param name="list"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        private List<BridgeConfiguration> RemoveOneConnectingToOne(List<BridgeConfiguration> list, NumberNode node)
        {
            return null; //todo new NotImplementedException();
        }
        #endregion




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

            Console.WriteLine("==============");

        }
        private string ConvertFromBase10ToBase3(int num)
        {
            if (num == 3)
            {
                int j = 2;
            }
            string answer = "";
            do
            {
                int newNum = num / 3;
                answer += "" + num % 3;
                num = newNum;

            } while (num > 0);

             answer = string.Join("", answer.Reverse()).PadLeft(4, '0');

            return answer; 
        }
        private List<NumberNode> GetUnfinishedNodes()
        {
            return NumberNodes.Where(x => !x.Finished).ToList();
        }

        #region SetBridges

        private void SetUp(NumberNode node, NumberBridge bridge)
        {
            if (bridge == NumberBridge.None)
            {
                return;
            }
            node.Up = bridge;

            int row = node.Row;

            bool foundNode;

            do
            {
                row--;
                Node currentNode = graph[row, node.Col];
                foundNode = currentNode is NumberNode;

                if (!foundNode)
                {
                    ((BridgeNode)graph[row, node.Col]).Bridge = bridge == NumberBridge.Double ? Bridge.DoubleVert : Bridge.SingleVert;
                }
            } while (!foundNode);

            ((NumberNode)graph[row, node.Col]).Down = bridge;
        }

        private void SetRight(NumberNode node, NumberBridge bridge)
        {
            if (bridge == NumberBridge.None)
            {
                return;
            }
            node.Right = bridge;

            int col = node.Col;

            bool foundNode;

            do
            {
                col--;
                Node currentNode = graph[node.Row, col];
                foundNode = currentNode is NumberNode;

                if (!foundNode)
                {
                    ((BridgeNode)graph[node.Row, col]).Bridge = bridge == NumberBridge.Double ? Bridge.DoubleHor : Bridge.SingleHor;
                }
            } while (!foundNode);

            ((NumberNode)graph[node.Row, col]).Left = bridge;
        }

        private void SetDown(NumberNode node, NumberBridge bridge)
        {
            if (bridge == NumberBridge.None)
            {
                return;
            }
            node.Down = bridge;

            int row = node.Row;

            bool foundNode;

            do
            {
                row++;
                Node currentNode = graph[row, node.Col];
                foundNode = currentNode is NumberNode;

                if (!foundNode)
                {
                    ((BridgeNode)graph[row, node.Col]).Bridge = bridge == NumberBridge.Double ? Bridge.DoubleVert : Bridge.SingleVert;
                }
            } while (!foundNode);

            ((NumberNode)graph[row, node.Col]).Up = bridge;
        }

        private void SetLeft(NumberNode node, NumberBridge bridge)
        {
            if (bridge == NumberBridge.None)
            {
                return;
            }
            node.Left = bridge;

            int col = node.Col;

            bool foundNode;

            do
            {
                col--;
                Node currentNode = graph[node.Row, col];
                foundNode = currentNode is NumberNode;

                if (!foundNode)
                {
                    ((BridgeNode)graph[node.Row, col]).Bridge = bridge == NumberBridge.Double ? Bridge.DoubleHor : Bridge.SingleHor;
                }
            } while (!foundNode);

            ((NumberNode)graph[node.Row, col]).Right = bridge;
        }
        #endregion
    }
}
