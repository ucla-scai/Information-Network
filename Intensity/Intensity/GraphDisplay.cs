using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intensity
{
    public enum DotOptions : uint
    {
        BoxAndCircles = 0x01,
        SameColor = 0x02,
        RedGray = 0x04,
        Small = 0x08,
        NodeLabels = 0x10,
        EdgeLabels = 0x20,
        CommunityLabels = 0x40,
        NoCommunities = 0x80,
        NoOveralp = 0x100,
        EdgesFirst = 0x200,
        TransparentEdges = 0x400,
        Labels = (NodeLabels | EdgeLabels | CommunityLabels),
        Pdf = (BoxAndCircles  | RedGray | Labels | NoOveralp),
        Paper = (RedGray | Small | NoCommunities)
    }

    public class GraphDisplay
    {
        public static string ToString(Graph g)
        {
            var str = "";
            foreach (var node in g.Nodes.ToList())
            {
                foreach (var edge in node.Value.Edges.ToList())
                {
                    str += "\n" + "c=" + node.Value.Community.ToString() + "," + node.Value.Id.ToString() + "-->" + edge.Node.Id.ToString() + "," + "c=" + edge.Node.Community.ToString();
                }
            }
            return str;
        }

        public static string ToDot(Graph g, DotOptions dotOptions)
        {
            var dot = new StringBuilder();
            Dictionary<string, bool> seen = new Dictionary<string, bool>();
            if (dotOptions.HasFlag(DotOptions.NoOveralp)) { dot.Append("graph G {\noverlap = false;\n"); } else { dot.Append("graph G {\n"); }
            if (dotOptions.HasFlag(DotOptions.EdgesFirst)) { dot.Append("outputorder=edgesfirst;\n"); }
            var str = dotOptions.HasFlag(DotOptions.Small) ? "node[margin=\"0.06,0,025\" width=0.2 height=0.15 style=filled, fontsize=6, fontname=\"Helvetica\", colorscheme=greens3, color=1];\n" : string.Empty;
            dot.Append(str);
            var cluster = 0;
            var communities = g.Nodes.Values.ToList().Select(s => s.Community).Distinct().ToList();

            if (!dotOptions.HasFlag(DotOptions.NoCommunities))
            {
                foreach (var community in communities)
                {
                    dot.Append("subgraph cluster" + cluster.ToString() + " {\n");
                    foreach (var node in g.Nodes)
                    {
                        if (node.Value.Community != community) { continue; }

                        foreach (var edge in node.Value.Edges.ToList().Where(e => e.Node.Community == node.Value.Community))
                        {
                            var nodeEdge = node.Value.Id.ToString() + " -- " + edge.Node.Id.ToString();
                            var reverse = edge.Node.Id.ToString() + " -- " + node.Value.Id.ToString();
                            if (seen.ContainsKey(nodeEdge) || seen.ContainsKey(reverse)) { continue; }
                            str = dotOptions.HasFlag(DotOptions.EdgeLabels) ? nodeEdge + " [ label=\"" + edge.Weight.ToString() + "\" ];\n" : nodeEdge + " ;\n";
                            dot.Append(str);
                            seen[nodeEdge] = true;
                        }
                    }
                    str = dotOptions.HasFlag(DotOptions.CommunityLabels) ? "label = \"" + community.ToString() + "\"\n" : string.Empty;
                    dot.Append(str);
                    dot.Append("style=dashed\n");
                    dot.Append("}\n");
                    cluster++;
                }
            }

            foreach (var node in g.Nodes)
            {
                foreach (var edge in node.Value.Edges.ToList())
                {
                    var nodeEdge = node.Value.Id.ToString() + " -- " + edge.Node.Id.ToString();
                    var reverse = edge.Node.Id.ToString() + " -- " + node.Value.Id.ToString();
                    if (seen.ContainsKey(nodeEdge) || seen.ContainsKey(reverse)) { continue; }
                    var extra = dotOptions.HasFlag(DotOptions.TransparentEdges) ? ",color=\"#0000005f\"" : "";
                    str = dotOptions.HasFlag(DotOptions.EdgeLabels) ? nodeEdge + " [ label=\"" + edge.Weight.ToString() + "\"" + extra + " ];\n" : dotOptions.HasFlag(DotOptions.TransparentEdges) ? nodeEdge + " [ color=\"#0000005f\" ];\n" : nodeEdge + " ;\n";
                    dot.Append(str);
                    seen[nodeEdge] = true;
                }
            }

            foreach (var node in g.Nodes)
            {
                var shape = string.Empty;
                shape = node.Value.IsAdvertiser ? "ellipse,style=filled,color=firebrick,fontcolor=white" : shape;
                shape = !node.Value.IsAdvertiser && dotOptions.HasFlag(DotOptions.RedGray) && dotOptions.HasFlag(DotOptions.BoxAndCircles) ? "box,style=filled,color=grey,fontcolor=white" : shape;
                shape = !node.Value.IsAdvertiser && dotOptions.HasFlag(DotOptions.SameColor) && dotOptions.HasFlag(DotOptions.BoxAndCircles) ? "box,style=filled,color=firebrick,fontcolor=white" : shape;
                shape = !node.Value.IsAdvertiser && dotOptions.HasFlag(DotOptions.RedGray) && !dotOptions.HasFlag(DotOptions.BoxAndCircles) ? "ellipse,style=filled,color=grey,fontcolor=white" : shape;
                shape = !node.Value.IsAdvertiser && dotOptions.HasFlag(DotOptions.SameColor) && !dotOptions.HasFlag(DotOptions.BoxAndCircles) ? "ellipse,style=filled,color=firebrick,fontcolor=white" : shape;
                str = dotOptions.HasFlag(DotOptions.NodeLabels) ? node.Value.Id.ToString() + " [shape=" + shape + "];\n" : node.Value.Id.ToString() + " [label=\"\",shape=" + shape + "];\n";
                dot.Append(str);
            }
            dot.Append("}");
            return dot.ToString();
        }

        public static string ToCommunities(Graph g)
        {
            Dictionary<int, Dictionary<string, Node>> communityIds = new Dictionary<int, Dictionary<string, Node>>();
            StringBuilder str = new StringBuilder();

            foreach (var node in g.Nodes)
            {
                if (!communityIds.ContainsKey(node.Value.Community)) { communityIds[node.Value.Community] = new Dictionary<string, Node>(); }
                communityIds[node.Value.Community][node.Value.Id] = node.Value;
            }
            var communities = communityIds.Count;
            var len = communities.ToString().Length;
            var currentCom = 1;

            foreach(var communityId in communityIds.OrderByDescending(o=>o.Value.Count))
            {
                var curLen = currentCom.ToString().Length;
                str.Append("[".PadRight(len - curLen+1, ' ') + currentCom.ToString() + "] ");
                List<string> idStrs = new List<string>();
                foreach (var ids in communityId.Value)
                {
                    if (ids.Value.IsAdvertiser) { idStrs.Add("A" + ids.Value.Id.ToString()); } else { idStrs.Add("K" + ids.Value.Id.ToString()); }
                }
                idStrs = idStrs.OrderByDescending(s => s).ToList();
                var global = 0;
                var inner = new StringBuilder();
                while (global < idStrs.Count)
                {
                    for (var i = 0; i < 7 && global + i < idStrs.Count; i++)
                    {
                        inner.Append(idStrs[global + i] + " ");
                    }
                    inner.Append("\n".PadRight(len+4, ' '));
                    global += 7;
                }
                str.Append(inner.ToString().Trim() + "\n");
                currentCom++;
            }
            return str.ToString().Trim();
        }
    }
}
