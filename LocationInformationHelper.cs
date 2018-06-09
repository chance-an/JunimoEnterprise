
namespace JunimoIntelliBox
{
    using Microsoft.Xna.Framework;
    using StardewValley;
    using StardewValley.Buildings;
    using StardewValley.Objects;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    class LocationInformationHelper
    {
        public IEnumerable<Building> GetAnimalFarmBuildings()
        {
            Farm farm = Game1.getFarm();

            List<Building> buildings = new List<Building>(farm.buildings);

            return buildings.Where(building =>
                building is Coop ||
                building is Barn);
        }

        public IEnumerable<KeyValuePair<Vector2, Object>> GetAnimalProducts(Building animalBuilding)
        {
            List<KeyValuePair<Vector2, Object>> result = new List<KeyValuePair<Vector2, Object>>();
            foreach (KeyValuePair<Vector2, Object> keyValuePair in animalBuilding.indoors.Value.Objects.Pairs)
            {
                Object product = keyValuePair.Value;
                if ((new Regex("egg", RegexOptions.IgnoreCase)).IsMatch(product.Name.ToString()) ||
                    (new Regex("wool", RegexOptions.IgnoreCase)).IsMatch(product.Name.ToString()) ||
                    (new Regex("feather", RegexOptions.IgnoreCase)).IsMatch(product.Name.ToString()) ||
                    (new Regex("foot", RegexOptions.IgnoreCase)).IsMatch(product.Name.ToString())) {
                    result.Add(keyValuePair);
                }
            }

            return result;
        }

        public IEnumerable<FarmAnimal> GetFarmAnimals(GameLocation location)
        {
            if (!(location is Farm))
            {
                return new List<FarmAnimal>(); 
            }

            return ((Farm)location).animals.Values;
        }

        public IEnumerable<FarmAnimal> GetAllAnimals()
        {
            IEnumerable<FarmAnimal> allAnimalsOutside = this.GetFarmAnimals(Game1.getFarm());
            List<FarmAnimal> allAnimals = new List<FarmAnimal>(allAnimalsOutside);

            IEnumerable<Building> farmBuildings = this.GetAnimalFarmBuildings();

            foreach(Building farmBuilding in farmBuildings)
            {
                if (!(farmBuilding.indoors is AnimalHouse))
                {
                    continue;
                }

                foreach (KeyValuePair<long, FarmAnimal> pair in ((AnimalHouse)farmBuilding.indoors).animals.Pairs)
                {
                    allAnimals.Add(pair.Value);
                };
            }
            return allAnimals;
        }

        public List<KeyValuePair<Vector2, Chest>> GetNearestChestsToFarmBuilding(Building building)
        {
            Farm farm = Game1.getFarm();

            List<KeyValuePair<Vector2, Chest>> chests = new List<KeyValuePair<Vector2, Chest>>(this.FindAllInLocation<Chest>(farm));

            Vector2 doorLocation = new Vector2(building.tileX + building.humanDoor.X, building.tileY + building.humanDoor.Y);

            // Sort the chests
            chests.Sort(delegate(KeyValuePair<Vector2, Chest> a, KeyValuePair<Vector2, Chest> b) {
                return (int)(Vector2.Distance(a.Key, doorLocation) - Vector2.Distance(b.Key, doorLocation));
            });

            return chests;
        }

        public IEnumerable<KeyValuePair<Vector2, T>> FindAllInLocation<T>(GameLocation location) where T : StardewValley.Object
        {
            IList<KeyValuePair<Vector2, T>> result = new List<KeyValuePair<Vector2, T>>();

            foreach (KeyValuePair<Vector2, StardewValley.Object> entry in location.Objects.Pairs)
            {
                StardewValley.Object obj = entry.Value;
                Vector2 objectLocation = entry.Key;

                if (obj is T)
                {
                    result.Add(new KeyValuePair<Vector2, T>(objectLocation, (T)obj));
                }
            }

            return result;
        }
    }
}
