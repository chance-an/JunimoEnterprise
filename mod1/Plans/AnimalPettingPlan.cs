namespace JunimoIntelliBox.Plans
{
    using StardewModdingAPI;
    using StardewValley;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;


    public class AnimalPettingPlan : IPlan
    {
        private FarmAnimal animal;
        private IMonitor monitor;

        public AnimalPettingPlan(IMonitor monitor, FarmAnimal animal)
        {
            this.monitor = monitor;
            this.animal = animal;
        }

        public PlanExecutionResult Execute()
        {
            if (Game1.timeOfDay >= 1900)
            {
                // Animal sleeps, cannot pet.
                return PlanExecutionResult.FAILED;
            }

            Farmer farmer = Game1.player;

            if (!this.animal.wasPet)
            {
                this.animal.wasPet.Set(true);
                this.AdjustAnimalHappiess();

                this.PlayerGainExperience();
            }

            this.TuneLayEggValue();

            this.monitor.Log($"Petted {this.animal.name}", LogLevel.Info);
           
            return PlanExecutionResult.SUCCESS;
        }

        private void AdjustAnimalHappiess()
        {
            Farmer farmer = Game1.player;

            this.animal.friendshipTowardFarmer.Set(Math.Min(1000, this.animal.friendshipTowardFarmer + 15));
            if (farmer.professions.Contains(3) && !this.animal.isCoopDweller())
            {
                this.animal.friendshipTowardFarmer.Set(Math.Min(1000, this.animal.friendshipTowardFarmer + 15));
                this.animal.happiness.Set(Math.Min(byte.MaxValue, (byte)((uint)this.animal.happiness + (uint)Math.Max(5, 40 - (int)this.animal.happinessDrain))));
            }
            else if (farmer.professions.Contains(2) && this.animal.isCoopDweller())
            {
                this.animal.friendshipTowardFarmer.Set(Math.Min(1000, this.animal.friendshipTowardFarmer + 15));
                this.animal.happiness.Set(Math.Min(byte.MaxValue, (byte)((uint)this.animal.happiness + (uint)Math.Max(5, 40 - (int)this.animal.happinessDrain))));
            }
            this.animal.happiness.Set((byte)Math.Min((int)byte.MaxValue, (int)this.animal.happiness + Math.Max(5, 40 - (int)this.animal.happinessDrain)));
        }

        private void PlayerGainExperience()
        {
            Game1.player.gainExperience(0, 5);
        }

        private void TuneLayEggValue()
        {
            if (!this.animal.type.Equals("Sheep") || this.animal.friendshipTowardFarmer < 900)
            {
                return;
            }
            this.animal.daysToLay.Set((byte)2);
        }
    }
}
