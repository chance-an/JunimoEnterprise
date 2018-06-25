// native.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"
#include "Graph.h"

#using <System.dll>
#using <System.Core.dll>

using namespace System;
using namespace System::Collections::Generic;
using namespace JunimoEnterpriseNative::JEGraphUtilities;

namespace JunimoEnterpriseNative {

	public ref class GraphHelper
	{
	public:
        generic <class NodeType, class ConnectionType>
            where ConnectionType : IEdgeDescriber<ConnectionType>
        static Graph<NodeType, ConnectionType>^  ConstructGraphByDiscovery(NodeType initialGraphNode,
            Func<NodeType, IEnumerable<Tuple<NodeType, ConnectionType>^>^>^ discoverMore) {
            Graph<NodeType, ConnectionType>^ graph = gcnew Graph<NodeType, ConnectionType>();

            HashSet<NodeType>^ hash = gcnew HashSet<NodeType>;
            Queue<NodeType>^ queue = gcnew Queue<NodeType>;

            hash->Add(initialGraphNode);
            queue->Enqueue(initialGraphNode);

            while (queue->Count != 0) {
                NodeType currentNode = queue->Dequeue();
                IEnumerable<Tuple<NodeType, ConnectionType>^>^ adjacentNodeInfo = discoverMore(currentNode);

                for each (Tuple<NodeType, ConnectionType>^ tuple in adjacentNodeInfo)
                {
                    NodeType nextNode = tuple->Item1;
                    ConnectionType connection = tuple->Item2;

                    if (nextNode == nullptr) {
                        continue;
                    }

                    graph->AddPath(currentNode, nextNode, connection);

                    if (!hash->Contains(nextNode)) {
                        hash->Add(nextNode);
                        queue->Enqueue(nextNode);
                    }
                }
            }

            return graph;
		}
	};
}