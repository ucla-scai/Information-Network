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
        float _lambda;

        public event EventHandler Message;
        public void OnMessage(string message)
        {
            if (Message == null) { return; }
            Message(this, new MessageEventArgs() { Message = message });
        }

        public Intensity(Graph graph, float lambda)
        {
            _graph = graph;
            _lambda = lambda;
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
                old_sum = initial ? -1 * _lambda * _graph.AdvertiserCount : sum;
                initial = false;
                sum = 0;
                var nodes = _graph.Nodes.ToList();
                var nodeCount = 0;
                foreach (var v in nodes)
                {
                    nodeCount++;
                    OnMessage("running iteration=" + iter.ToString() + "\n" + "node=" + nodeCount.ToString() + "\n" + "completed=" + (100.0m * (nodeCount.ToDecimal() / nodes.Count.ToDecimal())).ToString() + "%");

                    if (!v.Value.IsAdvertiser) { continue; }

                    var cur_p = Score(v.Value);
                    if (cur_p.Is(_lambda + 1f))
                    {
                        sum += cur_p;
                        continue;
                    }

                    float cur_p_neig = 0;
                    var edges = v.Value.Edges.ToList();
                    var advertisersToCheck = new ListDictionary<int, Node>();
                    foreach (var k in edges)
                    {
                        var keyword = k.Node;
                        foreach (var a in keyword.Edges.ToList())
                        {
                            advertisersToCheck[a.Node.Id] = a.Node;
                        }
                        
                    }

                    foreach (var p in advertisersToCheck.ToList())
                    {
                        cur_p_neig += Score(p);
                    }

                    var comOrig = v.Value.Community;
                    var notSameCommunities = new Dictionary<int, bool>();
                    var community = v.Value.Community;
                    for (var i = 0; i < edges.Count; i++)
                    {
                        var keyword = edges[i].Node;
                        foreach (var a in keyword.Edges.ToList())
                        {
                            notSameCommunities[a.Node.Community] = true;
                        }
                    }
                    if (notSameCommunities.ContainsKey(community)) { notSameCommunities.Remove(community); }

                    foreach (var c in notSameCommunities.Keys)
                    {
                        v.Value.Community = c;
                        float n_p = Score(v.Value);

                        float n_p_neig = 0;
                        
                        if (cur_p < n_p)
                        {
                            var advertisersToCheckList = advertisersToCheck.ToList();
                            for (var i = 0; i < advertisersToCheckList.Count; i++)
                            {
                                var t = advertisersToCheckList[i];
                                n_p_neig += Score(t);
                                var left = advertisersToCheckList.Count - i - 1;
                                if (cur_p_neig >= ((1+ _lambda) * left) + n_p_neig)
                                {
                                    i++;
                                    while (i < advertisersToCheckList.Count)
                                    {
                                        advertisersToCheckList[i].IsDirty = true;
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
            var calulator = new Calculator(v, _lambda);
            return calulator.Score();
        }
    }

    public class Calculator
    {
        private Node v;
        private ListDictionary<int, Edge> internals;
        private float E_max_v;
        float I_v;
        float _lambda;

        public Calculator(Node node, float lambda)
        {
            _lambda = lambda;
            v = node;
            var edges = v.Edges.ToList();
            internals = new ListDictionary<int, Edge>();
            Dictionary<int, float> diffComHash = new Dictionary<int, float>();
            E_max_v = 0;
            I_v = 0;
            foreach (var edge in edges)
            {
                if (edge.Node.Community != v.Community)
                {
                    if (diffComHash.ContainsKey(edge.Node.Community))
                    {
                        diffComHash[edge.Node.Community]+=edge.Weight;
                    }
                    else
                    {
                        diffComHash[edge.Node.Community] = edge.Weight;
                    }
                    float maxTest = diffComHash[edge.Node.Community];
                    if (E_max_v < maxTest) { E_max_v = maxTest; }
                }
                else
                {
                    internals[edge.Node.Id] = edge;
                    I_v += edge.Weight;
                }
            }
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
                    
                    var n = 0f;
                    var sum = 0f;
                    foreach (var intern in internals.ToList())
                    {
                        var keyword = intern.Node;
                        var attachedAdvertisers = keyword.Edges.ToList();
                            
                        foreach (var attachedAdvertiser in attachedAdvertisers)
                        {
                            if (attachedAdvertiser.Node.Community == v.Community)
                            {
                                sum += attachedAdvertiser.Weight;
                                n++;
                            }
                        }
                    }
                    _c_in_v = n == 0 ? 0 : sum / n;
                }
                return _c_in_v;
            }
        }

        public float Score()
        {
            if (E_max_v == 0)
            {
                var ret = c_in_v;
                return ret;
            }

            if (I_v == 0)
            {
                return -1 * _lambda;
            }

            float cc = _c_in_v;
            var retScore = cc + _lambda * (I_v - E_max_v);
            v.Score = retScore;
            return retScore;
        }
    }
}
