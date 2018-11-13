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
        public Dictionary<int, Dictionary<int, int>> Edges         { get; set; }
        public Dictionary<string, int>               VertexLabelMap { get; set; }
        public Dictionary<int, string>               VertexIndexMap { get; set; }


        public WeightedGraph(string path)
        {
            VertexLabelMap = new Dictionary<string, int> ();
            VertexIndexMap = new Dictionary<int, string> ();
            Edges          = new Dictionary<int, Dictionary<int, int>> ();
            LoadGraph (path);
        }

        public WeightedGraph()
        {
            VertexLabelMap = new Dictionary<string, int> ();
            VertexIndexMap = new Dictionary<int, string> ();
            Edges = new Dictionary<int, Dictionary<int, int>> ();
        }

        private void LoadGraph (string path)
        {
            List<string> lines = File.ReadAllLines (path).ToList ();

            foreach (string line in lines)
            {
                // Load line
                string[] splittedLine = line.Split ();

                AddEdge (splittedLine[0], splittedLine[1], splittedLine[2]);
            }
        }

        public void AddEdge (string v1Label, string v2Label, string edgeWeight)
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

            // Add edge
            if (!Edges.ContainsKey (VertexLabelMap[v1Label]))
            {
                Edges.Add (VertexLabelMap[v1Label], new Dictionary<int, int> ());
            }
            if (!Edges.ContainsKey (VertexLabelMap[v2Label]))
            {
                Edges.Add (VertexLabelMap[v2Label], new Dictionary<int, int> ());
            }

            Edges[VertexLabelMap[v2Label]].Add (VertexLabelMap[v1Label], int.Parse (edgeWeight));
            Edges[VertexLabelMap[v1Label]].Add (VertexLabelMap[v2Label], int.Parse (edgeWeight));
        }

        public List<Vertex> GetVertices ()
        {
            List<Vertex> vertices = new List<Vertex>();
            foreach (int vertexIndex in Edges.Keys)
            {
                vertices.Add (new Vertex(vertexIndex, VertexIndexMap[vertexIndex]));
            }

            return vertices;
        }
    }
}
