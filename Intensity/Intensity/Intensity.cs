using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intensity
{
    public class MessageEventArgs : EventArgs { public string Message;}

    public enum Decision
    {
        Weights = 0,
        HighIntensity = 1,
        LowIntensity = 2
    }

    public class IntensityByAdvertiser : Intensity
    {
        private Graph graph;
        private float p;
        private Decision decision;

        public IntensityByAdvertiser(Graph graph, float p, Decision decision) : base(graph, p, decision)
        {
        }
        public override bool ContinueWithHeuristic(Node n)
        {
            return !n.IsAdvertiser;
        }

        public override bool ContinueWithIteration(Node n)
        {
            return n.IsAdvertiser;
        }
    }

    public class IntensityByKeyword : Intensity
    {
        public IntensityByKeyword(Graph graph, float p, Decision decision)
            : base(graph, p, decision)
        {
        }

        public override bool ContinueWithHeuristic(Node n)
        {
            return n.IsAdvertiser;
        }

        public override bool ContinueWithIteration(Node n)
        {
            return !n.IsAdvertiser;
        }
    }

    public abstract class Intensity
    {
        Graph _graph;
        float _lambda;
        Decision _decision;

        public event EventHandler Message;
        public void OnMessage(string message)
        {
            if (Message == null) { return; }
            Message(this, new MessageEventArgs() { Message = message });
        }

        public Intensity(Graph graph, float lambda, Decision decision)
        {
            _graph = graph;
            _lambda = lambda;
            _decision = decision;
        }

        public void Init()
        {
            var nodes = _graph.Nodes.ToList();
            for (var i = 0; i < nodes.Count; i++)
            {
                nodes[i].Value.Community = i;
            }
            MoveKeywords();
        }

        private void MoveKeywords()
        {
            var weights = _decision == Decision.Weights;
            var highIntesity = _decision == Decision.HighIntensity;
            var lowIntensity = _decision == Decision.LowIntensity;

            foreach (var keyword in _graph.Nodes.ToList())
            {
                if (!ContinueWithHeuristic(keyword.Value)) { continue; }
                var maxCommunityWeight = -1f;
                var maxCommunity = -1;
                var communityWeights = new Dictionary<int, float>();
                foreach (var advertiserEdge in keyword.Value.Edges.ToList())
                {
                    var weight = advertiserEdge.Weight;
                    var community = advertiserEdge.Node.Community;
                    if (communityWeights.ContainsKey(community)) { communityWeights[community] += weight; } else { communityWeights[community] = weight; }
                    if (communityWeights[community] > maxCommunityWeight)
                    {
                        maxCommunityWeight = communityWeights[community];
                        maxCommunity = community;
                    }
                }
                keyword.Value.Community = maxCommunity;
            }
        }

        public abstract bool ContinueWithIteration(Node n);
        public abstract bool ContinueWithHeuristic(Node n);

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

                    if (!ContinueWithIteration(v.Value)) { continue; }

                    var cur_p = Score(v.Value);
                    if (cur_p.Is(_lambda + 1f))
                    {
                        sum += cur_p;
                        continue;
                    }

                    float cur_p_neig = 0;
                    var edges = v.Value.Edges.ToList();
                    var advertisersToCheck = new ListDictionary<string, Node>();
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
                    foreach (var a in advertisersToCheck.ToList())
                    {
                        notSameCommunities[a.Community] = true;
                    }
                    if (notSameCommunities.ContainsKey(comOrig)) { notSameCommunities.Remove(comOrig); }

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
                                if (cur_p_neig >= ((1 + _lambda) * left) + n_p_neig)
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
                            MoveKeywords();
                        }
                        else
                        {
                            v.Value.Community = comOrig;
                        }
                    }

                    sum += cur_p;
                }
            }
            float netw_intensity = sum / _graph.AdvertiserCount.ToFloat();
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
        private ListDictionary<string, Edge> communityKeywordEdges;
        private float E_max_v;
        float I_v;
        float _lambda;

        public Calculator(Node node, float lambda)
        {
            _lambda = lambda;
            v = node;
            var edges = v.Edges.ToList();
            communityKeywordEdges = new ListDictionary<string, Edge>();
            Dictionary<int, float> diffComHash = new Dictionary<int, float>();
            E_max_v = 0;
            I_v = 0;
            foreach (var keywordEdge in edges)
            {
                if (keywordEdge.Node.Community != v.Community)
                {
                    if (diffComHash.ContainsKey(keywordEdge.Node.Community))
                    {
                        diffComHash[keywordEdge.Node.Community] += keywordEdge.Weight;
                    }
                    else
                    {
                        diffComHash[keywordEdge.Node.Community] = keywordEdge.Weight;
                    }
                    float maxTest = diffComHash[keywordEdge.Node.Community];
                    if (E_max_v < maxTest) { E_max_v = maxTest; }
                }
                else
                {
                    communityKeywordEdges[keywordEdge.Node.Id] = keywordEdge;
                    I_v += keywordEdge.Weight;
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

                    var sum = 0f;
                    var seenAdvertisers = new Dictionary<string, bool>();
                    foreach (var communityKeywordEdge in communityKeywordEdges.ToList())
                    {
                        var keyword = communityKeywordEdge.Node;
                        var attachedAdvertisers = keyword.Edges.ToList();

                        foreach (var attachedAdvertiser in attachedAdvertisers)
                        {
                            if (attachedAdvertiser.Node.Community == v.Community)
                            {
                                sum += attachedAdvertiser.Weight;
                                seenAdvertisers[attachedAdvertiser.Node.Id] = true;
                            }
                        }
                    }
                    var n = seenAdvertisers.Count.ToFloat();
                    _c_in_v = n.Is(0) ? 0 : sum / n;
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

            float cc = c_in_v;
            var retScore = cc + _lambda * (I_v - E_max_v);
            return retScore;
        }
    }
}
