#include <algorithm>
#include "stdafx.h"
#include "Graph.h"
#include "SerializableGenericDictionary.h"
#using <System.Core.dll>

using namespace System;
using namespace System::Collections::Generic;
using namespace System::Linq;

using namespace JunimoEnterpriseNative;
using namespace JunimoEnterpriseNative::JEGraphUtilities;

generic <class NodeType, class ConnectionType>
Graph<NodeType, ConnectionType>::Graph()
{
    this->nodes = gcnew Dictionary<NodeType, int>;
    this->reverseNodesLookUp = gcnew SerializableGenericDictionary<int, NodeType>;
    this->adjacentList = gcnew SerializableGenericDictionary<int, HashSet<int>^>;
    this->edges = gcnew SerializableGenericDictionary<SerializableTuple<int, int>^, List<ConnectionType>^>;
    this->_id_generator = 0;
    this->edge_id_assignment = gcnew Dictionary<ConnectionType, int>;
    this->edge_id_info = gcnew Dictionary<int, FromTo>;
}

generic <class NodeType, class ConnectionType>
Graph<NodeType, ConnectionType>::~Graph()
{
}

generic <class NodeType, class ConnectionType>
void Graph<NodeType, ConnectionType>::rebuildNodesMapping() {
    this->nodes->Clear();

    for each(KeyValuePair<int, NodeType>^ kvp in this->reverseNodesLookUp)
    {
         int key = kvp->Key;
         NodeType value = kvp->Value;

        this->nodes->Add(value, key);
    }
}

generic <class NodeType, class ConnectionType>
IEnumerable<NodeType>^ Graph<NodeType, ConnectionType>::vertices() {
    return this->nodes->Keys;
}

generic <class NodeType, class ConnectionType>
void Graph<NodeType, ConnectionType>::AddPath(NodeType node1, NodeType node2, ConnectionType connection) {
    if (!this->nodes->ContainsKey(node1)) {
        int id1 = this->getNewId();
        this->nodes->Add(node1, id1);
        this->reverseNodesLookUp->Add(id1, node1);
    }

    if (!this->nodes->ContainsKey(node2)) {
        int id2 = this->getNewId();
        this->nodes->Add(node2, id2);
        this->reverseNodesLookUp->Add(id2, node2);
    }
    
    int id1 = this->nodes[node1];
    int id2 = this->nodes[node2];

    if (!this->adjacentList->ContainsKey(id1)) {
        this->adjacentList->Add(id1, gcnew HashSet<int>());
    }

    (this->adjacentList[id1])->Add(id2);

    if (!this->adjacentList->ContainsKey(id2)) {
        this->adjacentList->Add(id2, gcnew HashSet<int>());
    }
    (this->adjacentList[id2])->Add(id1);

    this->addEdge(id1, id2, connection);
}

generic <class NodeType, class ConnectionType>
void Graph<NodeType, ConnectionType>::addEdge(int vertex1, int vertex2, ConnectionType edge) {
    SerializableTuple<int, int>^ key = gcnew SerializableTuple<int, int>(min(vertex1, vertex2), max(vertex1, vertex2));

    ConnectionType edgeToAdd = edge;

    if (vertex1 > vertex2) {
        edgeToAdd = edge->Reverse();
    }

    if (!this->edges->ContainsKey(key)) {
        this->edges->Add(key, gcnew List<ConnectionType>());
    }

    (this->edges[key])->Add(edgeToAdd);
}

generic <class NodeType, class ConnectionType>
IList<ConnectionType>^ Graph<NodeType, ConnectionType>::GetEdge(int node1, int node2) {
    const int from = min(node1, node2);
    const int to = max(node1, node2);
    SerializableTuple<int, int> key(from, to);
    return this->edges[%key];
}

generic <class NodeType, class ConnectionType>
int Graph<NodeType, ConnectionType>::GetVertexID(NodeType node1) {
    int id;
    bool hasKey = this->nodes->TryGetValue(node1, id);
    return hasKey ? id : -1;
}

generic <class NodeType, class ConnectionType>
List<int>^ Graph<NodeType, ConnectionType>::GetAdjacentVertexes(int id1) {
    HashSet<int>^ adjacentVertexes = this->adjacentList[id1];
    return System::Linq::Enumerable::ToList(adjacentVertexes);
}

generic <class NodeType, class ConnectionType>
const int Graph<NodeType, ConnectionType>::num_vertices() {
    int num_vertices = this->nodes->Count;
    return num_vertices;
}

generic <class NodeType, class ConnectionType>
void Graph<NodeType, ConnectionType>::getAdjacentvertices(int vertex, JunimoEnterpriseNative::Accessor*  p_vertexes) {
    List<int>^ adjacentVertices = this->GetAdjacentVertexes(vertex);
    p_vertexes->list = adjacentVertices;
}

generic <class NodeType, class ConnectionType>
void Graph<NodeType, ConnectionType>::vertices(JunimoEnterpriseNative::Accessor*  p_vertexes) {
    List<int>^ node_ids = System::Linq::Enumerable::ToList(this->nodes->Values);
    p_vertexes->list = node_ids;
}

generic <class NodeType, class ConnectionType>
void Graph<NodeType, ConnectionType>::out_edges(int vertex, JunimoEnterpriseNative::Accessor*  p_vertexes) {
    List<int>^ adjacentVertices = System::Linq::Enumerable::ToList(this->adjacentList[vertex]);

    List<int>^ allEdges = gcnew List<int>();
    for each (int targetVertex in adjacentVertices) {
        const int from = min(vertex, targetVertex);
        const int to = max(vertex, targetVertex);
        SerializableTuple<int, int> key(from, to);
        IList<ConnectionType>^ edges = this->edges[%key];
        
        for each (ConnectionType edge in edges) {
            const int edgeId = this->GetEdgeId(from, to, edge);
            allEdges->Add(edgeId);
        }
    }

    p_vertexes->list = allEdges;
};

generic <class NodeType, class ConnectionType>
unsigned int Graph<NodeType, ConnectionType>::out_degree(int vertex) {
    HashSet<int>^ adjacentVertices = this->adjacentList[vertex];

    int numberOfEdges = 0;
    for each (int targetVertex in adjacentVertices) {
        const int from = min(vertex, targetVertex);
        const int to = max(vertex, targetVertex);

        SerializableTuple<int, int> key(from, to);
        IList<ConnectionType>^ edges = this->edges[%key];

        numberOfEdges += edges->Count;
    }

    return numberOfEdges;
}

generic <class NodeType, class ConnectionType>
const int& Graph<NodeType, ConnectionType>::source(int edge) {
    FromTo fromTo = this->edge_id_info[edge];
    return fromTo.from;
}

generic <class NodeType, class ConnectionType>
const int& Graph<NodeType, ConnectionType>::target(int edge) {
    FromTo fromTo = this->edge_id_info[edge];
    return fromTo.to;
}

