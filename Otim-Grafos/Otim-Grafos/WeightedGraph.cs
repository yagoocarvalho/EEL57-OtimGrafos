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
        public Dictionary<int, HashSet<int>>        Edges           { get; set; } // <vertexIndex, listOfNeighbors>
        public Dictionary<int, int>                 Vertices        { get; set; } // <vertexIndex, vertexWeight>
        public Dictionary<string, int>              VertexLabelMap  { get; set; } // <vertexLabel, vertexIndex>
        public Dictionary<int, string>              VertexIndexMap  { get; set; } // <vertexIndex, vertexLabel>
        public int                                  GraphWeight     { get; set; }

        public WeightedGraph(string path)
        {
            VertexLabelMap  = new Dictionary<string, int> ();
            VertexIndexMap  = new Dictionary<int, string> ();
            Edges           = new Dictionary<int, HashSet<int>> ();
            Vertices        = new Dictionary<int, int> ();
            LoadGraph (path);
        }

        public WeightedGraph()
        {
            VertexLabelMap  = new Dictionary<string, int> ();
            VertexIndexMap  = new Dictionary<int, string> ();
            Edges           = new Dictionary<int, HashSet<int>> ();
            Vertices        = new Dictionary<int, int> ();
            GraphWeight     = 0;
        }

        private void LoadGraph (string path)
        {
            List<string> lines = File.ReadAllLines (path).ToList ();

            foreach (string line in lines)
            {
                // Load line
                string[] splittedLine = line.Split ();

                if (splittedLine[0] == "n")
                {
                    // Create node map
                    if (!VertexLabelMap.ContainsKey (splittedLine[1]))
                    {
                        VertexLabelMap.Add (splittedLine[1], VertexLabelMap.Count);
                        VertexIndexMap.Add (VertexIndexMap.Count, splittedLine[1]);
                    }

                    Vertices.Add (VertexLabelMap[splittedLine[1]], int.Parse (splittedLine[2]));
                }
                else if (splittedLine[0] == "e")
                {
                    AddEdge (splittedLine[1], splittedLine[2]);
                }
            }
        }

        public void AddEdge (string v1Label, string v2Label)
        {
            // Create node map
            if (!VertexLabelMap.ContainsKey (v1Label))
            {
                VertexLabelMap.Add (v1Label, VertexLabelMap.Count);
                VertexIndexMap.Add (VertexIndexMap.Count, v1Label);
            }

            // Create node map
            if (!VertexLabelMap.ContainsKey (v2Label))
            {
                VertexLabelMap.Add (v2Label, VertexLabelMap.Count);
                VertexIndexMap.Add (VertexIndexMap.Count, v2Label);
            }

            // Add edge and vertices
            if (!Edges.ContainsKey (VertexLabelMap[v1Label]))
            {
                Edges.Add (VertexLabelMap[v1Label], new HashSet<int>());
                GraphWeight += Vertices[VertexLabelMap[v1Label]];
            }
            if (!Edges.ContainsKey (VertexLabelMap[v2Label]))
            {
                Edges.Add (VertexLabelMap[v2Label], new HashSet<int> ());
                GraphWeight += Vertices[VertexLabelMap[v2Label]];
            }

            Edges[VertexLabelMap[v2Label]].Add (VertexLabelMap[v1Label]);
            Edges[VertexLabelMap[v1Label]].Add (VertexLabelMap[v2Label]);
        }
    }
}
