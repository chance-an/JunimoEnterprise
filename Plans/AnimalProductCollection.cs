namespace JunimoIntelliBox.Plans
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using StardewValley.Objects;
    using StardewValley.Buildings;
    using StardewValley;

    class AnimalProductCollection : IPlan
    {
        private StardewValley.Object product;
        private Building building;
        private List<KeyValuePair<Vector2, Chest>>  chests;
        private Vector2 productLocation;

        public AnimalProductCollection(Vector2 productLocation, StardewValley.Object product, Building building, List<KeyValuePair<Vector2, Chest>> chests)
        {
            this.productLocation = productLocation;
            this.product = product;
            this.building = building;
            this.chests = chests;
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

            this.RemoveProduct();

            this.AddToChest(chest, itemCopy);

            this.PlayerGainExperience();

            return PlanExecutionResult.SUCCESS;
        }

        private void RemoveProduct()
        {
            this.building.indoors.objects.Remove(this.productLocation);
        }

        private void AddToChest(Chest chest, Item animalProduct)
        {
            chest.addItem(animalProduct);
        }

        private Chest FindAcceptableChest(Item animalProduct)
        {
            for(int i = 0; i < this.chests.Count; i++)
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
            Game1.player.gainExperience(0, 5);
        }
    }
}
