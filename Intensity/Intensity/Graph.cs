using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intensity
{
    public class Edge
    {
        public Node Node;
        public float Weight;
    }

    public class Node
    {
        public Graph Graph;

        private bool _dirty = false;
        private int _community = -1;
        public int Community { get { return _community; } set { _dirty = true; _community = value; } }
        public int Id;

        public ListDictionary<int, Edge> Edges = new ListDictionary<int, Edge>();
        public bool IsAdvertiser;
        public bool IsDirty { get { return _dirty; } set { _dirty = value; } }

        public float E_max_v = -1;
        public float I_v = -1;
        public float Score = -1;

        public Node(Graph graph) { Graph = graph; }
    }

    public class Graph
    {
        private int _advertiserCount = 0;
        private Dictionary<int, Node> _nodes = new Dictionary<int, Node>();
        
        public int AdvertiserCount { get { return _advertiserCount; } }
        public Dictionary<int, Node> Nodes { get { return _nodes; } }

        public void AddNode(int node, bool isAdvertiser)
        {
            if (_nodes.ContainsKey(node)) { return; }
            if (isAdvertiser) { _advertiserCount++; }
            _nodes[node] = new Node(this) { Id = node, IsAdvertiser = isAdvertiser };
        }

        public void AddEdge(int a, bool aIsAdvertiser, int b, bool bIsAdvertiser, float weight)
        {
            if (!_nodes.ContainsKey(a)) { AddNode(a, aIsAdvertiser); }
            if (!_nodes.ContainsKey(b)) { AddNode(b, bIsAdvertiser); }

            if (!_nodes[a].Edges.ContainsKey(b)) { _nodes[a].Edges[b] = new Edge() { Node = _nodes[b], Weight = weight }; }
            if (!_nodes[b].Edges.ContainsKey(a)) { _nodes[b].Edges[a] = new Edge() { Node = _nodes[a], Weight = weight }; }
        }

        public override string ToString() { return GraphDisplay.ToString(this); }
        public string ToDot(DotOptions dotOptions = DotOptions.Paper) { return GraphDisplay.ToDot(this, dotOptions); }
        public string ToCommunities() { return GraphDisplay.ToCommunities(this); }
    }
}
