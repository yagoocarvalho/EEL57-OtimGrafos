using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otim_Grafos
{
    public class Edge
    {
        public List<Vertex> Vertices { get; set; }
        public int          Weight   { get; set; }

        public Edge (Vertex v1, Vertex v2, int weight)
        {
            Vertices = new List<Vertex> ();
            Vertices.Add (v1);
            Vertices.Add (v2);
            Weight = weight;
        }
    }
}
