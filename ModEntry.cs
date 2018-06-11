using System;
using System.Collections.Generic;
using System.Linq;
using JunimoIntelliBox.Plans;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Buildings;

namespace JunimoIntelliBox
{
    public class ModEntry : Mod
    {
        private JunimoSlave junimo;

        private Planner planer;

        private Queue<IPlan> plans;

        PathFindController controller;

        LocationInformationHelper locationInformationHelper;

        private System.Object plansAccessLock = new System.Object();

        private bool dayHasStarted = false;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.ConsoleCommands.Add("dbg1", "???.", this.Dbg1);
            helper.ConsoleCommands.Add("collect", "???.", this.Collect);

            TimeEvents.AfterDayStarted += this.TimeEvents_AfterDayStarted;

            GameEvents.UpdateTick += this.Update;

            GameEvents.OneSecondTick += OneSecondTick_Update;

            this.planer = new Planner(this.Monitor);

            this.plans = null;
        }

        private void TimeEvents_AfterDayStarted(object sender, EventArgs e)
        {
            this.dayHasStarted = true;
            //this.PlaceJunimo();
        }

        private int countOfSeconds = 0; 
        private void OneSecondTick_Update(object sender, EventArgs e)
        {
            if (this.dayHasStarted)
            {
                this.countOfSeconds++;
            }

            if (this.countOfSeconds > 5)
            {
                this.countOfSeconds = 0;
                this.dayHasStarted = false;
                this.FiveSecondsAfterDayStart();
            }

            //this.PlaceJunimo();
        }

        private void FiveSecondsAfterDayStart()
        {
            this.Monitor.Log("Five seconds after day started", LogLevel.Info);
            this.Collect();
        }

        private void PlaceJunimo()
        {
            Vector2 playerPosition = Game1.player.position;
            Vector2 newNPCPosition = playerPosition + new Vector2( 0, 80);
            this.junimo = new JunimoSlave(newNPCPosition, this.Monitor);

            Game1.player.currentLocation.addCharacter(this.junimo);
        }

        private void Dbg1 (string command, string[] args)
        {
            List<Vector2> openSpaces = this.planer.AnalyzeLocationInfo(Game1.currentLocation);


            this.controller = new PathFindController(this.junimo, Game1.currentLocation, StardewValley.Utility.Vector2ToPoint(openSpaces[0]), 2);

            this.locationInformationHelper = new LocationInformationHelper();

            IEnumerable<Building> animalBuildings = this.locationInformationHelper.GetAnimalFarmBuildings();

            this.locationInformationHelper.GetFarmAnimals(Game1.getFarm());

            this.locationInformationHelper.GetNearestChestsToFarmBuilding(animalBuildings.First());
        }

        private void Collect(string command, string[] args)
        {
            this.Collect();
        }

        private void Collect()
        {
            lock(this.plansAccessLock)
            {
                this.plans = new Queue<IPlan>();

                IEnumerable<IPlan> productCollectionPlan = this.planer.CreateAnimalProductCollectionPlan();
                Utility.AddAllToQueue(this.plans, productCollectionPlan);


                IEnumerable<IPlan> milkingPlans = this.planer.CreateMilkingPlan();
                Utility.AddAllToQueue(this.plans, milkingPlans);

                IEnumerable<IPlan> shearingPlans = this.planer.CreateShearingPlan();
                Utility.AddAllToQueue(this.plans, shearingPlans);

                IEnumerable<IPlan> truffleCollectionPlans = this.planer.CreateTruffleCollectionPlan();
                Utility.AddAllToQueue(this.plans, truffleCollectionPlans);

                IEnumerable<IPlan> animalPettingPlans = this.planer.CreateAnimalPettingPlans();
                Utility.AddAllToQueue(this.plans, animalPettingPlans);
            }
        }

        private void Update(object sender, EventArgs e)
        {
            if (this.controller != null)
            {
                this.controller.update(Game1.currentGameTime);
            }

            lock (this.plansAccessLock)
            {
                if (this.plans == null)
                {
                    return;
                }

                if (this.plans.Count == 0)
                {
                    this.plans = null;
                    return;
                }

                IPlan plan = this.plans.Peek();

                PlanExecutionResult result = plan.Execute();

                if (result != PlanExecutionResult.CONTINUE)
                {
                    this.plans.Dequeue();
                }
            }
        }

    }
}
