namespace JunimoIntelliBox
{
    using Microsoft.Xna.Framework;
    using StardewModdingAPI;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using StardewValley;
    using StardewValley.Objects;
    using StardewValley.TerrainFeatures;
    using StardewValley.Buildings;
    using JunimoIntelliBox.Plans;
    using System.Text.RegularExpressions;

    public class Planner
    {
        private IMonitor monitor;

        private LocationInformationHelper locationInformationHelper;

        public Planner(IMonitor _monitor)
        {
            this.monitor = _monitor;

            this.locationInformationHelper = new LocationInformationHelper();
        }

        public List<Vector2> AnalyzeLocationInfo(GameLocation location)
        {
            foreach (KeyValuePair<Vector2, StardewValley.Object> entry in location.Objects)
            {
                StardewValley.Object obj = entry.Value;
                Vector2 objectLocation = entry.Key;

                this.monitor.Log($"Found {obj.Name} at {objectLocation.ToString()}", LogLevel.Info);
            }

            IEnumerable<KeyValuePair<Vector2, Chest>> chests = this.locationInformationHelper.FindAllInLocation<Chest>(location);

            this.monitor.Log($"Found {chests.Count()} chests");

            KeyValuePair<Vector2, Chest> chestAndLocation = chests.First();

            this.monitor.Log($"The first chest is at {chestAndLocation.Key.ToString()}", LogLevel.Info);

            foreach (Item item in chestAndLocation.Value.items)
            {
                this.monitor.Log($"Found {item.Stack} {item.Name} in chest", LogLevel.Info);
            }
            
            foreach (KeyValuePair<Vector2, TerrainFeature> tile in location.terrainFeatures )
            {
                TerrainFeature item = tile.Value;
                Vector2 loc = tile.Key;

                this.monitor.Log($"Found terrain feature {item.GetType().Name} at ${loc.ToString()}", LogLevel.Info);
            }

            List<Vector2> openSpaces = this.FindOpenSpaceAroundChest(location, chestAndLocation.Key, chestAndLocation.Value);

            return openSpaces;
        }

        public IEnumerable<IPlan> CreateAnimalProductCollectionPlan()
        {
            IEnumerable<Building> animalBuildings = this.locationInformationHelper.GetAnimalFarmBuildings();

            List<IPlan> plans = new List<IPlan>();

            foreach (Building building in animalBuildings)
            {
                IEnumerable<KeyValuePair<Vector2, StardewValley.Object>> products = this.locationInformationHelper.GetAnimalProducts(building);
                List<KeyValuePair<Vector2, Chest>> chests = this.locationInformationHelper.GetNearestChestsToFarmBuilding(building);

                this.monitor.Log($"FarmBuilding {building.buildingType} has {products.Count()} products", LogLevel.Info);

                foreach (KeyValuePair<Vector2, StardewValley.Object> keyValuePair in products)
                {
                    StardewValley.Object product = keyValuePair.Value;

                    plans.Add(new AnimalProductCollection(keyValuePair.Key, product, building, chests));

                    this.monitor.Log($"Created plan for {product.Name} inside {building.buildingType}", LogLevel.Info);
                }
            }

            return plans;
        }

        public IEnumerable<IPlan> CreateTruffleCollectionPlan()
        {
            IEnumerable<KeyValuePair<Vector2, StardewValley.Object>> allObjects = this.locationInformationHelper.FindAllInLocation<StardewValley.Object>(Game1.getFarm());

            IEnumerable<KeyValuePair<Vector2, StardewValley.Object>> truffles = allObjects.Where(pair => (new Regex("truffle", RegexOptions.IgnoreCase)).IsMatch(pair.Value.Name.ToString()));


            IEnumerable<FarmAnimal> animals = this.locationInformationHelper.GetAllAnimals();

            List<FarmAnimal> pigs = new List<FarmAnimal>(animals.Where(animal => (new Regex("pig", RegexOptions.IgnoreCase)).IsMatch(animal.type)));
            // Bug: pigs might be inside.

            if (truffles == null)
            {
                return null;
            }

            List<IPlan> plans = new List<IPlan>();

            foreach(KeyValuePair<Vector2, StardewValley.Object> pair in truffles)
            {
                Vector2 truffleLocation = Utility.TileToPixelDimension(pair.Key);

                pigs.Sort((a, b) => (int)(Vector2.Distance(truffleLocation, a.Position) - Vector2.Distance(truffleLocation, b.Position)));

                FarmAnimal closestPig = pigs.FirstOrDefault();

                if (closestPig == null)
                {
                    continue;
                }

                List<KeyValuePair<Vector2, Chest>> chests = this.locationInformationHelper.GetNearestChestsToFarmBuilding(closestPig.home);

                plans.Add(new FarmProductCollectionPlan(this.monitor, pair.Value, pair.Key, chests));
            }

            return plans;
        }

