using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intensity
{
    public class BFS
    {
        Graph _graph;

        public BFS(Graph graph)
        {
            _graph = graph;
        }

        public List<Tree> Forest()
        {
            var forest = new List<Tree>();
            var seen = new Dictionary<string, bool>();

            while (true)
            {
                Node root = null;
                string min = null;

                foreach (var node in _graph.Nodes)
                {
                    if (!seen.ContainsKey(node.Value.Id)) 
                    {
                        if (root == null)
                        {
                            root = node.Value;
                            min = node.Value.Id;
                        }
                        else if(node.Value.Id.CompareTo(min) < 0)
                        {
                            min = node.Value.Id;
                            root = node.Value;
                        }
                    }
                }

                if (root == null)
                {
                    break;
                }

                var treeRoot = new TreeNode() { Node = root, Level = 1 };
                var tree = new Tree(treeRoot);
                tree.FromBFS();
                tree.Flat.ForEach(t => seen[t.Id] = true);
                forest.Add(tree);
            }

            return forest;
        }
    }

    public class TreeNode
    {
        public List<TreeNode> Children { get; set; }
        public int Level { get; set; }
        public Node Node { get; set; }
        public string Id { get { return Node.Id; } }
    }

    public class Tree
    {
        public int Metric { get { return Count * Depth; } }

        public bool Same(Tree t)
        {
            if (t._flat.Count != _flat.Count) { return false; }
            if (t.Count != Count) { return false; }
            if (t.Depth != Depth) { return false; }

            foreach (var pair in _flat.ToList())
            {
                if (!t._flat.ContainsKey(pair.Id)) { return false; }
            }
            return true;
        }

        private TreeNode _treeRoot;
        private int _depth = 0;
        private int _count = 0;
        private ListDictionary<string, Node> _flat = new ListDictionary<string, Node>();

        public TreeNode Root { get { return _treeRoot; } }
        public int Depth { get { return _depth; } }
        public int Count { get { return _count; } }
        public List<Node> Flat { get { return _flat.ToList(); } }

        public Tree(TreeNode treeRoot)
        {
            _treeRoot = treeRoot;
        }

        public Tree FromBFS()
        {
            Queue<TreeNode> q = new Queue<TreeNode>();
            q.Enqueue(_treeRoot);
            _flat[_treeRoot.Id] = _treeRoot.Node;
            while (q.Count > 0)
            {
                var node = q.Dequeue();
                _depth = node.Level;
                _count++;
                node.Children = new List<TreeNode>();
                foreach(var childEdge in node.Node.Edges.ToList())
                {
                    var childNode = childEdge.Node;
                    if (_flat.ContainsKey(childNode.Id)) { continue; }
                    var treeNode = new TreeNode() { Node = childNode, Level = node.Level+1 };
                    node.Children.Add(treeNode);
                    q.Enqueue(treeNode);
                    _flat[childNode.Id] = childNode;
                }
            }

            return this;
        }
    }
}
