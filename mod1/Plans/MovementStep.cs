namespace JunimoIntelliBox.Plans
{
    using StardewValley;
    using System;
    using JunimoIntelliBox.Types;
    using Microsoft.Xna.Framework;
    using System.Collections.Generic;
    using System.Linq;
    using JunimoEnterpriseNative.JEGraphUtilities;
    using System.Diagnostics;

    public class MovementStep : AbstractStep
    {
        private JunimoSlave junimo;
        private GameLocation destinationLocation;
        private LocationInformationHelper locationInformationHelper;

        IList<GameLocation> initialNagivationBeacons;
        IList<GameLocation> nagivationBeacons;

        Movement movement;
        #region TestOverrides
        public Graph<GameLocation, WarpCorrespondence> overrideInitialMap;
        #endregion

        public MovementStep(JunimoSlave junimo, GameLocation location, Point destination)
        {
            this.junimo = junimo;
            this.destinationLocation = location;
            this.locationInformationHelper = new LocationInformationHelper();
        }

        public override void FirstTick()
        {
            if (this.nagivationBeacons == null)
            {
                this.nagivationBeacons = this.GetNavigationBeacons();                
            }

            this.initialNagivationBeacons = new List<GameLocation>(this.nagivationBeacons);

            Debug.Assert(this.junimo.currentLocation == this.nagivationBeacons[0]);

            //JunimoIntelliBox.Utility.SerializeJSON(Game1.bigCraftablesInformation, JunimoIntelliBox.Utility.BigCraftablesInformationFilePath);
            //JunimoIntelliBox.Utility.SerializeJSON(Game1.objectInformation, JunimoIntelliBox.Utility.ObjectInformationFilePath);
            //Serializer.SerializeGraphXML(testGraph);


            //this.locationInformationHelper.FindLocationGraphPath(this.junimo.currentLocation, this.destinationLocation);
            if (this.junimo.currentLocation == this.destinationLocation)
            {
                return;
            } else
            {
                this.movement = new Movement
                {
                    current = this.nagivationBeacons[0],
                    goal = this.nagivationBeacons[1],
                    junimo = this.junimo
                };
            }
        }

        public override ExecutionResult Tick()
        {
            if (this.movement != null)
            {
                ExecutionResult movementResult = this.movement.RunGenerator();

                if (movementResult == ExecutionResult.CONTINUE)
                {
                    return ExecutionResult.CONTINUE;
                }

                // Else warp Junimo,
                // Create new movement if possible. If not possible, conclude the process
                // Reduce nagivationBeacons
                // 
            }

            return ExecutionResult.SUCCESS;
        }

        private IList<GameLocation> GetNavigationBeacons()
        {
            GameLocation currentLocation = this.junimo.currentLocation;

            Func<GameLocation, IEnumerable<Tuple<GameLocation, WarpCorrespondence>>> discoverLogic =
                (gameLocation) =>
                {
                    IEnumerable<GameLocationNavigationNode> allWarpLocations
                        = this.locationInformationHelper.GetAllWarpLocations(gameLocation);

                    return allWarpLocations.Select(warpLocation =>
                        new Tuple<GameLocation, WarpCorrespondence>(warpLocation.next, warpLocation));
                };

            Graph<GameLocation, WarpCorrespondence> gameLocationMap;
            if (this.overrideInitialMap != null)
            {
                gameLocationMap = this.overrideInitialMap;
            } else
            {
                gameLocationMap = JunimoEnterpriseNative.GraphHelper.ConstructGraphByDiscovery<GameLocation, WarpCorrespondence>(currentLocation, discoverLogic);
            }

            return Algorithms.Graph.Instance.SearchPath(gameLocationMap, currentLocation, destinationLocation);
        }

        private class Movement: GeneratorRunner
        {
            public GameLocation current;
            public GameLocation goal;
            public JunimoSlave junimo;

            LocationInformationHelper locationInformationHelper;

            public Movement()
            {
                locationInformationHelper = new LocationInformationHelper();
            }

            public override IEnumerable<ExecutionResult> Run()
            {
                IEnumerable<GameLocationNavigationNode> warpInfo = locationInformationHelper.GetAllWarpLocations(this.current, this.goal);

                GameLocationNavigationNode navigationNode = warpInfo.FirstOrDefault();
                Point goalPoint = new Point(navigationNode.CurrentX, navigationNode.CurrentY);


                MovementWithinLocaiton subMovement = new MovementWithinLocaiton
                {
                    location = this.current,
                    junimo = this.junimo,
                    goal = goalPoint
                };

                while (true)
                {
                    ExecutionResult subMovementResult = subMovement.RunGenerator();

                    if (subMovementResult == ExecutionResult.SUCCESS)
                    {
                        break;
                    }

                    yield return subMovementResult;
                }

                // Warp junimo

                yield return ExecutionResult.SUCCESS;
            }
        }

        private class MovementWithinLocaiton: GeneratorRunner
        {
            public GameLocation location;
            public Point goal;
            public JunimoSlave junimo;

            public override IEnumerable<ExecutionResult> Run()
            {
                PathFindController pathFindController = new PathFindController(this.junimo, location, goal, 0);

                bool hasFinished = false;

                while (!hasFinished)
                {
                    hasFinished = pathFindController.update(Game1.currentGameTime);
                    yield return ExecutionResult.CONTINUE;
                }

                yield return ExecutionResult.SUCCESS;
            }
        }

    }
}
