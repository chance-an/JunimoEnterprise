using JunimoEnterpriseNative.JEGraphUtilities;


namespace JunimoIntelliBox.Algorithms
{
    using JunimoIntelliBox.Types;
    using QuickGraph;
    using QuickGraph.Algorithms.Search;
    using StardewValley;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class QuickGraphHelper
    {
        public struct EdgeAdapter<TVertex> : IEdge<TVertex>, IEquatable<EdgeAdapter<TVertex>>
        {
            public EdgeAdapter(WarpCorrespondence warpCorrespondence, TVertex Source, TVertex Target)
            {
                this.Source = Source;
                this.Target = Target;
            }
            public TVertex Source { get; private set; }

            public TVertex Target { get; private set; }

            public bool Equals(QuickGraphHelper.EdgeAdapter<TVertex> other)
            {
                throw new NotImplementedException();
            }
        }

        public static void BfsSearch(Graph<GameLocation, WarpCorrespondence> graph, GameLocation s, GameLocation t)
        {

            DelegateVertexAndEdgeListGraph<GameLocation, EdgeAdapter<GameLocation>> g =
            new DelegateVertexAndEdgeListGraph<GameLocation, EdgeAdapter<GameLocation>>(
                graph.vertices(),
                delegate(GameLocation key, out IEnumerable<EdgeAdapter<GameLocation>> resultEdges)
            {
                int vertexId = graph.GetVertexID(key);

                List<int> adjacentVertices = graph.GetAdjacentVertexes(vertexId);

                List<EdgeAdapter<GameLocation>>  rEdges = new List<EdgeAdapter<GameLocation>>();

                GameLocation source = key;                

                foreach (int otherVertex in adjacentVertices)
                {
                    GameLocation target = graph.GetNodeById(otherVertex);

                    IList<WarpCorrespondence> edges = graph.GetEdge(vertexId, otherVertex);

                    foreach (WarpCorrespondence edge in edges)
                    {
                        if (vertexId < otherVertex)
                        {
                            rEdges.Add(new EdgeAdapter<GameLocation>(edge, source, target));
                        } else
                        {
                            rEdges.Add(new EdgeAdapter<GameLocation>(edge.Reverse(), source, target));
                        }
                    }
                }

                resultEdges = rEdges;

                return true;
            });

            // https://stackoverflow.com/questions/8606494/how-to-set-target-vertex-in-quickgraph-dijkstra-or-a

            BreadthFirstSearchAlgorithm<GameLocation, EdgeAdapter<GameLocation>> bfs 
                = new BreadthFirstSearchAlgorithm<GameLocation, EdgeAdapter<GameLocation>>(g);

            bfs.TreeEdge += (edge) =>
            {
                GameLocation from = edge.Source;
                GameLocation to = edge.Target;
            };

            bfs.DiscoverVertex += (v) =>
            {
                if (v == t)
                {
                    bfs.Abort();
                }
            };

            bfs.Compute(s);

        }
    }
}
