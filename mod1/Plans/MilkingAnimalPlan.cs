﻿namespace JunimoIntelliBox.Plans
{
    using Microsoft.Xna.Framework;
    using StardewModdingAPI;
    using StardewValley;
    using StardewValley.Buildings;
    using StardewValley.Objects;
    using System.Collections.Generic;

    public class MilkingAnimalPlan : AbstractMilkingShearingLikePlan
    {
        public MilkingAnimalPlan(IMonitor monitor, FarmAnimal animal, Building building, List<KeyValuePair<Vector2, Chest>> chests):
            base(monitor, animal, building, chests)
        {
        }
        protected override void OnExecutionSuccessful(Item itemCopy) {
            this.monitor.Log($"Milked {this.animal.name}, and got {itemCopy.Name}", LogLevel.Info);
        }
    }
}
