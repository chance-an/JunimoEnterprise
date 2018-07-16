
namespace JunimoIntelliBox
{
    using Microsoft.Xna.Framework;
    using StardewValley;
    using StardewValley.Buildings;
    using StardewValley.Objects;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using JunimoEnterpriseNative;

    class LocationInformationHelper
    {
        static Random rnd = new Random();

        public IEnumerable<Building> GetAnimalFarmBuildings()
        {
            Farm farm = Game1.getFarm();

            List<Building> buildings = new List<Building>(farm.buildings);

            return buildings.Where(building =>
                building is Coop ||
                building is Barn);
        }

        public IEnumerable<KeyValuePair<Vector2, StardewValley.Object>> GetAnimalProducts(Building animalBuilding)
        {
            List<KeyValuePair<Vector2, StardewValley.Object>> result = new List<KeyValuePair<Vector2, StardewValley.Object>>();

            foreach (KeyValuePair<Vector2, StardewValley.Object> keyValuePair in animalBuilding.indoors.Value.Objects.Pairs)
            {
                StardewValley.Object product = keyValuePair.Value;
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

        public List<GameLocationNavigationNode> FindLocationGraphPath(GameLocation source, GameLocation destination) 
        {
            List<GameLocationNavigationNode> path = new List<GameLocationNavigationNode>();
            HashSet<string> visited = new HashSet<string>();

            Queue<Tuple<GameLocation, List<GameLocationNavigationNode>>> queue = new Queue<Tuple<GameLocation, List<GameLocationNavigationNode>>>();

            queue.Enqueue(new Tuple<GameLocation, List<GameLocationNavigationNode>>(source, new List<GameLocationNavigationNode>()));

            while (queue.Any())
            {
                Tuple<GameLocation, List<GameLocationNavigationNode>> entry = queue.Dequeue();
                GameLocation currentLocation = entry.Item1;
                visited.Add(currentLocation.Name);

                foreach (GameLocationNavigationNode nextNode in this.GetAllWarpLocations(currentLocation))
                {
                    if (visited.Contains(nextNode.TargetName))
                    {
                        continue;
                    }

                    GameLocation nextLocation = nextNode.next;

                    List<GameLocationNavigationNode> route = new List<GameLocationNavigationNode>(entry.Item2);

                    route.Add(nextNode);

                    if (nextNode.TargetName == destination.Name)
                    {
                        return route;
                    }

                    if (nextNode.next.uniqueName == destination.uniqueName)
                    {
                        return route;
                    }

                    queue.Enqueue(new Tuple<GameLocation, List<GameLocationNavigationNode>>(nextLocation, route));
                }; 

            }

            return path;
        }

        public IEnumerable<GameLocationNavigationNode> GetAllWarpLocations(GameLocation location)
        {
            Dictionary<string, List<Warp>> cascadedWraps = new Dictionary<string, List<Warp>>();

            foreach (Warp warp in location.warps)
            {
                if (!cascadedWraps.ContainsKey(warp.TargetName))
                {
                    cascadedWraps.Add(warp.TargetName, new List<Warp>());
                }

                List<Warp> list = cascadedWraps[warp.TargetName];
                list.Add(warp);
            }

            List<GameLocationNavigationNode> result = new List<GameLocationNavigationNode>();

            foreach(KeyValuePair<string, List<Warp>> pair in cascadedWraps)
            {
                string name = pair.Key;
                GameLocation nextLocation = Game1.getLocationFromName(name);
                if (nextLocation == null)
                {
                    continue;
                }

                // Choose one connection randomly.
                Warp warp = pair.Value[rnd.Next(pair.Value.Count)];
                result.Add(new GameLocationNavigationNode(location, nextLocation, warp.X, warp.Y, 
                    warp.TargetX, warp.TargetY, name));
            }

            Farm farm = location as Farm;

            if (farm != null)
            {
                foreach(Building building in farm.buildings)
                {
                    if (building.indoors == null)
                    {
                        continue;
                    }
                    Vector2 doorLocation = new Vector2(building.tileX + building.humanDoor.X, building.tileY + building.humanDoor.Y);

                    // TODO: Find the entrance near doorLocation
                    // TODO: find the location for indoor
                    result.Add(new GameLocationNavigationNode(location, building.indoors, (int)doorLocation.X, (int)doorLocation.Y,
                        0, 0, building.nameOfIndoors));
                }
            }

            return result;
        }

        public IEnumerable<GameLocationNavigationNode> GetAllWarpLocations(GameLocation from, GameLocation to)
        {
            List<GameLocationNavigationNode> result = new List<GameLocationNavigationNode>();

            foreach (Warp warp in from.warps)
            {
                string targetName = warp.TargetName;

                GameLocation nextLocation = Game1.getLocationFromName(targetName);

                if (nextLocation == to)
                {
                    result.Add(
                        new GameLocationNavigationNode(from, nextLocation, warp.X, warp.Y, 
                            warp.TargetX, warp.TargetY, targetName)
                    );
                }
            }

            Farm farm = from as Farm;

            if (farm != null)
            {
                foreach (Building building in farm.buildings)
                {
                    if (building.indoors == null)
                    {
                        continue;
                    }
                    Vector2 doorLocation = new Vector2(building.tileX + building.humanDoor.X, building.tileY + building.humanDoor.Y);

                    // TODO: Find the entrance near doorLocation
                    // TODO: find the location for indoor
                    result.Add(new GameLocationNavigationNode(from, building.indoors, (int)doorLocation.X, (int)doorLocation.Y,
                        0, 0, building.nameOfIndoors));
                }
            }

            return result;
        }

    }
}
