using System.Linq;
using System.Xml.Linq;

namespace Hashi_Puzzle_Solver
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string path = "..\\..\\..\\data.json";

            JsonReader.JsonData a = JsonReader.GetData(path);
            Console.WriteLine("Row Num: " + a.rowNum);
            Console.WriteLine("Col Num: " + a.colNum + "\n");

            Graph graphSheet = new Graph(a.rowNum, a.colNum, a.nodes);

            graphSheet.DrawGraph();
            graphSheet.Solve();
        }
    }
}