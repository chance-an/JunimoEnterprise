namespace JunimoIntelliBox.Plans
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using StardewValley.Objects;
    using StardewValley.Buildings;
    using StardewValley;
    using System;

    class AnimalProductCollectionPlan : AbstractStepBasedPlan, IPlan
    {
        private StardewValley.Object product;
        private Building building;
        private List<KeyValuePair<Vector2, Chest>>  chests;
        private Vector2 productLocation;
        private JunimosManager junimosManager;
        private JunimoSlave assignedJunimo;

        public AnimalProductCollectionPlan(Vector2 productLocation, StardewValley.Object product, Building building, List<KeyValuePair<Vector2, Chest>> chests, JunimosManager junimosManager)
        {
            this.productLocation = productLocation;
            this.product = product;
            this.building = building;
            this.chests = chests;
            this.junimosManager = junimosManager;
        }

        protected override Func<IStep>[] SetupSteps()
        {
            Chest chest = null;
            Item itemCopy = null;

            return new Func<IStep>[]
            {
                () => new SimpleProcedureStep(() => {
                    itemCopy = this.product.getOne();

                    chest = this.FindAcceptableChest(itemCopy);

                    if (this.assignedJunimo == null)
                    {
                        this.assignedJunimo = this.junimosManager.RequestJunimoToExecutePlan(this);
                    }

                    return ExecutionResult.SUCCESS;
                }),
                () => new MovementStep(this.assignedJunimo, this.building.indoors, new Point(0, 0)),
                () => new SimpleProcedureStep(() =>
                {
                    if (chest == null)
                    {
                        // Cannot find a chest to hold this item. Execution failed.
                        return ExecutionResult.FAILED;
                    }

                    this.RemoveProduct();

                    this.AddToChest(chest, itemCopy);

                    this.PlayerGainExperience();

                    return ExecutionResult.SUCCESS;
                })
            };
        }

        private void RemoveProduct()
        {
            this.building.indoors.Value.objects.Remove(this.productLocation);
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
