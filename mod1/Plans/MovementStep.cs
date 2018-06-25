namespace JunimoIntelliBox.Plans
{
    using StardewValley;
    using System;
    using JunimoIntelliBox.Types;
    using Microsoft.Xna.Framework;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using JunimoEnterpriseNative.JEGraphUtilities;
    using System.Xml.Serialization;
    using System.IO;
    using System.Runtime.Serialization;

    public class MovementStep : AbstractStep
    {
        private JunimoSlave junimo;
        private IStep dependentStep;
        private GameLocation destinationLocation;
        private LocationInformationHelper locationInformationHelper;

        public MovementStep(JunimoSlave junimo, GameLocation location, Point destination)
        {
            this.junimo = junimo;
            this.destinationLocation = location;
            this.locationInformationHelper = new LocationInformationHelper();
        }

        public override void FirstTick()
        {
            //Graph<GameLocation, WarpCorrespondence> testGraph = new Graph<GameLocation, WarpCorrespondence>();

            //IEnumerable<GameLocationNavigationNode> allWarpLocations =  this.locationInformationHelper.GetAllWarpLocations(this.junimo.currentLocation);

            GameLocation currentLocation = this.junimo.currentLocation;

            Func<GameLocation, IEnumerable<Tuple<GameLocation, WarpCorrespondence>>> discoverLogic = 
                (gameLocation) =>
                {
                    IEnumerable<GameLocationNavigationNode> allWarpLocations 
                        = this.locationInformationHelper.GetAllWarpLocations(gameLocation);

                    return allWarpLocations.Select(warpLocation => 
                        new Tuple<GameLocation, WarpCorrespondence>(warpLocation.next, warpLocation));
                };

            Graph<GameLocation, WarpCorrespondence> testGraph 
                = JunimoEnterpriseNative.GraphHelper.ConstructGraphByDiscovery<GameLocation, WarpCorrespondence>(currentLocation, discoverLogic);
            ////XmlSerializer s = new XmlSerializer(typeof(Graph<GameLocation, WarpCorrespondence>), "http://anchangsi.com");
            //XmlSerializer s = new XmlSerializer(typeof(Graph<GameLocation, WarpCorrespondence>), new Type[] { typeof(JunimoSlave), typeof(NPC) });
            //TextWriter writer = new StreamWriter("D:\\Develop\\Stardewvally\\mod1\\mod1\\serialized\\testGraph.xml");
            //s.Serialize(writer, testGraph);
            //writer.Close();
            //FileStream writer = new FileStream("D:\\Develop\\Stardewvally\\mod1\\mod1\\serialized\\testGraph.xml", FileMode.Create);
            //DataContractSerializer ser =
            //    new DataContractSerializer(typeof(Graph<GameLocation, WarpCorrespondence>));
            //ser.WriteObject(writer, testGraph);
            //writer.Close();

            JunimoIntelliBox.Utility.SerializeGraph(testGraph);


            //this.locationInformationHelper.FindLocationGraphPath(this.junimo.currentLocation, this.destinationLocation);
            if (this.junimo.currentLocation == this.destinationLocation)
            {
                return;
            } else
            {

            }
        }

        public override ExecutionResult Tick()
        {
            if (this.dependentStep != null)
            {
                ExecutionResult result = this.dependentStep.RunTick();

                if (result != ExecutionResult.CONTINUE)
                {
                    this.dependentStep = null;
                }

                return ExecutionResult.CONTINUE;
            }

            return ExecutionResult.CONTINUE;
        }

    }
}
