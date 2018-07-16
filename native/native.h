#include "Graph.h"

#pragma once
#pragma unmanaged

#define _SILENCE_ALL_CXX17_DEPRECATION_WARNINGS

#include "BoostGraph.h"
#include <boost/graph/breadth_first_search.hpp>
#include <map>

#using <System.dll>

namespace JunimoEnterpriseNative {
    using namespace boost;
    using namespace JunimoEnterpriseNative::JEGraphUtilities;

    struct BFS_Accessor : public default_bfs_visitor {
        struct done {};

        BFS_Accessor(int num_vertices, int goal): goal(goal) {
            // +1, so we avoid converting vertex id (1 more than index) to array index.
            const int array_size = num_vertices + 1;
            this->predecessor_map = std::make_shared<int *>(new int[array_size]);
            memset(*this->predecessor_map.get(), 0, sizeof(int)*array_size);
        };

        ~BFS_Accessor() {
        }

        const int* getData() {
            return *this->predecessor_map.get();
        }

        void examine_vertex(int u, const UnmanagedGraph & g) const
        {
            typedef graph_traits<UnmanagedGraph>::vertex_descriptor Vertex;
            typedef graph_traits<UnmanagedGraph>::vertex_iterator Iter;

            if (u == this->goal) {
                throw done();
            };

            std::pair<Iter, Iter> pair = adjacent_vertices(u, g);

            Iter& ptr = pair.first;

            do {
                Vertex v = *ptr;

                int* data = *this->predecessor_map.get();
                if (data[v] == 0) {
                    data[v] = u;
                }

                if (ptr == pair.second) {
                    break;
                }

                ptr++;
            } while (ptr != pair.second);
        };

    private:
        std::shared_ptr<int*> predecessor_map;

        const graph_traits<typename UnmanagedGraph>::vertex_descriptor goal;
    };

    struct GraphHelperNative {
        inline static void graphSearch(UnmanagedGraph & graph, int source, int target, int ** result, int* path_node_count) {
            typedef boost::custom_property_map<boost::default_color_type> color_map_t;

            boost::queue<int> Q;
            const int n_v = boost::num_vertices(graph);

            color_map_t colorMap(n_v);

            BFS_Accessor visitor(n_v, target);

            try {
                boost::breadth_first_search(graph, source, Q, visitor, colorMap);
            }
            catch (BFS_Accessor::done const &) {
            }
            
            const int* data = visitor.getData();

            int n = target;
            int& p = *path_node_count;
            p = 1;
            while (n != source) {
                n = data[n];
                p++;
            }

            *result = new int[p];
            int* r = *result;
            int i = p - 1;

            r[i] = target;
            while (target != source) {
                target = data[target];
                r[--i] = target;
            }
        }
    };
}