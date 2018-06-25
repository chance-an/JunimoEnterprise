namespace JunimoEnterprise.test
{
    using NUnit.Framework;
    using JunimoEnterpriseNative.JEGraphUtilities;
    using JunimoIntelliBox.Types;
    using StardewValley;
    using System;

    using GameGraph = JunimoEnterpriseNative.JEGraphUtilities.Graph<StardewValley.GameLocation, JunimoIntelliBox.Types.WarpCorrespondence>;

    [TestFixture]
    public class GraphTest
    {
        GameGraph graph;

        public GraphTest()
        {
            
        }

        [SetUp]
        public void SetUp() {
            graph = JunimoIntelliBox.Utility.DeserializeGraph();
        }

        [Test]
        public void Test2()
        {

        }
    }
}
