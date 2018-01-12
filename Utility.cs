

namespace JunimoIntelliBox
{
    using Microsoft.Xna.Framework;
    using StardewValley;
    using StardewValley.Objects;
    using System.Collections.Generic;
    using System.Linq;

    public class Utility
    {
        public static IEnumerable<Vector2> GetSurroundingTiles(Vector2 tileLocation)
        {
            Vector2[] offsets = new Vector2[] {
                new Vector2(0, 1),
                new Vector2(0, -1),
                new Vector2(1, 0),
                new Vector2(-1, 0),
                new Vector2(1, 1),
                new Vector2(1, -1),
                new Vector2(-1, 1),
                new Vector2(-1, -1)
            };

            return offsets.Select(offset => tileLocation + offset).
                Where(v => v.X >= 0 && v.Y >= 0);
        }

        public static bool DoesChestHaveRoomForItem(Chest chest, Item item)
        {
            int incomingStack = item.Stack;

            for (int index = 0; index < chest.items.Count; ++index)
            {
                if (chest.items[index] != null && chest.items[index].canStackWith(item))
                {
                    int currentStack = chest.items[index].Stack;
                    int maxStack = chest.items[index].maximumStackSize();

                    if (currentStack + incomingStack <= maxStack)
                    {
                        return true;
                    }
                    else
                    {
                        incomingStack = currentStack + incomingStack - maxStack;                       
                        continue;
                    }                    
                }
            }
            if (chest.items.Count >= 36)
                return false;

            return true;
        }

        public static void AddAllToQueue<T>(Queue<T> queue, IEnumerable<T> toAdd)
        {
            if (toAdd == null || queue == null)
            {
                return;
            }

            foreach(T element in toAdd)
            {
                queue.Enqueue(element);
            }
        }

        public static Vector2 TileToPixelDimension(Vector2 tile)
        {
            return new Vector2(tile.X * 64, tile.Y * 64);
        }
    }
}
