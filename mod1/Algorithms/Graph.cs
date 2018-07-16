

namespace JunimoIntelliBox.Algorithms
{
    using StardewValley;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using GameGraph = JunimoEnterpriseNative.JEGraphUtilities.Graph<StardewValley.GameLocation, JunimoIntelliBox.Types.WarpCorrespondence>;


    public class Graph
    {
        private static readonly Lazy<Graph> lazy =
        new Lazy<Graph>(() => new Graph());

        public static Graph Instance { get { return lazy.Value; } }

        private Graph()
        {
        }

        public IList<GameLocation> SearchPath(GameGraph graph, GameLocation source, GameLocation target)
        {
            return JunimoEnterpriseNative.GraphHelper.graphSearch(graph, source, target);
        }
    }
}
