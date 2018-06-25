

namespace JunimoIntelliBox
{
    using JunimoEnterpriseNative.JEGraphUtilities;
    using JunimoIntelliBox.Types;
    using Microsoft.Xna.Framework;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Linq;
    using Newtonsoft.Json.Serialization;
    using StardewValley;
    using StardewValley.Objects;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;

    using GameGraph = JunimoEnterpriseNative.JEGraphUtilities.Graph<StardewValley.GameLocation, Types.WarpCorrespondence>;

    public class Utility
    {
        static Utility() {
            TypeDescriptor.AddAttributes(typeof(GameLocation), new TypeConverterAttribute(typeof(GameLocationToStringTypeConvertor)));
            TypeDescriptor.AddAttributes(typeof(Tuple<Guid,Guid>), new TypeConverterAttribute(typeof(TupleToStringConverter)));
        }

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

            foreach (T element in toAdd)
            {
                queue.Enqueue(element);
            }
        }

        public static Vector2 TileToPixelDimension(Vector2 tile)
        {
            return new Vector2(tile.X * 64, tile.Y * 64);
        }

        public static void SerializeGraph(GameGraph graph)
        {
            JsonSerializer serializer = new JsonSerializer();
            //serializer.Converters.Add(new JavaScriptDateTimeConverter());
            serializer.NullValueHandling = NullValueHandling.Ignore;
            serializer.TypeNameHandling = TypeNameHandling.All;

            ITraceWriter traceWriter = new MemoryTraceWriter();

            serializer.TraceWriter = traceWriter;

            using (StreamWriter sw = new StreamWriter("D:\\Develop\\Stardewvally\\mod1\\mod1\\serialized\\testGraph.xml"))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, graph);
            }

            Console.WriteLine(traceWriter.ToString());
        }

        public static GameGraph DeserializeGraph(string datafile = @"D:\Develop\Stardewvally\mod1\mod1\serialized\testGraph.xml")
        {
            JsonSerializer serializer = new JsonSerializer();
            serializer.NullValueHandling = NullValueHandling.Ignore;
            serializer.TypeNameHandling = TypeNameHandling.All;

            ITraceWriter traceWriter = new MemoryTraceWriter();

            serializer.TraceWriter = traceWriter;

            GameGraph graph;

            using (StreamReader sr = new StreamReader(datafile))
            using (JsonTextReader reader = new JsonTextReader(sr))
            {
                graph = serializer.Deserialize<GameGraph>(reader);
            }

            Console.WriteLine(traceWriter.ToString());

            return graph;
        }

        public class GameLocationCreator : CustomCreationConverter<GameLocation>
        {
            public override GameLocation Create(Type objectType)
            {
                throw new NotImplementedException();
            }
        }

        public class GameLocationToStringTypeConvertor: TypeConverter {
            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            {
                if (sourceType == typeof(string))
                    return true;

                return base.CanConvertFrom(context, sourceType);
            }
            public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
            {
                if (value.GetType() != typeof(string))
                    return base.ConvertFrom(context, culture, value);

                return new JunimoIntelliBox.Mocks.MockGameLocation((string)value);
            }
        }

        public class TupleToStringConverter: TypeConverter
        {
            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            {
                if (sourceType == typeof(string))
                    return true;

                return base.CanConvertFrom(context, sourceType);
            }
            public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
            {
                if (value.GetType() != typeof(string))
                    return base.ConvertFrom(context, culture, value);

                string str = value as string;
                str = str.Trim(new char[] { '(', ')' });
                string[] parts = str.Split(',');

                return new Tuple<Guid, Guid>(new Guid(parts[0]), new Guid(parts[1]));
            }
        }
    }
}
