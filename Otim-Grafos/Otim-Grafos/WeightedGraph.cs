using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otim_Grafos
{
    public class WeightedGraph
    {
        public Dictionary<int, Dictionary<int, int>> Edges2         { get; set; }
        public Dictionary<string, int>               VertexLabelMap { get; set; }
        public List<Vertex>                          Vertices       { get; set; }
        public List<Edge>                            Edges          { get; set; }


        public WeightedGraph(string path)
        {
            LoadGraph (path);
        }

        private void LoadGraph (string path)
        {
            List<string> lines = File.ReadAllLines (path).ToList ();
            Vertices           = new List<Vertex> ();
            Edges              = new List<Edge> ();

            foreach (string line in lines)
            {
                string[] splittedLine = line.Split ();


                Vertex v1   = new Vertex (Vertices.Count, (splittedLine[0]));
                AddVertex (v1);
                Vertex v2   = new Vertex (Vertices.Count, (splittedLine[1]));
                AddVertex (v2);
                Edge   edge = new Edge (v1, v2, int.Parse(splittedLine[2]));
                AddEdge (edge);
            }
        }

        private void AddVertex (Vertex vertex)
        {
            // If is new vertex, add it to vertex list
            if (Vertices.Where (v => v.Index == vertex.Index).Count () == 0)
            {
                Vertices.Add (vertex);
            }
        }

        private void AddEdge(Edge edge)
        {
            
        }
    }
}
