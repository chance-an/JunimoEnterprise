namespace JunimoEnterprise.test
{
    using NUnit.Framework;
    using JunimoEnterpriseNative.JEGraphUtilities;
    using JunimoIntelliBox.Types;
    using StardewValley;
    using System;

    using GameGraph = JunimoEnterpriseNative.JEGraphUtilities.Graph<StardewValley.GameLocation, JunimoIntelliBox.Types.WarpCorrespondence>;
    using System.Collections.Generic;
    using System.Diagnostics;
    using JunimoIntelliBox.Algorithms;
    using JunimoIntelliBox.Tools;
    using StardewValley.Network;
    using Netcode;
    using JunimoEnterprise.Test;
    using Harmony;
    using System.Reflection;
    using StardewValley.Locations;
    using StardewValley.Objects;
    using Microsoft.Xna.Framework;
    using Nuclex.Testing.Xna;

    [TestFixture]
    public class GraphTest
    {
        GameGraph graph;

        public GraphTest()
        {
            
        }

        [HarmonyPatch(typeof(Farmer))]
        [HarmonyPatch("farmerInit")]
        [HarmonyPatch(new Type[] {})]
        public class MyPatchClass
        {
            public static bool Prefix()
            {
                return false;
            }

        }

        [HarmonyPatch(typeof(FarmHouse))]
        [HarmonyPatch("initNetFields")]
        [HarmonyPatch(new Type[] { })]
        public class MyPatchClass1
        {
            public static bool Prefix()
            {
                return false;
            }
        }

        [HarmonyPatch(typeof(Forest))]
        [HarmonyPatch("initNetFields")]
        [HarmonyPatch(new Type[] { })]
        public class MyPatchClass3
        {
            public static bool Prefix()
            {
                return false;
            }
        }


        [HarmonyPatch(typeof(Character))]
        [HarmonyPatch("faceDirection")]
        [HarmonyPatch(new Type[] { typeof(int)})]
        public class MyPatchClass2
        {
            public static bool Prefix(int direction)
            {
                return false;
            }
        }


        [SetUp]
        public void SetUp() {
            //graph = JunimoIntelliBox.Utility.DeserializeGraph();

            Dictionary<int, string> bigCraftablesInformation = JunimoIntelliBox.Utility.DeserializeGraph<Dictionary<int, string>>(JunimoIntelliBox.Utility.BigCraftablesInformationFilePath);
            Dictionary<int, string> objectInformation = JunimoIntelliBox.Utility.DeserializeGraph<Dictionary<int, string>>(JunimoIntelliBox.Utility.ObjectInformationFilePath);
            Game1.bigCraftablesInformation = bigCraftablesInformation;
            Game1.objectInformation = bigCraftablesInformation;

            IWorldState worldState = new MockNetWorldState();
            Game1.netWorldState = new NetRoot<IWorldState>(worldState);

            Game1.content = new MockLocalizedContentManager();

            HarmonyInstance harmony = HarmonyInstance.Create("JunimoEnterprise.Test");
            HarmonyInstance.DEBUG = true;
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            Game1.player = new Farmer();

            graph = Serializer.DeserializeGraphXML();
            graph.rebuildNodesMapping();
        }

        [Test]
        public void Test1()
        {
            StardewValley.GameLocation source = graph.GetNodeById(1);
            StardewValley.GameLocation target = graph.GetNodeById(12);

            Stopwatch stopWatch = new Stopwatch();

            stopWatch.Start();
            QuickGraphHelper.BfsSearch(graph, source, target);
            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:0000}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds);

            Assert.Pass("RunTime " + elapsedTime);

        }

        [Test]
        public void Test2()
        {
            Stopwatch stopWatch = new Stopwatch();

            StardewValley.GameLocation source = graph.GetNodeById(1);
            StardewValley.GameLocation target = graph.GetNodeById(12);
            stopWatch.Start();
            IList<StardewValley.GameLocation> result = JunimoEnterpriseNative.GraphHelper.graphSearch(graph, source, target);
            stopWatch.Stop();
            Assert.AreEqual(result.Count, 4);
            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:0000}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds);

            Assert.Pass("RunTime " + elapsedTime);
            
        }
    }
}
