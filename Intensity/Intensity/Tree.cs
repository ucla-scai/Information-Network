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

                foreach (var node in _graph.Nodes)
                {
                    if (!seen.ContainsKey(node.Value.Id)) { root = node.Value; }
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
            while (q.Count > 0)
            {
                var node = q.Dequeue();
                _depth = node.Level;
                _count++;
                _flat[node.Id] = node.Node;
                node.Children = new List<TreeNode>();
                foreach(var childEdge in node.Node.Edges.ToList())
                {
                    var childNode = childEdge.Node;
                    if (_flat.ContainsKey(childNode.Id)) { continue; }
                    var treeNode = new TreeNode() { Node = childNode, Level = node.Level+1 };
                    node.Children.Add(treeNode);
                    q.Enqueue(treeNode);
                }
            }

            return this;
        }
    }
}
