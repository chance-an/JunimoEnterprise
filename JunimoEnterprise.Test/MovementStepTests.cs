

namespace JunimoEnterprise.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using GameGraph = JunimoEnterpriseNative.JEGraphUtilities.Graph<StardewValley.GameLocation, JunimoIntelliBox.Types.WarpCorrespondence>;
    using System.Diagnostics;
    using JunimoIntelliBox.Algorithms;
    using NUnit.Framework;
    using JunimoIntelliBox.Plans;
    using StardewValley.Locations;
    using JunimoIntelliBox;
    using StardewValley;

    [TestFixture]
    public class MovementStepTests
    {
        GameGraph graph;
        MovementStep movementStep;
        FarmHouse farmHouse;

        JunimoSlave junimoSlave;

        GameLocation destination;

        [SetUp]
        public void SetUp()
        {
            graph = TestUtility.GetTestGraph();
            var r = TestUtility.GetLocation<FarmHouse>(graph);
            farmHouse = r.FirstOrDefault();

            junimoSlave = TestUtility.FindTypeInEnumerable<JunimoSlave, StardewValley.NPC>(farmHouse.characters)
                .FirstOrDefault();

            junimoSlave.currentLocation = farmHouse;

            var animalHouses = TestUtility.GetLocation<AnimalHouse>(graph);

            destination = animalHouses.ToList()[1];

            movementStep = new MovementStep(junimoSlave, destination, new Microsoft.Xna.Framework.Point(0, 0));

            movementStep.overrideInitialMap = graph;
        }

        [Test]
        public void RunTickTest()
        {
            ExecutionResult executionResult;
            while (true)
            {
                executionResult = movementStep.RunTick();

                if (executionResult == ExecutionResult.CONTINUE)
                {
                    continue;
                }

                break;
            }
            


        }
    }
}
