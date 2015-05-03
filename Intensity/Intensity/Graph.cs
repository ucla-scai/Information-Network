using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intensity
{
    public class Node
    {
        public Graph Graph;
        public int Community = -1;
        public int Id;
        public ListDictionary<int, Node> Edges = new ListDictionary<int, Node>();

        public Node(Graph graph)
        {
            Graph = graph;
        }
    }

    public class Graph
    {
        public override string ToString()
        {
            var str = "";
            foreach (var node in _nodeEdges.ToList())
            {
                foreach (var edge in node.Value.Edges.ToList())
                {
                    str += "\n" + "c=" + node.Value.Community.ToString() + "," + node.Value.Id.ToString() + "-->" + edge.Id.ToString() + "," + "c=" + edge.Community.ToString();
                }
            }
            return str;
        }

        public Dictionary<int, Node> _nodeEdges = new Dictionary<int, Node>();

        public void AddNode(int node)
        {
            if (_nodeEdges.ContainsKey(node)) { return; }
            _nodeEdges[node] = new Node(this) { Id = node };
        }

        public void AddEdge(int a, int b)
        {
            if (!_nodeEdges.ContainsKey(a)) { AddNode(a); }
            if (!_nodeEdges.ContainsKey(b)) { AddNode(b); }

            if (!_nodeEdges[a].Edges.ContainsKey(b)) { _nodeEdges[a].Edges[b] = _nodeEdges[b]; }
            if (!_nodeEdges[b].Edges.ContainsKey(a)) { _nodeEdges[b].Edges[a] = _nodeEdges[a]; }
        }
    }
}
