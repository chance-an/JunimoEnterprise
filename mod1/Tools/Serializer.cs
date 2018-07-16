using JunimoEnterpriseNative;
using JunimoEnterpriseNative.JEGraphUtilities;
using JunimoIntelliBox.Types;
using StardewValley;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace JunimoIntelliBox.Tools
{
    public class Serializer
    {
        static string XMLNS = "http://www.sdv.com";
        const string defaultSerializePath = "D:\\Develop\\Stardewvally\\mod1\\mod1\\serialized\\testGraph.xml";
        static Serializer()
        {
            TypeDescriptor.AddAttributes(typeof(NPC), new XmlIncludeAttribute(typeof(JunimoSlave)));
            //TypeDescriptor.AddAttributes(typeof(Tuple<int, int>), new TypeConverterAttribute(typeof(TupleToStringConverter)));

            SerializableGenericDictionary<int, GameLocation>.AddAdditionalSerializeTypes(typeof(JunimoSlave));
            SerializableGenericDictionary<int, GameLocation>.AddAdditionalSerializeTypes(typeof(StardewValley.Characters.Horse));
            SerializableGenericDictionary<int, GameLocation>.AddAdditionalSerializeTypes(typeof(StardewValley.Characters.Cat));
            SerializableGenericDictionary<int, GameLocation>.AddAdditionalSerializeTypes(typeof(StardewValley.Characters.Child));
            SerializableGenericDictionary<int, GameLocation>.AddAdditionalSerializeTypes(typeof(StardewValley.Characters.Dog));
            SerializableGenericDictionary<int, GameLocation>.AddAdditionalSerializeTypes(typeof(StardewValley.Characters.Junimo));
            SerializableGenericDictionary<int, GameLocation>.AddAdditionalSerializeTypes(typeof(StardewValley.Characters.JunimoHarvester));
            SerializableGenericDictionary<int, GameLocation>.AddAdditionalSerializeTypes(typeof(StardewValley.Characters.Pet));
        }

        public static void SerializeGraphXML(Graph<GameLocation, WarpCorrespondence> graph)
        {
            XmlSerializer ser = new XmlSerializer(
                typeof(Graph<GameLocation, WarpCorrespondence>), 
                new Type[] { typeof(JunimoSlave)});
            string serilizedFile = defaultSerializePath;

            TextWriter writer = new StreamWriter(serilizedFile);
            ser.Serialize(writer, graph);
            writer.Close();
        }

        public static Graph<GameLocation, WarpCorrespondence> DeserializeGraphXML()
        {
            XmlSerializer serializer = new XmlSerializer(
                typeof(Graph<GameLocation, WarpCorrespondence>),
                new Type[] { typeof(JunimoSlave) });

            StreamReader reader = new StreamReader(defaultSerializePath);
            return (Graph<GameLocation, WarpCorrespondence>)serializer.Deserialize(reader);
        }
    }
}
