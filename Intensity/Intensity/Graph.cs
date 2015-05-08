using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intensity
{
    public class Edge
    {
        public Node Node;
        public decimal Weight;
        public decimal Price;
    }

    public class Node
    {
        public Graph Graph;
        public int Community = -1;
        public int Id;
        public ListDictionary<int, Edge> Edges = new ListDictionary<int, Edge>();
        public bool IsAdvertiser;

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
            foreach (var node in _nodes.ToList())
            {
                foreach (var edge in node.Value.Edges.ToList())
                {
                    str += "\n" + "c=" + node.Value.Community.ToString() + "," + node.Value.Id.ToString() + "-->" + edge.Node.Id.ToString() + "," + "c=" + edge.Node.Community.ToString();
                }
            }
            return str;
        }

        private Dictionary<int, Node> _nodes = new Dictionary<int, Node>();

        public Dictionary<int, Node> Nodes { get { return _nodes; } }

        public void AddNode(int node, bool isAdvertiser)
        {
            if (_nodes.ContainsKey(node)) { return; }
            _nodes[node] = new Node(this) { Id = node, IsAdvertiser = isAdvertiser };
        }

        public void AddEdge(int a, bool aIsAdvertiser, int b, bool bIsAdvertiser, decimal weight, decimal price)
        {
            if (!_nodes.ContainsKey(a)) { AddNode(a, aIsAdvertiser); }
            if (!_nodes.ContainsKey(b)) { AddNode(b, bIsAdvertiser); }

            if (!_nodes[a].Edges.ContainsKey(b)) { _nodes[a].Edges[b] = new Edge() { Node = _nodes[b], Price = price, Weight = weight }; }
            if (!_nodes[b].Edges.ContainsKey(a)) { _nodes[b].Edges[a] = new Edge() { Node = _nodes[a], Price = price, Weight = weight }; }
        }
    }
}
