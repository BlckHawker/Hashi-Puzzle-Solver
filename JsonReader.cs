using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Hashi_Puzzle_Solver
{
    internal class JsonReader
    {
        [Serializable]
        public class JsonData
        {
            public int rowNum; //how many rows the grid will have
            public int colNum; //how many col the grid will have
            public List<NumberNode> nodes; //a list of all the nodes
        }

        public static JsonData GetData(string path)
        {
            return JsonConvert.DeserializeObject<JsonData>(new StreamReader(path).ReadToEnd());
        }
    }
}
