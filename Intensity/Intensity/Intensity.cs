using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intensity
{
    public class MessageEventArgs : EventArgs { public string Message;}

    public class Intensity
    {
        Graph _graph;

        public event EventHandler Message;
        public void OnMessage(string message)
        {
            if (Message == null) { return; }
            Message(this, new MessageEventArgs() { Message = message });
        }

        public Intensity(Graph graph)
        {
            _graph = graph;
        }

        public void Init()
        {
            var nodes = _graph.Nodes.ToList();
            for (var i = 0; i < nodes.Count; i++)
            {
                nodes[i].Value.Community = i;
            }
        }

        public float Run()
        {
            float sum = 0;
            float old_sum = -1;
            var iter = 0;
            var initial = true;
            while (sum != old_sum)
            {
                iter++;
                old_sum = initial ? -1 * _graph.Nodes.Count : sum;
                initial = false;
                sum = 0;
                var nodes = _graph.Nodes.ToList();
                var nodeCount = 0;
                foreach (var v in nodes)
                {
                    nodeCount++;
                    OnMessage("running iteration=" + iter.ToString() + "\n" + "node=" + nodeCount.ToString() + "\n" + "completed=" + (100.0m * (nodeCount.ToDecimal() / nodes.Count.ToDecimal())).ToString() + "%");
                    var cur_p = Score(v.Value);
                    if (cur_p.Is(1))
                    {
                        sum += cur_p;
                        continue;
                    }

                    float cur_p_neig = 0;
                    var edges = v.Value.Edges.ToList();
                    foreach (var u in edges)
                    {
                        cur_p_neig += Score(u.Node);
                    }

                    var comOrig = v.Value.Community;
                    var notSameCommunities = new Dictionary<int, bool>();
                    var community = v.Value.Community;
                    for (var i = 0; i < edges.Count; i++)
                    {
                        notSameCommunities[edges[i].Node.Community] = true;    
                    }
                    if (notSameCommunities.ContainsKey(community)) { notSameCommunities.Remove(community); }

                    foreach (var c in notSameCommunities.Keys)
                    {
                        v.Value.Community = c;
                        float n_p = Score(v.Value);

                        float n_p_neig = 0;
                        
                        if (cur_p < n_p)
                        {
                            for (var i = 0; i < edges.Count; i++)
                            {
                                var t = edges[i];
                                n_p_neig += Score(t.Node);
                                var left = edges.Count - i - 1;
                                if (cur_p_neig >= (1 * left) + n_p_neig)
                                {
                                    i++;
                                    while (i < edges.Count)
                                    {
                                        edges[i].Node.IsDirty = true;
                                        i++;
                                    }
                                    break;
                                }
                            }
                        }

                        if (cur_p < n_p && cur_p_neig < n_p_neig)
                        {
                            cur_p = n_p;
                            comOrig = v.Value.Community;
                        }
                        else
                        {
                            v.Value.Community = comOrig;
                        }
                    }

                    sum += cur_p;
                }
            }
            float netw_intensity = sum / _graph.Nodes.ToList().Count.ToFloat();
            return netw_intensity;
        }

        public float Score(Node v)
        {
            var calulator = new Calculator(v);
            return calulator.Score();
        }
    }

    public class Calculator
    {
        private Node v;
        private ListDictionary<int, Edge> internals;
        private float E_max_v;
        float D_v;
        float I_v;

        public Calculator(Node node)
        {
            v = node;
            var edges = v.Edges.ToList();
            D_v = edges.Count;
            internals = new ListDictionary<int, Edge>();
            Dictionary<int, int> diffComHash = new Dictionary<int, int>();
            E_max_v = 0;
            foreach (var edge in edges)
            {
                if (edge.Node.Community != v.Community)
                {
                    if (diffComHash.ContainsKey(edge.Node.Community))
                    {
                        diffComHash[edge.Node.Community]++;
                    }
                    else
                    {
                        diffComHash[edge.Node.Community] = 1;
                    }
                    float count = diffComHash[edge.Node.Community];
                    if (E_max_v < count) { E_max_v = count; }
                }
                else
                {
                    internals[edge.Node.Id] = edge;
                }
            }
            I_v = internals.Count;
        }

        float _c_in_v;
        bool has_c_in_v = false;
        private float c_in_v
        {
            get
            {
                if (!has_c_in_v)
                {
                    has_c_in_v = true;
                    if (I_v < 3)
                    {
                        _c_in_v = 0;
                    }
                    else
                    {
                        var internalCount = 0;
                        foreach (var intern in internals.ToList())
                        {
                            internalCount += intern.Node.Edges.ToList().Count(e => e.Node.Community == v.Community && internals.ContainsKey(e.Node.Id));
                        }
                        _c_in_v = internalCount.ToFloat() / (I_v * (I_v - 1));
                    }
                }
                return _c_in_v;
            }
        }

        public float Score()
        {
            if (v.D_v == D_v && v.E_max_v == E_max_v && v.I_v == I_v && !v.IsDirty)
            {
                return v.Score;
            }

            v.D_v = D_v;
            v.E_max_v = E_max_v;
            v.I_v = I_v;

            if (D_v == 0 || E_max_v == 0)
            {
                var ret = c_in_v;
                v.Score = ret;
                return ret;
            }

            if (I_v == 0)
            {
                v.Score = -1;
                return -1;
            }

            float f1 = I_v / E_max_v;
            float f2 = 1 / D_v;
            float f3 = 1 - c_in_v;

            var retScore = (f1 * f2) - f3;
            v.Score = retScore;
            return retScore;
        }
    }
}
