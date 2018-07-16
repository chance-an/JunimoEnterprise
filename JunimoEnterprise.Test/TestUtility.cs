namespace JunimoEnterprise.Test
{
    using Harmony;
    using JunimoIntelliBox.Tools;
    using Netcode;
    using StardewValley;
    using StardewValley.Locations;
    using StardewValley.Network;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using GameGraph = JunimoEnterpriseNative.JEGraphUtilities.Graph<StardewValley.GameLocation, JunimoIntelliBox.Types.WarpCorrespondence>;

    public class TestUtility
    {

        private static HarmonyInstance harmony;

        public static GameGraph GetTestGraph()
        {
            GameGraph graph;

            if (Game1.bigCraftablesInformation == null)
            {
                Game1.bigCraftablesInformation = JunimoIntelliBox.Utility.DeserializeGraph<Dictionary<int, string>>(JunimoIntelliBox.Utility.BigCraftablesInformationFilePath);
            }

            if (Game1.objectInformation == null)
            {
                Game1.objectInformation = JunimoIntelliBox.Utility.DeserializeGraph<Dictionary<int, string>>(JunimoIntelliBox.Utility.ObjectInformationFilePath);
            }

            if (Game1.netWorldState == null) {
                IWorldState worldState = new MockNetWorldState();
                Game1.netWorldState = new NetRoot<IWorldState>(worldState);
            }

            if (Game1.content == null)
            {
                Game1.content = new MockLocalizedContentManager();
            }


            if (harmony == null)
            {
                HarmonyInstance harmony = HarmonyInstance.Create("JunimoEnterprise.Test");
                harmony.PatchAll(Assembly.GetExecutingAssembly());

            }

            if (Game1.player == null)
            {
                Game1.player = new Farmer();
            }

            graph = Serializer.DeserializeGraphXML();
            graph.rebuildNodesMapping();


            FarmHouse farmHouse = TestUtility.GetLocation<FarmHouse>(graph).FirstOrDefault();

            if (Game1.currentLocation == null)
            {
                Game1.currentLocation = farmHouse;
            }

            if (Game1.locations == null || Game1.locations.Count == 0)
            {
                Game1.locations = new List<GameLocation>(graph.nodes.Keys);
            }

            return graph;
        }

        public static IEnumerable<T> GetLocation<T>(GameGraph graph)
        {
            IEnumerable<GameLocation> gameLocations = graph.nodes.Keys;

            return FindTypeInEnumerable<T, GameLocation>(gameLocations);
        }

        public static IEnumerable<T> FindTypeInEnumerable<T, S>(IEnumerable<S> enumerable)
        {
            IEnumerable<S> filtered = enumerable.Where(elem => elem.GetType() == typeof(T));

            return filtered.Select(e => (T)Convert.ChangeType(e, typeof(T)));
        }
    }
}
