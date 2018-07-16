#pragma once

#include "SerializableGenericDictionary.h"
#include "BoostGraph.h"
#include "SerializableTuple.h"

#pragma managed
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

#pragma managed

namespace JunimoEnterpriseNative{
namespace JEGraphUtilities {
    generic <class EType>
    public interface class IEdgeDescriber
    {
        EType Reverse();
    };

    public value struct FromTo {
        int from;
        int to;
    };

    generic <class NodeType, class ConnectionType>
        where ConnectionType: IEdgeDescriber<ConnectionType>
    public ref class Graph: public IIntDataGraph
    {
    public:
        [XmlIgnore]
        Dictionary<NodeType, int>^ nodes;
        
        [XmlElement]
        SerializableGenericDictionary<int, NodeType>^ reverseNodesLookUp;

        [XmlElement]
        SerializableGenericDictionary<int, HashSet<int>^>^ adjacentList;

        [XmlElement]
        SerializableGenericDictionary<SerializableTuple<int, int>^, List<ConnectionType>^>^ edges;

    public:
        Graph();
        ~Graph();

        void AddPath(NodeType node1, NodeType node2, ConnectionType connection);
        IList<ConnectionType>^ GetEdge(int node1, int node2);
        int GetVertexID(NodeType node1);
        List<int>^ GetAdjacentVertexes(int node1);

        inline NodeType GetNodeById(int node_id) {
            return this->reverseNodesLookUp[node_id];
        }

        virtual void getAdjacentvertices(int vertex, JunimoEnterpriseNative::Accessor*  p_vertexes) override;
        virtual void vertices(JunimoEnterpriseNative::Accessor*  p_vertexes) override;
        virtual const int num_vertices() override;
        virtual void out_edges(int vertex, JunimoEnterpriseNative::Accessor*  p_vertexes) override;
        virtual unsigned int out_degree(int vertex) override;
        virtual const int& source(int edge) override;
        virtual const int& target(int edge) override;

        void rebuildNodesMapping();
        IEnumerable<NodeType>^ vertices();

    private:
        int _id_generator;

        inline int getNewId() {
            return ++this->_id_generator;
        }

        [XmlIgnore]
        Dictionary<ConnectionType, int>^ edge_id_assignment;

        [XmlIgnore]
        Dictionary<int, FromTo>^ edge_id_info;

        inline int GetEdgeId(int source, int target, ConnectionType edge) {
            int edgeId;

            bool hasKey = edge_id_assignment->TryGetValue(edge, edgeId);
            if (hasKey) {
                return edgeId;
            }

            // else
            edgeId = edge_id_assignment->Count + 1;
            edge_id_assignment->Add(edge, edgeId);

            edge_id_info->Add(edgeId, {source, target});
            return edgeId;
        }

        
        void addEdge(int vertex1, int vertex2, ConnectionType edge);
    };
}
}

