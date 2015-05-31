using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intensity
{

    public class ModPermLeast : ModPerm
    {
        public ModPermLeast(Graph graph, float p, Decision decision)
            : base(graph, p, decision)
        {
        }


        protected override void MoveKeywords()
        {
            var weights = _decision == Decision.Weights;
            var highIntesity = _decision == Decision.HighIntensity;
            var lowIntensity = _decision == Decision.LowIntensity;

            foreach (var keyword in _graph.Nodes.ToList())
            {
                if (keyword.Value.IsAdvertiser) { continue; }
                var minCommunityCount = int.MaxValue;
                var minCommunity = -1;
                var communityCounts = new Dictionary<int, int>();
                foreach (var advertiserEdge in keyword.Value.Edges.ToList())
                {
                    var community = advertiserEdge.Node.Community;
                    if (communityCounts.ContainsKey(community)) { communityCounts[community]++; } else { communityCounts[community] = 1; }
                    if (communityCounts[community] < minCommunityCount)
                    {
                        minCommunityCount = communityCounts[community];
                        minCommunity = community;
                    }
                }
                keyword.Value.Community = minCommunity;
            }
        }
    }

    public class ModPermCount : ModPerm
    {
        public ModPermCount(Graph graph, float p, Decision decision) : base(graph, p, decision)
        {
        }


        protected override void MoveKeywords()
        {
            var weights = _decision == Decision.Weights;
            var highIntesity = _decision == Decision.HighIntensity;
            var lowIntensity = _decision == Decision.LowIntensity;

            foreach (var keyword in _graph.Nodes.ToList())
            {
                if (keyword.Value.IsAdvertiser) { continue; }
                var maxCommunityCount = -1;
                var maxCommunity = -1;
                var communityCounts = new Dictionary<int, int>();
                foreach (var advertiserEdge in keyword.Value.Edges.ToList())
                {
                    var community = advertiserEdge.Node.Community;
                    if (communityCounts.ContainsKey(community)) { communityCounts[community]++; } else { communityCounts[community] = 1; }
                    if (communityCounts[community] > maxCommunityCount)
                    {
                        maxCommunityCount = communityCounts[community];
                        maxCommunity = community;
                    }
                }
                keyword.Value.Community = maxCommunity;
            }
        }
    }

    public class ModPermMaxPerm : ModPerm
    {

        public ModPermMaxPerm(Graph graph, float p, Decision decision) : base(graph, p, decision)
        {
        }

        protected override void MoveKeywords()
        {
            var weights = _decision == Decision.Weights;
            var highIntesity = _decision == Decision.HighIntensity;
            var lowIntensity = _decision == Decision.LowIntensity;

            foreach (var keyword in _graph.Nodes.ToList())
            {
                if (keyword.Value.IsAdvertiser) { continue; }
                var maxPermanence = -1f;
                var maxCommunity = -1;
                var communityCounts = new Dictionary<int, int>();
                foreach (var advertiserEdge in keyword.Value.Edges.ToList())
                {
                    var perm = Score(advertiserEdge.Node);
                    var community = advertiserEdge.Node.Community;
                    if (perm > maxPermanence)
                    {
                        maxPermanence = perm;
                        maxCommunity = community;
                    }
                }
                keyword.Value.Community = maxCommunity;
            }
        }
    }


    public abstract class ModPerm
    {
        protected Graph _graph;
        float _lambda;
        protected Decision _decision;

        public event EventHandler Message;
        public void OnMessage(string message)
        {
            if (Message == null) { return; }
            Message(this, new MessageEventArgs() { Message = message });
        }

        public ModPerm(Graph graph, float lambda, Decision decision)
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
                if (nodes[i].Value.IsAdvertiser)
                {
                    nodes[i].Value.Community = i;
                }
                else
                {
                    nodes[i].Value.Community = i;
                }
            }
            MoveKeywords();
        }

        protected abstract void MoveKeywords();

        public float Run()
        {
            float sum = 0;
            float old_sum = -1;
            var iter = 0;
            var initial = true;
            var totalScore = (-1f * _graph.Count.ToFloat()).ToString();
            while (sum != old_sum && iter < 50)
            {
                iter++;
                old_sum = initial ? -1 * _graph.Count : sum; //-1 * _lambda * _graph.AdvertiserCount : sum;
                initial = false;
                sum = 0;
                var nodes = _graph.Nodes.ToList();
                var nodeCount = 0;
                totalScore = GetTotalScore();
                foreach (var v in nodes)
                {
                    nodeCount++;
                    OnMessage("running iteration=" + iter.ToString() + "\n" + "node=" + nodeCount.ToString() + "\n" + "completed=" + (100.0m * (nodeCount.ToDecimal() / nodes.Count.ToDecimal())).ToString() + "%" + "\nscore=" + totalScore.ToString());

                    if (!v.Value.IsAdvertiser) { continue; }

                    var cur_p = Score(v.Value);
                    if (cur_p.Is(1))
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
                                if (cur_p_neig >= left + n_p_neig)
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

        private string GetTotalScore()
        {
            float sum = 0f;
            foreach (var n in _graph.Nodes)
            {
                sum += Score(n.Value);
            }
            return (sum / _graph.Count.ToFloat()).ToString();
        }

        public float Score(Node v)
        {
            var calulator = new PermCalculator(v, _lambda);
            return calulator.Score();
        }
    }

    public class EdgeCount
    {
        public Edge Edge;
        public int Count;
    }

    public class PermCalculator
    {
        private Node v;
        private ListDictionary<string, EdgeCount> communityAdvertiserEdges;
        private ListDictionary<string, Edge> communityKeywordEdges;
        private float E_max_v;
        float I_v;
        float _lambda;
        float D_v;

        public PermCalculator(Node node, float lambda)
        {
            _lambda = lambda;
            v = node;
            var edges = v.Edges.ToList();
            communityAdvertiserEdges = new ListDictionary<string, EdgeCount>();
            communityKeywordEdges = new ListDictionary<string, Edge>();
            Dictionary<int, float> diffComHash = new Dictionary<int, float>();
            E_max_v = 0;
            I_v = 0;
            foreach (var keywordEdge in edges)
            {
                if (keywordEdge.Node.Community != v.Community)
                {
                    foreach (var advertiserEdge in keywordEdge.Node.Edges.ToList())
                    {
                        D_v++;
                        if (advertiserEdge.Node.Community != v.Community)
                        {
                            if (diffComHash.ContainsKey(advertiserEdge.Node.Community))
                            {
                                diffComHash[advertiserEdge.Node.Community]++;
                            }
                            else
                            {
                                diffComHash[advertiserEdge.Node.Community] = 1;
                            }
                            float maxTest = diffComHash[advertiserEdge.Node.Community];
                            if (E_max_v < maxTest) { E_max_v = maxTest; }
                        }
                    }
                }
                else
                {
                    communityKeywordEdges[keywordEdge.Node.Id] = keywordEdge;
                    foreach (var advertiserEdge in keywordEdge.Node.Edges.ToList())
                    {
                        D_v++;
                        if (advertiserEdge.Node.Community != v.Community) { continue; }
                        
                        if (communityAdvertiserEdges.ContainsKey(advertiserEdge.Node.Id))
                        {
                            communityAdvertiserEdges[advertiserEdge.Node.Id].Count++;
                        }
                        else
                        {
                            communityAdvertiserEdges[advertiserEdge.Node.Id] = new EdgeCount() { Edge = advertiserEdge, Count = 1 };
                        }

                        I_v++;
                    }
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
                    
                    var sum = 0;
                    var seenAdvertisers = new Dictionary<string, bool>();
                    foreach (var communityAdvertiserEdge in communityAdvertiserEdges.ToList())
                    {
                        sum += communityAdvertiserEdge.Count;
                    }
                    var n = communityKeywordEdges.Count.ToFloat() * communityAdvertiserEdges.Count.ToFloat();
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
                return -1;
            }

            float f1 = I_v / E_max_v;
            float f2 = 1 / D_v;
            float f3 = 1 - c_in_v;
            var retScore = (f1 * f2) - (f3);
            return retScore;
        }
    }
}