        public IEnumerable<IPlan> CreateMilkingPlan()
        {
            return this.CreateMilkingShearingLikePlans(delegate (FarmAnimal animal)
            {
                return animal.toolUsedForHarvest == "Milk Pail" && animal.currentProduce > 0 && animal.age >= animal.ageWhenMature;
            }, delegate (IMonitor monitor, FarmAnimal animal, Building building, List<KeyValuePair<Vector2, Chest>> chests)
            {
                return new MilkingAnimalPlan(monitor, animal, building, chests);
            });
        }

        public IEnumerable<IPlan> CreateShearingPlan()
        {
            return this.CreateMilkingShearingLikePlans(delegate (FarmAnimal animal)
            {
                return animal.toolUsedForHarvest == "Shears" && animal.currentProduce > 0 && animal.age >= animal.ageWhenMature;
            }, delegate (IMonitor monitor, FarmAnimal animal, Building building, List<KeyValuePair<Vector2, Chest>> chests)
            {
                return new ShearingAnimalPlan(monitor, animal, building, chests);
            });
        }

        public IEnumerable<IPlan> CreateAnimalPettingPlans()
        {
            IEnumerable<FarmAnimal> animals = this.locationInformationHelper.GetAllAnimals();
            if (animals == null)
            {
                return null;
            }

            List<IPlan> result = new List<IPlan>();

            foreach (FarmAnimal animal in animals)
            {
                result.Add(new AnimalPettingPlan(this.monitor, animal));
            }

            return result;
        }

        private IEnumerable<IPlan> CreateMilkingShearingLikePlans(Func<FarmAnimal, Boolean> actionableAnimalFilter, 
            Func<IMonitor, FarmAnimal, Building, List<KeyValuePair<Vector2, Chest>>, IPlan> planCreator)
        {
            IEnumerable<FarmAnimal> animals = this.locationInformationHelper.GetAllAnimals();

            IEnumerable<FarmAnimal> actionableAnimals = animals.Where(animal => actionableAnimalFilter(animal));

            if (actionableAnimals == null || !actionableAnimals.Any())
            {
                return null;
            }

            List<IPlan> result = new List<IPlan>();

            HashSet<Building> allBuildings = new HashSet<Building>();

            foreach (FarmAnimal animal in actionableAnimals)
            {
                allBuildings.Add(animal.home);
            }

            Dictionary<Building, List<KeyValuePair<Vector2, Chest>>> buildingChestsMapping = new Dictionary<Building, List<KeyValuePair<Vector2, Chest>>>();

            foreach (Building building in allBuildings)
            {
                List<KeyValuePair<Vector2, Chest>> chests = this.locationInformationHelper.GetNearestChestsToFarmBuilding(building);
                buildingChestsMapping.Add(building, chests);
            }

            foreach (FarmAnimal animal in actionableAnimals)
            {
                result.Add(planCreator(this.monitor, animal, animal.home, buildingChestsMapping[animal.home]));
            }
            return result;
        }

        private List<Vector2> FindOpenSpaceAroundChest(GameLocation location, Vector2 chestLocation, Chest chest)
        {
            IEnumerable<Vector2> suroundings = Utility.GetSurroundingTiles(chestLocation);

            IEnumerable<Vector2> openFreeTiles = suroundings.Where(delegate (Vector2 tile)
            {
                return location.isTileLocationTotallyClearAndPlaceable(tile);
            });

            // Order the freeTiles by their distance to the chest.
            List<Vector2> ordered = new List<Vector2>(openFreeTiles);

            ordered.Sort(delegate(Vector2 a, Vector2 b) {
                return (int)(Vector2.Distance(a, chestLocation) - Vector2.Distance(b, chestLocation));
            });

            return ordered;
        }

        
    }
}
