namespace JunimoIntelliBox.Plans
{
    using Microsoft.Xna.Framework;
    using StardewModdingAPI;
    using StardewValley;
    using StardewValley.Objects;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;


    public class FarmProductCollectionPlan : IPlan
    {
        private StardewValley.Object product;
        private List<KeyValuePair<Vector2, Chest>> chests;
        private Vector2 productLocation;
        private IMonitor monitor;

        public FarmProductCollectionPlan(IMonitor monitor, StardewValley.Object obj, Vector2 location, List<KeyValuePair<Vector2, Chest>> chests)
        {
            this.product = obj;
            this.chests = chests;
            this.productLocation = location;
            this.monitor = monitor;
        }

        public PlanExecutionResult ExecuteImmediate()
        {
            Item itemCopy = this.product.getOne();

            Chest chest = this.FindAcceptableChest(itemCopy);

            if (chest == null)
            {
                // Cannot find a chest to hold this item. Execution failed.
                return PlanExecutionResult.FAILED;
            }

            if (!this.RemoveProduct())
            {
                return PlanExecutionResult.FAILED;
            };

            this.AddToChest(chest, itemCopy);

            this.PlayerGainExperience();

            this.monitor.Log($"Picked up {this.product.Name} from Farm", LogLevel.Info);

            return PlanExecutionResult.SUCCESS;
        }

        public PlanExecutionResult Execute()
        {
            Item itemCopy = this.product.getOne();

            Chest chest = this.FindAcceptableChest(itemCopy);

            if (chest == null)
            {
                // Cannot find a chest to hold this item. Execution failed.
                return PlanExecutionResult.FAILED;
            }

            if (!this.RemoveProduct())
            {
                return PlanExecutionResult.FAILED;
            };

            this.AddToChest(chest, itemCopy);

            this.PlayerGainExperience();

            this.monitor.Log($"Picked up {this.product.Name} from Farm", LogLevel.Info);

            return PlanExecutionResult.SUCCESS;
        }

        private bool RemoveProduct()
        {
            GameLocation farm = Game1.getFarm();

            // Check if the item has already been picked up.
            if (!farm.Objects.ContainsKey(this.productLocation))
            {
                return false;
            }

            farm.Objects.Remove(this.productLocation);

            return true;
        }

        private void AddToChest(Chest chest, Item animalProduct)
        {
            chest.addItem(animalProduct);
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

        private void PlayerGainExperience()
        {
            Game1.player.gainExperience(2, 7);
        }
    }
}
