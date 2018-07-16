#pragma once
#pragma optimize("", off)
#pragma managed

#define _SILENCE_ALL_CXX17_DEPRECATION_WARNINGS

#include "Graph.h"
#include <vcclr.h>
#include <boost/graph/visitors.hpp>

namespace JunimoEnterpriseNative {
    // On gcroot: https://docs.microsoft.com/en-us/previous-versions/visualstudio/visual-studio-2010/481fa11f(v=vs.100)

    struct Accessor {
    public:
        Accessor() {
        }

        Accessor(const Accessor &old_obj) {
            this->list = old_obj.list;
        };

        Accessor(List<int>^ l) {
            this->list = l;
        }

        int get(int index) const {
            List<int>^ r = static_cast<List<int>^>(this->list);
            return r[index];
        }

        int size() const {
            return this->list->Count;
        }

        void operator=(const Accessor& another) {
            this->list = another.list;
        }

        gcroot<List<int>^> list;
    };

    namespace JEGraphUtilities {
        /* 
            Define an interface that the graphs have to follow, which enforces them to return int data. 
            This will be easier for the downstream processing in the unmanaged side.
        */
        public  ref class IIntDataGraph abstract {
        public:
            virtual void getAdjacentvertices(int vertex, JunimoEnterpriseNative::Accessor*  p_vertexes) = 0;
            virtual void vertices(JunimoEnterpriseNative::Accessor*  p_vertexes) = 0;
            virtual const int num_vertices() = 0;
            virtual void out_edges(int vertex, JunimoEnterpriseNative::Accessor*  p_vertexes) = 0;
            virtual unsigned int out_degree(int vertex) = 0;
            virtual const int& source(int edge) = 0;
            virtual const int& target(int edge) = 0;
        };

        struct UnmanagedGraph {
            gcroot<IIntDataGraph^> delegateGraph;

            /*typedef boost::no_property graph_property_type;
            typedef boost::no_property vertex_property_type;
            typedef boost::no_property edge_property_type;*/

            UnmanagedGraph(IIntDataGraph^ dG) {
                this->delegateGraph = dG;
            }

            inline void getAdjacentVertices(int vertexId, JunimoEnterpriseNative::Accessor*  p_vertexes) const {
                this->delegateGraph->getAdjacentvertices(vertexId, p_vertexes);
            }

            inline void getVertices(JunimoEnterpriseNative::Accessor*  p_vertexes) const {
                this->delegateGraph->vertices(p_vertexes);
            }

            inline const int num_vertices() const {
                return this->delegateGraph->num_vertices();
            }

            inline void out_edges(int vertexId, JunimoEnterpriseNative::Accessor*  p_vertexes) const {
                this->delegateGraph->out_edges(vertexId, p_vertexes);
            }

            inline const unsigned int out_degree(int vertex) const {
                return this->delegateGraph->out_degree(vertex);
            }

            inline const int& source(int edge) const {
                return this->delegateGraph->source(edge);
            }

            inline const int& target(int edge) const {
                return this->delegateGraph->target(edge);
            }
        };
    }
}

#pragma unmanaged
#include <boost/config.hpp>
#include <boost/graph/graph_traits.hpp>
#include <boost/graph/properties.hpp>

namespace boost {
    using UnmanagedGraph = JunimoEnterpriseNative::JEGraphUtilities::UnmanagedGraph;

    template <class _value_type >
    struct custom_property_map {
        typedef int key_type;
        typedef _value_type value_type;
        typedef boost::lvalue_property_map_tag category;

        custom_property_map(int size): map(size) {
            //map.reserve(size);
        }

        std::vector<_value_type> map;
    };

    template <class _value_type >
    struct property_traits<custom_property_map<_value_type>> {
        typedef _value_type value_type;
        typedef _value_type& reference;
        typedef int key_type;
        typedef boost::lvalue_property_map_tag category;
    };

    template <class value_type>
    typename value_type get(const custom_property_map<value_type>& map,
            typename custom_property_map<value_type>::key_type key)
    {
        int index = key - 1;
        return map.map[index];
    }

    template <class value_type >
    void put(custom_property_map<value_type>& property_map,
        typename custom_property_map<value_type>::key_type key,
        const value_type& value)
    {
        int index = key - 1;
        property_map.map.at(index) = value;
    }

    template <class value_type>
    typename value_type&
        at(const custom_property_map<value_type>& map,
            typename custom_property_map<value_type>::key_type key)
    {
        int index = key - 1;
        return map.map[index];
    }


    template <>
    struct graph_traits<typename UnmanagedGraph> {
        typedef int vertex_descriptor;
        typedef int edge_descriptor;

