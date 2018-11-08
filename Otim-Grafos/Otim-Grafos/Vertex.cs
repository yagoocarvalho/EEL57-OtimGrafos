using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otim_Grafos
{
    public class Vertex
    {
        public int          Index     { get; set; }
        public string       Label     { get; set; }
        public List<Vertex> Neighbors { get; set; }

        public Vertex (int index, string label)
        {
            Label     = label;
            Index     = index;
            Neighbors = new List<Vertex> ();
        }
    }
}
