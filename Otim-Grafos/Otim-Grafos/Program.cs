using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otim_Grafos
{
    class Program
    {
        public static string _graphInstancePath;

        static void Main (string[] args)
        {
            // Load configs
            Log ("Parsing Configs");
            if (!ParseConfigs(args))
            {
                Log ("Couldn't load all configs. Abort");
                System.Environment.Exit (-100);
            }

            // Load Instance
            Log ("Loading graph instance");
            WeightedGraph graph = new WeightedGraph (_graphInstancePath);

            // Solve using bruteforce
            List<Vertex> visited    = new List<Vertex> ();
            List<Vertex> notVisited = graph.GetVertices ();
            WeightedGraph ansGraph  = new WeightedGraph ();
            // Visit every vertex and 
            foreach (Vertex vertex in notVisited)
            {

                ansGraph.AddEdge ();
            }

            // Solve using backtrack
            //var answer = BackTrack ();
            // Solve using Branch & Bound
            // Solve using heuristics
            // Solve using GRASP
        }

        private static void Log (string message)
        {
            Console.WriteLine (String.Format ("{0:dd/MM/yyyy hh:mm:ss}\t{1}", DateTime.Now, message));
        }

        private static bool ParseConfigs (string[] args)
        {
            _graphInstancePath = ConfigurationManager.AppSettings.Get ("graphInstancePath");

            if (String.IsNullOrEmpty(_graphInstancePath))
            {
                return false;
            }

            return true;
        }
    }
}
