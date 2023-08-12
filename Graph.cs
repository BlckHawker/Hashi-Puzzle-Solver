using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
