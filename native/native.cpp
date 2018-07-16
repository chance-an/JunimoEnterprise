// native.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"
#include "Graph.h"
#include "BoostGraph.h"
#include "native.h"

#define _SILENCE_ALL_CXX17_DEPRECATION_WARNINGS

#include <boost/graph/breadth_first_search.hpp>

#using <System.dll>
#using <System.Core.dll>

#pragma managed
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

        

        generic <class NodeType, class ConnectionType>
            where ConnectionType : IEdgeDescriber<ConnectionType>
        static IList<NodeType>^ graphSearch(Graph<NodeType, ConnectionType>^ g, NodeType source, NodeType target) {

            UnmanagedGraph ugraph = UnmanagedGraph(g);
            int s = g->GetVertexID(source);
            int t = g->GetVertexID(target);
            int* result;
            int pathNode = 0;

            JunimoEnterpriseNative::GraphHelperNative::graphSearch(ugraph, s, t, &result, &pathNode);

            IList<NodeType>^ path = gcnew List<NodeType>();

            int i = 0;
            do {
                path->Add(g->GetNodeById(result[i]));
                i++;
            } while  ( i < pathNode);

            return path;
        }
	};
};