        class int_iterator
            : public iterator_facade<int_iterator,
            int,
            forward_traversal_tag,
            int,
            int>
        {
        public:
            int_iterator() {
            }

            int_iterator(const int_iterator& another): 
                base(another.base), 
                enumerator_accessor(another.enumerator_accessor){
            }

            // _adjacent_vertices will be freed after the call, so this class should make a copy
            int_iterator(JunimoEnterpriseNative::Accessor* _adjacent_vertices, int vertexIndex = 0)
                : base(vertexIndex), enumerator_accessor(*_adjacent_vertices){
            }
            JunimoEnterpriseNative::Accessor enumerator_accessor;

            void operator=(const int_iterator& another) {
                this->base = another.base;
                this->enumerator_accessor = another.enumerator_accessor;
            }

        private:
            int dereference() const { 
                return enumerator_accessor.get(this->base);
            }

            bool equal(const int_iterator& other) const
            {
                return base == other.base;
            }

            void increment() { base ++; }
            void decrement() { base --; }

            int base;

            friend class iterator_core_access;
        };

        typedef int_iterator adjacency_iterator;
        typedef int_iterator vertex_iterator;
        typedef int_iterator out_edge_iterator;


        typedef undirected_tag directed_category;
        typedef allow_parallel_edge_tag edge_parallel_category;
        typedef unsigned int vertices_size_type;
        typedef unsigned int edges_size_type;
        typedef unsigned int degree_size_type;

        typedef boost::no_property graph_property_type;
        typedef boost::no_property vertex_property_type;
        typedef boost::no_property edge_property_type;


        struct traversal_category :
            public virtual bidirectional_graph_tag,
            public virtual adjacency_graph_tag,
            public virtual vertex_list_graph_tag { };
    };

    inline std::pair<
        typename graph_traits<UnmanagedGraph >::adjacency_iterator,
        typename graph_traits<UnmanagedGraph >::adjacency_iterator >
        adjacent_vertices(
            typename graph_traits<UnmanagedGraph >::vertex_descriptor u,
            const UnmanagedGraph& g)
    {
        typedef graph_traits< UnmanagedGraph >::adjacency_iterator Iter;

        JunimoEnterpriseNative::Accessor  vertices;
        JunimoEnterpriseNative::Accessor*  p_Vertices = &vertices;
        g.getAdjacentVertices(u, p_Vertices);
        int numOfVertices = p_Vertices->size();

        return std::make_pair(Iter(p_Vertices, 0), Iter(p_Vertices, numOfVertices));
    };

    inline graph_traits< UnmanagedGraph >::vertices_size_type
        num_vertices(const UnmanagedGraph& g)
    {
        return g.num_vertices();
    };


    inline std::pair<
        typename graph_traits<UnmanagedGraph >::vertex_iterator,
        typename graph_traits<UnmanagedGraph >::vertex_iterator >
        vertices(const UnmanagedGraph& g)
    {
        typedef graph_traits< UnmanagedGraph >::vertex_iterator Iter;

        JunimoEnterpriseNative::Accessor  vertices;
        JunimoEnterpriseNative::Accessor*  p_Vertices = &vertices;
        g.getVertices(p_Vertices);
        int numOfVertices = p_Vertices->size();

        return std::make_pair(Iter(p_Vertices, 0), Iter(p_Vertices, numOfVertices));
    };

    inline std::pair<
        typename graph_traits<UnmanagedGraph >::out_edge_iterator,
        typename graph_traits<UnmanagedGraph >::out_edge_iterator>
        out_edges(
            typename graph_traits<UnmanagedGraph >::vertex_descriptor u,
            const UnmanagedGraph& g)
    {
        typedef graph_traits< UnmanagedGraph >::out_edge_iterator Iter;

        JunimoEnterpriseNative::Accessor  edges;
        JunimoEnterpriseNative::Accessor*  p_edges = &edges;
        g.out_edges(u, p_edges);
        int numOfEdges = p_edges->size();

        return std::make_pair(Iter(p_edges, 0), Iter(p_edges, numOfEdges));
    };

    inline graph_traits< UnmanagedGraph >::degree_size_type
        out_degree(
            typename graph_traits<UnmanagedGraph >::vertex_descriptor u,
            const UnmanagedGraph& g)
    {
        return g.out_degree(u);
    };

    inline graph_traits< UnmanagedGraph >::vertex_descriptor
        source(
            typename graph_traits<UnmanagedGraph >::edge_descriptor e,
            const UnmanagedGraph& g)
    {
        return g.source(e);
    };

    inline graph_traits< UnmanagedGraph >::vertex_descriptor
        target(
            typename graph_traits<UnmanagedGraph >::edge_descriptor e,
            const UnmanagedGraph& g)
    {
        return g.target(e);
    };

}