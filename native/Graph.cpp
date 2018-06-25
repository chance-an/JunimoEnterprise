#include "stdafx.h"
#include "Graph.h"
#include "SerializableGenericDictionary.h"

using namespace System;
using namespace System::Collections::Generic;

using namespace JunimoEnterpriseNative;
using namespace JunimoEnterpriseNative::JEGraphUtilities;

generic <class NodeType, class ConnectionType>
Graph<NodeType, ConnectionType>::Graph()
{
    this->nodes = gcnew Dictionary<NodeType, Guid>;
    this->adjacentList = gcnew Dictionary<Guid, HashSet<Guid>^>;
    this->edges = gcnew Dictionary<Tuple<Guid, Guid>^, IList<ConnectionType>^>;
}


generic <class NodeType, class ConnectionType>
Graph<NodeType, ConnectionType>::~Graph()
{
}

generic <class NodeType, class ConnectionType>
void Graph<NodeType, ConnectionType>::AddPath(NodeType node1, NodeType node2, ConnectionType connection) {
    if (!this->nodes->ContainsKey(node1)) {
        this->nodes->Add(node1, Guid::NewGuid());
    }

    if (!this->nodes->ContainsKey(node2)) {
        this->nodes->Add(node2, Guid::NewGuid());
    }
    
    Guid^ guid1 = this->nodes[node1];
    Guid^ guid2 = this->nodes[node2];

    if (!this->adjacentList->ContainsKey(*guid1)) {
        this->adjacentList->Add(*guid1, gcnew HashSet<Guid>());
    }

    (this->adjacentList[*guid1])->Add(*guid2);

    if (!this->adjacentList->ContainsKey(*guid2)) {
        this->adjacentList->Add(*guid2, gcnew HashSet<Guid>());
    }
    (this->adjacentList[*guid2])->Add(*guid1);

    Tuple<Guid, Guid>^ key = gcnew Tuple<Guid, Guid>(*guid1, *guid2);

    if (!this->edges->ContainsKey(key)) {
        this->edges->Add(key, gcnew List<ConnectionType>());
    }
    
    (this->edges[key])->Add(connection);
}

generic <class NodeType, class ConnectionType>
IList<ConnectionType>^ Graph<NodeType, ConnectionType>::GetEdge(NodeType node1, NodeType node2) {
    return nullptr;
}
