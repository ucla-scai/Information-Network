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

        public string ToDot()
        {
            Dictionary<string, bool> seen = new Dictionary<string, bool>();
            var dot = "graph G {\n";
            var cluster = 0;
            var communities = _nodes.Values.ToList().Select(s => s.Community).Distinct().ToList();
            foreach (var community in communities)
            {
                dot += "subgraph cluster" + cluster.ToString() + " {\n";
                foreach (var node in _nodes)
                {
                    if (node.Value.Community != community) { continue; }
                    
                    foreach (var edge in node.Value.Edges.ToList().Where(e => e.Node.Community == node.Value.Community))
                    {
                        var nodeEdge = node.Value.Id.ToString() + " -- " + edge.Node.Id.ToString();
                        var reverse = edge.Node.Id.ToString() + " -- " + node.Value.Id.ToString();
                        if (seen.ContainsKey(nodeEdge) || seen.ContainsKey(reverse)) { continue; }
                        dot += nodeEdge + " [ label=\"" + edge.Weight.ToString() + "\" ];\n";
                        seen[nodeEdge] = true;
                    }
                }
                dot += "label = \"" + community.ToString() + "\"\n";
                dot += "style=dashed\n";
                dot += "}\n";
                cluster++;
            }

            foreach (var node in _nodes)
            {
                foreach (var edge in node.Value.Edges.ToList())
                {
                    var nodeEdge = node.Value.Id.ToString() + " -- " + edge.Node.Id.ToString();
                    var reverse = edge.Node.Id.ToString() + " -- " + node.Value.Id.ToString();
                    if (seen.ContainsKey(nodeEdge) || seen.ContainsKey(reverse)) { continue; }
                    dot += nodeEdge + " [ label=\"" + edge.Weight.ToString() + "\" ];\n";
                    seen[nodeEdge] = true;
                }
            }
            foreach (var node in _nodes)
            {
                var shape = node.Value.IsAdvertiser ? "ellipse,style=filled,color=firebrick,fontcolor=white" : "box,style=filled,color=grey,fontcolor=white";
                dot += node.Value.Id.ToString() + " [shape=" + shape + "];\n";
            }
            dot += "}";
            return dot;
        }
    }
}
