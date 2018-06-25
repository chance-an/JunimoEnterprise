#pragma once
#include "SerializableGenericDictionary.h"
#using <System.Core.dll>
#using <System.Xml.dll>
#using <System.Runtime.Serialization.dll>
#using <Newtonsoft.Json.dll>

using namespace System;
using namespace System::Collections::Generic;
using namespace System::Xml::Serialization;
using namespace System::Runtime::Serialization;
using namespace Newtonsoft::Json;
using namespace Newtonsoft::Json::Converters;
using namespace Newtonsoft::Json::Linq;
using namespace Newtonsoft::Json::Serialization;

namespace JunimoEnterpriseNative{
namespace JEGraphUtilities {
    generic <class EType>
    public interface class IEdgeDescriber
    {
        EType Reverse();
    };

    generic <class NodeType, class ConnectionType>
        where ConnectionType: IEdgeDescriber<ConnectionType>
    public ref class Graph
    {
    public:
        [JsonProperty]
        Dictionary<NodeType, Guid>^ nodes;

        [JsonProperty]
        Dictionary<Guid, HashSet<Guid>^>^ adjacentList;

        [JsonProperty]
        Dictionary<Tuple<Guid, Guid>^, IList<ConnectionType>^>^ edges;

    public:
        Graph();
        ~Graph();

        void AddPath(NodeType node1, NodeType node2, ConnectionType connection);
        IList<ConnectionType>^ GetEdge(NodeType node1, NodeType node2);
    };
}
}

