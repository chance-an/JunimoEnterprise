namespace JunimoIntelliBox.Plans
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using StardewModdingAPI;
    using StardewValley;
    using StardewValley.Buildings;
    using StardewValley.Objects;
    using System;
    using System.Collections.Generic;

    public abstract class AbstractMilkingShearingLikePlan: IPlan
    {
        protected FarmAnimal animal;
        protected Building building;
        protected List<KeyValuePair<Vector2, Chest>> chests;
        protected IMonitor monitor;

        public AbstractMilkingShearingLikePlan(IMonitor monitor, FarmAnimal animal, Building building, List<KeyValuePair<Vector2, Chest>> chests)
        {
            this.animal = animal;
            this.building = building;
            this.chests = chests;
            this.monitor = monitor;
        }

        public PlanExecutionResult Execute()
        {
            Item itemCopy = this.CreateItem();

            if (itemCopy == null)
            {
                return PlanExecutionResult.FAILED;
            }

            Chest chest = this.FindAcceptableChest(itemCopy);

            if (chest == null)
            {
                // Cannot find a chest to hold this item. Execution failed.
                return PlanExecutionResult.FAILED;
            }

            this.RemoveProduct();

            this.PlayerGainExperience();

            this.AddToChest(chest, itemCopy);

            this.OnExecutionSuccessful(itemCopy);

            return PlanExecutionResult.SUCCESS;
        }

        protected abstract void OnExecutionSuccessful(Item itemCopy);

        private Item CreateItem()
        {
            if (this.animal.currentProduce <= 0 || this.animal.age < (int)this.animal.ageWhenMature)
            {
                return null;
            }

            StardewValley.Object @object = new StardewValley.Object(Vector2.Zero, this.animal.currentProduce, (string)null, false, true, false, false);
            @object.quality.Set(this.animal.produceQuality);

            return @object;
        }

        private Chest FindAcceptableChest(Item animalProduct)
        {
            for (int i = 0; i < this.chests.Count; i++)
            {
                KeyValuePair<Vector2, Chest> pair = this.chests[i];

                Chest chest = pair.Value;

                if (JunimoIntelliBox.Utility.DoesChestHaveRoomForItem(chest, animalProduct))
                {
                    return chest;
                }
            }

            return null;
        }

        private void RemoveProduct()
        {
            this.animal.friendshipTowardFarmer.Set(Math.Min(1000, this.animal.friendshipTowardFarmer + 5));

            this.animal.currentProduce.Set(-1);

            if (this.animal.showDifferentTextureWhenReadyForHarvest)
            {
                this.animal.sprite.Value.LoadTexture("Animals\\Sheared" + this.animal.type);
            }
        }

        private void AddToChest(Chest chest, Item animalProduct)
        {
            chest.addItem(animalProduct);
        }

        private void PlayerGainExperience()
        {
            Game1.player.gainExperience(0, 5);
        }
    }
}
