using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otim_Grafos
{
    class Program
    {
        public static TimeSpan  _maxRunTime;
        public static string    _solveMethod;
        public static string    _graphInstancePath;
        public static bool      _optimizeBacktrack;
        public static int       _backtrackCallCount = 0;
        public static Stopwatch _stopwatch          = new Stopwatch ();
        static void Main (string[] args)
        {
            Log ("Start!");
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

            switch (_solveMethod)
            {
                // Solve using backtrack
                case "backtrack":
                    SolveWithBackTrack (graph);
                    break;
                // Solve using heuristics
                case "heuristic":
                    SolveWithHeuristic (graph);
                    break;
            }
            Log ("End!");
        }

        private static void SolveWithHeuristic (WeightedGraph graph)
        {
            int             currentAnswerWeight = 0;
            int             candidateWeight     = 0;
            List<int>       answer              = new List<int> ();

            _stopwatch.Start ();
            foreach(int v in graph.Vertices.Keys)
            {
                candidateWeight = 0;

                // Select all neighbors
                List<int> neighbors         = Neighbors (v, graph);
                List<int> cliqueCandidate   = new List<int> ();

                // Start the candidate clique with the current vertex
                cliqueCandidate.Add (v);

                // Sort neighbors by combined Weight and degree
                List<KeyValuePair<int, int>> neighborsByWeightPlus  = SortNeighbors (v, neighbors, graph, "weight+");

                // For each neighbor check if it makes a clique with the candidate
                foreach (var neighbor in neighborsByWeightPlus)
                {
                    bool makeClique = true;
                    foreach (int vCand in cliqueCandidate)
                    {
                        // Check for maxTime
                        if (_stopwatch.Elapsed >= _maxRunTime)
                        {
                            Log ("Abort! Max Run Time exceeded!");
                            break;
                        }

                        if (!graph.Edges[vCand].Contains(neighbor.Key))
                        {
                            makeClique = false;
                            break;
                        }
                    }

                    if (makeClique)
                    {
                        cliqueCandidate.Add (neighbor.Key);
                    }

                    // Check for maxTime
                    if (_stopwatch.Elapsed >= _maxRunTime)
                    {
                        Log ("Abort! Max Run Time exceeded!");
                        break;
                    }
                }

                // Check if the new candidate is better than the current best answer
                candidateWeight = 0;
                cliqueCandidate.ForEach (c => candidateWeight += graph.Vertices[c]);
                Log (String.Format ("current weight: {0}", currentAnswerWeight));
                Log (String.Format ("candidate weight: {0}", candidateWeight));
                Log ("================================================");
                if (candidateWeight < currentAnswerWeight)
                {
                    continue;
                }
                else
                {
                    currentAnswerWeight = candidateWeight;
                    answer.Clear ();
                    answer.AddRange (cliqueCandidate);
                }

                // Check for maxTime
                if (_stopwatch.Elapsed >= _maxRunTime)
                {
                    Log ("Abort! Max Run Time exceeded!");
                    break;
                }
            }
            TimeSpan timeElapsed = _stopwatch.Elapsed;
            List<string> answerLabels = new List<string> ();
            answer.ForEach (v => answerLabels.Add (graph.VertexIndexMap[v]));
            Log (String.Format ("Max Clique: [{0}]", String.Join (", ", answerLabels)));
            Log (String.Format ("Max Clique Weight: {0}", currentAnswerWeight));
            using (StreamWriter sw = new StreamWriter (_graphInstancePath.Replace (".txt", "out.csv"), append: true))
            {
                sw.WriteLine (String.Format ("{0}\t{1}\t{2}", timeElapsed.ToString (), _graphInstancePath, currentAnswerWeight));
            }
        }

        private static List<KeyValuePair<int, int>> SortNeighbors (int v, List<int> neighbors, WeightedGraph graph, string mode)
        {
            List<KeyValuePair<int, int>> sortedList = new List<KeyValuePair<int,int>>();
         
            switch (mode)
            {
                case "weight":
                    foreach (int neighbor in neighbors)
                    {
                        sortedList.Add (new KeyValuePair<int, int> (neighbor, graph.Vertices[neighbor]));
                    }
                    break;
                case "degree":
                    foreach (int neighbor in neighbors)
                    {
                        sortedList.Add (new KeyValuePair<int, int> (neighbor, graph.Edges[neighbor].Count));
                    }
                    break;
                case "weight+":
                    foreach (int neighbor in neighbors)
                    {
                        sortedList.Add (new KeyValuePair<int, int> (neighbor, CalculateVertexCumulateWeight (neighbor, graph) - graph.Vertices[v] + graph.Vertices[neighbor]));
                    }
                    break;
            }

            sortedList.Sort ((pair1, pair2) => pair2.Value.CompareTo (pair1.Value));
            
            return sortedList;
        }

        private static int CalculateVertexCumulateWeight (int v, WeightedGraph graph)
        {
            int weight = 0;

            foreach (int neighbor in Neighbors(v, graph))
            {
                weight += graph.Vertices[neighbor];
            }

            return weight;
        }

        private static void SolveWithBackTrack (WeightedGraph graph)
        {
            List<int> R             = new List<int> ();
            List<int> P             = graph.Vertices.Keys.ToList();
            List<int> X             = new List<int> ();
            List<int> answerList    = new List<int> ();

            _stopwatch.Start ();
            BronKerbosch (R, P, X, graph, ref answerList);
            TimeSpan timeElapsed = _stopwatch.Elapsed;
            
            int ansWeight = 0;
            foreach (int v in answerList)
            {
                ansWeight += graph.Vertices[v];
            }
            List<string> answerLabels = new List<string> ();
            answerList.ForEach (v => answerLabels.Add(graph.VertexIndexMap[v]));
            Log (String.Format ("Max Clique: [{0}]", String.Join (", ", answerLabels)));
            Log (String.Format ("Max Clique Weight: {0}", ansWeight));
            Log (String.Format ("Number of Backtrack calls: {0}", _backtrackCallCount));
            Log (String.Format ("Optmized Backtrack: {0}", _optimizeBacktrack));
            using (StreamWriter sw = new StreamWriter(_graphInstancePath.Replace(".txt", "out.csv"), append: true))
            {
                sw.WriteLine (String.Format("{0}\t{1}\t{2}\t{3}\t{4}", timeElapsed.ToString(), _graphInstancePath, ansWeight, _optimizeBacktrack, _backtrackCallCount));
            }
        }

        private static bool BronKerbosch (List<int> R, List<int> P, List<int> X, WeightedGraph graph, ref List<int> answer)
        {
            _backtrackCallCount++;
            //Log ("======================================================");
            //Log (String.Format ("R: [{0}]", String.Join(", ", R)));
            //Log (String.Format ("P: [{0}]", String.Join(", ", P)));
            //Log (String.Format ("X: [{0}]", String.Join(", ", X)));

            int currentCliqueWeight     = 0;
            int possibleMaxCliqueWeight = 0;
            int candidateCliqueWeight   = 0;

            if (_optimizeBacktrack)
            {
                // Check if we can increase the max clique with the current set
                possibleMaxCliqueWeight = 0;
                foreach (int vPos in R.Union (P))
                {
                    possibleMaxCliqueWeight += graph.Vertices[vPos];
                }
                // calculate weight of maximal clique
                currentCliqueWeight = 0;
                foreach (int vAns in answer)
                {
                    currentCliqueWeight += graph.Vertices[vAns];
                }
                //Log (String.Format ("Possible Max Weight: {0}", possibleMaxCliqueWeight));
                //Log (String.Format ("Current Weight: {0}", currentCliqueWeight));
                if (possibleMaxCliqueWeight < currentCliqueWeight)
                {
                    return false;
                }
            }

            //if P and X are both empty:
            if (P.Count == 0 && X.Count == 0)
            {
                //report R as a maximal clique
                return true;
            } 

            int[] auxP = new int[P.Count];
            P.CopyTo (auxP);
            List<int> auxPList = auxP.ToList ();
            //for each vertex v in P:
            foreach (int v in P)
            {
                // Check for maxTime
                if (_stopwatch.Elapsed >= _maxRunTime)
                {
                    Log ("Abort! Max Run Time exceeded!");
                    return false;
                }

                List<int> Nv = Neighbors(v, graph);

                //BronKerbosch1(R ⋃ {v}, P ⋂ N(v), X ⋂ N(v))
                if (BronKerbosch (R.Union(new List<int> {v}).ToList(), auxPList.Intersect (Nv).ToList (), X.Intersect (Nv).ToList (), graph, ref answer))
                {
                    // calculate weight of maximal clique
                    currentCliqueWeight = 0;
                    foreach (int vAns in answer)
                    {
                        currentCliqueWeight += graph.Vertices[vAns];
                    }

                    Log (String.Format("current weight: {0}", currentCliqueWeight));
                    
                    candidateCliqueWeight = 0;
                    foreach (int vCand in R.Union (new List<int> { v }).ToList ())
                    {
                        candidateCliqueWeight += graph.Vertices[vCand];
                    }
                    Log (String.Format ("candidate weight: {0}", candidateCliqueWeight));
                    
                    // Save heaviest clique as answer
                    if (candidateCliqueWeight > currentCliqueWeight)
                    {
                        answer = R.Union (new List<int> { v }).ToList ();
                        Log (String.Format ("Candidate is the new max clique! Weight: {0}. Clique: [{1}]", candidateCliqueWeight, String.Join (", ", answer)));
                    }

                    Log ("================================================");
                }

                //P := P \ {v}
                auxPList.Remove (v);
                //X := X ⋃ {v}
                X = X.Union (new List<int> {v}).ToList();
            }

            return false;
        }

        private static List<int> Neighbors (int v, WeightedGraph graph)
        {
            return graph.Edges[v].ToList();
        }

        private static void Log (string message)
        {
            string line = String.Format ("{0:dd/MM/yyyy HH:mm:ss}\t{1}", DateTime.Now, message);
            Console.WriteLine (line);
            string logname = String.IsNullOrEmpty(_graphInstancePath) ? @"log.txt" : _graphInstancePath.Replace(".txt", "-log.txt");
            using (StreamWriter sw = new StreamWriter (logname, append: true))
            {
                sw.WriteLine (line);
            }
        }

        private static bool ParseConfigs (string[] args)
        {
            _graphInstancePath  = ConfigurationManager.AppSettings.Get ("graphInstancePath");
            _solveMethod        = ConfigurationManager.AppSettings.Get ("solveMethod");
            _optimizeBacktrack  = ConfigurationManager.AppSettings.Get ("optimizeBacktrack").Equals("1");
            _maxRunTime         = TimeSpan.FromSeconds(int.Parse(ConfigurationManager.AppSettings.Get ("maxRunTime")));

            if (String.IsNullOrEmpty(_graphInstancePath))
            {
                return false;
            }

            return true;
        }
    }
}
