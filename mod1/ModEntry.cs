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

        JunimosManager junimosManager;

        LocationInformationHelper locationInformationHelper;

        private System.Object plansAccessLock = new System.Object();

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

            this.planer = new Planner(this.Monitor);

            this.plans = null;

            this.junimosManager = new JunimosManager(this.Monitor);
        }

        private void TimeEvents_AfterDayStarted(object sender, EventArgs e)
        {
            //this.PlaceJunimo();
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
            lock(this.plansAccessLock)
            {
                this.plans = new Queue<IPlan>();

                IEnumerable<IPlan> productCollectionPlan = this.planer.CreateAnimalProductCollectionPlan(this.junimosManager);
                //Utility.AddAllToQueue(this.plans, productCollectionPlan);

                this.plans.Enqueue(productCollectionPlan.FirstOrDefault());


                //IEnumerable<IPlan> milkingPlans = this.planer.CreateMilkingPlan();
                //Utility.AddAllToQueue(this.plans, milkingPlans);

                //IEnumerable<IPlan> shearingPlans = this.planer.CreateShearingPlan();
                //Utility.AddAllToQueue(this.plans, shearingPlans);

                //IEnumerable<IPlan> truffleCollectionPlans = this.planer.CreateTruffleCollectionPlan();
                //Utility.AddAllToQueue(this.plans, truffleCollectionPlans);

                //IEnumerable<IPlan> animalPettingPlans = this.planer.CreateAnimalPettingPlans();
                //Utility.AddAllToQueue(this.plans, animalPettingPlans);
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
