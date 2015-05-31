using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;
using System.Diagnostics;

namespace Intensity
{
    public enum ParserOptions : uint
    {
        None = 0x0,
        SpaceDelimited = 0x1
    }

    public class Parser
    {
        ParserOptions _options = ParserOptions.None;
        public Parser() { }
        public Parser(ParserOptions options) { _options = options; }

        private class KeywordInfo
        {
            public int N = 0;
            public int Sum = 0;
            public Dictionary<int, bool> Advertisers = new Dictionary<int, bool>();
        }

        private class AdvertiserInfo
        {
            public int N = 0;
            public int Sum = 0;
            public Dictionary<int, bool> Keywords = new Dictionary<int, bool>();
        }

        private class AdvertiserKeyword
        {
            public int Advertiser;
            public int Id;
            public int Position;
            public int Consumption;
        }

        public void ToFile(string advertiserFile, string keywordFile, string output, int sectors, int sectorFilter = -1)
        {
            var sectorAdvertisers = new Dictionary<int, Dictionary<int, bool>>();

            using (var reader = new StreamReader(advertiserFile))
            {
                var line = reader.ReadLine();
                while (line != null)
                {
                    var split = line.Split('\t');
                    var id_sector = split.First();
                    var id = int.Parse(id_sector.Split('_').First());
                    var sector = int.Parse(id_sector.Split('_').Last());
                    var showPositions = int.Parse(split[1]);
                    var date = DateTime.ParseExact(split[2], "yyyyMMdd", CultureInfo.InvariantCulture);
                    var impressions = int.Parse(split[3]);
                    var clicks = int.Parse(split[4]);
                    var consumption = int.Parse(split[5]);
                    var biddingPrice = float.Parse(split[6]);
                    var quality = float.Parse(split[7]);
                    var clickPrice = float.Parse(split[8]);
                    var rank = float.Parse(split[9]);
                    var asn = float.Parse(split[10]);
                    if (!sectorAdvertisers.ContainsKey(sector)) { sectorAdvertisers[sector] = new Dictionary<int, bool>(); }
                    sectorAdvertisers[sector][id] = true;
                    line = reader.ReadLine();
                }
            }

            var luckySectors = sectorFilter == -1 ? sectorAdvertisers.Keys.ToList().OrderBy(c => Guid.NewGuid()).ToList().Take(sectors).ToList() : new List<int>(new[] { sectorFilter });
            var luckyAdvertisers = new Dictionary<int, bool>();
            foreach (var luckySector in luckySectors)
            {
                foreach (var advertiser in sectorAdvertisers[luckySector].Keys)
                {
                    luckyAdvertisers[advertiser] = true;
                }
            }

            Dictionary<int, KeywordInfo> keywords = new Dictionary<int, KeywordInfo>();
            Dictionary<string, AdvertiserKeyword> advertiserKeywords = new Dictionary<string, AdvertiserKeyword>();
            using (var reader = new StreamReader(keywordFile))
            {
                var line = reader.ReadLine();
                while (line != null)
                {
                    var split = line.Split('\t');
                    var advertiser = int.Parse(split.First());
                    if (luckyAdvertisers.ContainsKey(advertiser))
                    {
                        var id = int.Parse(split[1]);
                        var position = int.Parse(split[2]);
                        var consumption = int.Parse(split[3]);
                        var advertiserKeyword = new AdvertiserKeyword() { Advertiser = advertiser, Consumption = consumption, Id = id, Position = position };
                        var adKeyString = advertiser.ToString() + "_" + id.ToString();
                        if (!advertiserKeywords.ContainsKey(adKeyString))
                        {
                            advertiserKeywords[adKeyString] = advertiserKeyword;
                        }
                        else
                        {
                            advertiserKeywords[adKeyString].Consumption += consumption;
                        }

                        if (!keywords.ContainsKey(id)) { keywords[id] = new KeywordInfo(); }
                        keywords[id].N++;
                        keywords[id].Sum += consumption;
                        keywords[id].Advertisers[advertiser] = true;
                    }
                    line = reader.ReadLine();
                }
            }

            var lines = new List<string>();

            foreach (var key in keywords.Keys)
            {
                var keywordInfo = keywords[key];
                if (keywordInfo.Sum == 0) { continue; }
                foreach (var advertiser in keywordInfo.Advertisers.Keys)
                {
                    var adKeyString = advertiser.ToString() + "_" + key.ToString();
                    var advertiserKeyword = advertiserKeywords[adKeyString];
                    var weight = advertiserKeyword.Consumption.ToDecimal() / keywordInfo.Sum.ToDecimal();
                    var writeLine = string.Format("{0}\t{1}\t{2}\t{3}\t{4}", advertiser, key, weight, advertiserKeyword.Consumption, keywordInfo.Sum);

                    if (weight > 0)
                    {
                        lines.Add(writeLine);
                    }
                }
            }

            lines = Filter(lines);

            using (var writer = new StreamWriter(output))
            {
                foreach (var line in lines)
                {
                    writer.WriteLine(line);
                }
            }
        }

        public List<string> ToFileList(List<string> advertiserFile, string keywordFile, int sectors, int sectorFilter = -1, bool byKeyword = true)
        {
            var sectorAdvertisers = new Dictionary<int, Dictionary<int, bool>>();

            foreach(var line in advertiserFile)
            {
                var split = line.Split('\t');
                var id_sector = split.First();
                var id = int.Parse(id_sector.Split('_').First());
                var sector = int.Parse(id_sector.Split('_').Last());
                var showPositions = int.Parse(split[1]);
                var date = DateTime.ParseExact(split[2], "yyyyMMdd", CultureInfo.InvariantCulture);
                var impressions = int.Parse(split[3]);
                var clicks = int.Parse(split[4]);
                var consumption = int.Parse(split[5]);
                var biddingPrice = float.Parse(split[6]);
                var quality = float.Parse(split[7]);
                var clickPrice = float.Parse(split[8]);
                var rank = float.Parse(split[9]);
                var asn = float.Parse(split[10]);
                if (!sectorAdvertisers.ContainsKey(sector)) { sectorAdvertisers[sector] = new Dictionary<int, bool>(); }
                sectorAdvertisers[sector][id] = true;
            }

            var luckySectors = sectorFilter == -1 ? sectorAdvertisers.Keys.ToList().OrderBy(c => Guid.NewGuid()).ToList().Take(sectors).ToList() : new List<int>(new[] { sectorFilter });
            var luckyAdvertisers = new Dictionary<int, bool>();
            foreach (var luckySector in luckySectors)
            {
                foreach (var advertiser in sectorAdvertisers[luckySector].Keys)
                {
                    luckyAdvertisers[advertiser] = true;
                }
            }

            Dictionary<int, KeywordInfo> keywords = new Dictionary<int, KeywordInfo>();
            Dictionary<string, AdvertiserKeyword> advertiserKeywords = new Dictionary<string, AdvertiserKeyword>();

            Dictionary<int, AdvertiserInfo> advertisers = new Dictionary<int, AdvertiserInfo>();

            using (var reader = new StreamReader(keywordFile))
            {
                var line = reader.ReadLine();
                while (line != null)
                {
                    var split = line.Split('\t');
                    var advertiser = int.Parse(split.First());
                    if (luckyAdvertisers.ContainsKey(advertiser))
                    {
                        var id = int.Parse(split[1]);
                        var position = int.Parse(split[2]);
                        var consumption = int.Parse(split[3]);
                        var advertiserKeyword = new AdvertiserKeyword() { Advertiser = advertiser, Consumption = consumption, Id = id, Position = position };
                        var adKeyString = advertiser.ToString() + "_" + id.ToString();
                        if (!advertiserKeywords.ContainsKey(adKeyString))
                        {
                            advertiserKeywords[adKeyString] = advertiserKeyword;
                        }
                        else
                        {
                            advertiserKeywords[adKeyString].Consumption += consumption;
                        }

                        if (!advertisers.ContainsKey(advertiser)) { advertisers[advertiser] = new AdvertiserInfo(); }
                        advertisers[advertiser].N++;
                        advertisers[advertiser].Sum += consumption;
                        advertisers[advertiser].Keywords[id] = true;

                        if (!keywords.ContainsKey(id)) { keywords[id] = new KeywordInfo(); }
                        keywords[id].N++;
                        keywords[id].Sum += consumption;
                        keywords[id].Advertisers[advertiser] = true;
                    }
                    line = reader.ReadLine();
                }
            }

            var lines = new List<string>();

            foreach (var key in keywords.Keys)
            {
                var keywordInfo = keywords[key];
                if (keywordInfo.Sum == 0) { continue; }
                foreach (var advertiser in keywordInfo.Advertisers.Keys)
                {
                    var adKeyString = advertiser.ToString() + "_" + key.ToString();
                    var advertiserKeyword = advertiserKeywords[adKeyString];
                    var weight = advertiserKeyword.Consumption.ToDecimal() / keywordInfo.Sum.ToDecimal();
                    var writeLine = string.Format("{0}\t{1}\t{2}\t{3}\t{4}", advertiser, key, weight, advertiserKeyword.Consumption, keywordInfo.Sum);

                    if (weight > 0 && byKeyword)
                    {
                        lines.Add(writeLine);
                    }
                }
            }

            foreach (var key in advertisers.Keys)
            {
                var advertiserInfo = advertisers[key];
                if (advertiserInfo.Sum == 0) { continue; }
                foreach (var keyword in advertiserInfo.Keywords.Keys)
                {
                    var adKeyString = key.ToString() + "_" + keyword.ToString();
                    var advertiserKeyword = advertiserKeywords[adKeyString];
                    var weight = advertiserKeyword.Consumption.ToDecimal() / advertiserInfo.Sum.ToDecimal();
                    var writeLine = string.Format("{0}\t{1}\t{2}\t{3}\t{4}", key, keyword, weight, advertiserKeyword.Consumption, advertiserInfo.Sum);

                    if (weight > 0 && !byKeyword)
                    {
                        lines.Add(writeLine);
                    }
                }
            }

            return lines;
        }

        private List<string> Filter(List<string> lines)
        {
            var filtered = new List<string>();
            var graph = FromFile(lines);
            var bfs = new BFS(graph);
            var trees = bfs.Forest();

            var toTake = Math.Max((trees.Count.ToFloat() * 0.1f).ToInt(), 1);
            var ordered = trees.OrderByDescending(o => o.Depth * o.Count).ToList();
            ordered.RemoveAll(o => o.Depth * o.Count < 200);
            var list = ordered.Take(toTake).ToList();
            var takeHash = new Dictionary<string, bool>();

            foreach (var tree in list)
            {
                foreach (var node in tree.Flat)
                {
                    if (!node.IsAdvertiser) { takeHash[node.Id] = true; }
                }
            }

            foreach (var line in lines)
            {
                var split = line.Split('\t');
                var advertiser = split[0];
                var keyword = split[1];
                if (takeHash.ContainsKey(keyword))
                {
                    filtered.Add(line);
                }
            }

            return filtered;
        }

        public static bool Same(List<string> a, List<string> b)
        {
            Dictionary<string, bool> aDic = new Dictionary<string, bool>();
            foreach (var line in a)
            {
                var split = line.Split('\t');
                var adv = split[0];
                var key = split[1];
                aDic[adv + "-" + key] = true;
            }

            Dictionary<string, bool> bDic = new Dictionary<string, bool>();
            foreach (var line in b)
            {
                var split = line.Split('\t');
                var adv = split[0];
                var key = split[1];
                bDic[adv + "-" + key] = true;
            }

            var ret = true;

            foreach (var pair in aDic)
            {
                if (!(bDic.ContainsKey(pair.Key))) { 
                    Debug.WriteLine("Mismatch Found in a not in b:");
                    Debug.WriteLine(pair.Key);
                    ret = false;
                }
            }

            foreach (var pair in bDic)
            {
                if (!(aDic.ContainsKey(pair.Key)))
                {
                    Debug.WriteLine("Mismatch Found in b not in a:");
                    Debug.WriteLine(pair.Key);
                    ret = false;
                }
            }

            if (aDic.Count != bDic.Count) { ret = false; }

            return ret;
        }

        private bool SameForest(List<Tree> a, List<Tree> b)
        {
            foreach (var t in a)
            {
                var found = false;
                foreach (var s in b)
                {
                    if (s.Same(t)) { found = true; break; }
                }
                if (!found) { return false; }
            }
            return true;
        }

        private List<string> FilterStrict(List<string> lines, List<string> lines2, bool byKeyword, ref List<string> linesRet2)
        {
            var filtered = new List<string>();
            var filtered2 = new List<string>();

            var strict = Strict(lines);
            var strict2 = Strict(lines2);

            var same = Same(strict, strict2);

            var graph = FromFile(strict);
            var graph2 = FromFile(strict2);

            var same2 = Same(graph.ToFile(), graph2.ToFile());

            var bfs = new BFS(graph);
            var trees = bfs.Forest();

            var bfs2 = new BFS(graph2);
            var trees2 = bfs2.Forest();

            var same3 = SameForest(trees, trees2);

            var sum1 = trees.Sum(s => s.Metric);
            var sum2 = trees2.Sum(s => s.Metric);

            var sum3 = trees.Sum(s => s.Count);
            var sum4 = trees2.Sum(s => s.Count);

            var toTake = Math.Max((trees.Count.ToFloat() * 0.1f).ToInt(), 1);
            var ordered = trees.OrderByDescending(o => o.Depth * o.Count).ToList();
            var cutOff = ordered[toTake - 1].Count * ordered[toTake - 1].Depth;
            var b4_1 = new List<Tree>(ordered.ToArray());
            ordered.RemoveAll(o => o.Depth * o.Count < 200);
            var list = ordered.Where(w=>w.Depth*w.Count >= cutOff).ToList();

            var ordered2 = trees2.OrderByDescending(o => o.Depth * o.Count).ToList();
            var cutOff2 = ordered2[toTake - 1].Count * ordered2[toTake - 1].Depth;
            var b4_2 = new List<Tree>(ordered2.ToArray());
            ordered2.RemoveAll(o => o.Depth * o.Count < 200);
            var list2 = ordered2.Where(w => w.Depth * w.Count >= cutOff2).ToList();

            var same4 = SameForest(list, list2);

            var takeHash = new Dictionary<string, bool>();

            foreach (var tree in list)
            {
                foreach (var node in tree.Flat)
                {
                    if (!node.IsAdvertiser) { takeHash[node.Id] = true; }
                }
            }

            var takeHash2 = new Dictionary<string, bool>();

            foreach (var tree in list2)
            {
                foreach (var node in tree.Flat)
                {
                    if (!node.IsAdvertiser) { takeHash2[node.Id] = true; }
                }
            }

            foreach (var line in lines)
            {
                var split = line.Split('\t');
                var advertiser = split[0];
                var keyword = split[1];
                if (takeHash.ContainsKey(keyword))
                {
                    filtered.Add(line);
                }
            }

            foreach (var line in lines2)
            {
                var split = line.Split('\t');
                var advertiser = split[0];
                var keyword = split[1];
                if (takeHash2.ContainsKey(keyword))
                {
                    filtered2.Add(line);
                }
            }

            var same5 = Same(filtered, filtered2);

            var same6 = Same(Normalize(Strict(filtered)), Normalize(Strict(filtered2), !byKeyword));

            linesRet2 = Normalize(Strict(filtered2), !byKeyword);

            return Normalize(Strict(filtered), byKeyword);
        }

        private List<string> Normalize(List<string> list, bool byKeyword = true)
        {
            var ret = new List<string>();
            Dictionary<string, int> keywordNorm = new Dictionary<string,int>();
            Dictionary<string, int> advertiserNorm = new Dictionary<string, int>();

            foreach (var line in list)
            {
                var split = line.Split('\t');
                var advertiser = split[0];
                var keyword = split[1];
                var weight = int.Parse(split[3]);
                if (keywordNorm.ContainsKey(keyword)) { keywordNorm[keyword] += weight; } else { keywordNorm[keyword] = weight; }
                if (advertiserNorm.ContainsKey(advertiser)) { advertiserNorm[advertiser] += weight; } else { advertiserNorm[advertiser] = weight; }
            }

            foreach (var line in list)
            {
                var split = line.Split('\t');
                var advertiser = split[0];
                var keyword = split[1];
                var frac = split[2];
                var weight = int.Parse(split[3]);
                var tot = byKeyword ? keywordNorm[keyword].ToString() : advertiserNorm[advertiser].ToString();
                var writeLine = string.Format("{0}\t{1}\t{2}\t{3}\t{4}", advertiser, keyword, frac, weight, tot);
                ret.Add(writeLine);
            }

            return ret;
        }

        private List<string> Strict(List<string> lines)
        {
            var strict = new List<string>();

            var keywordAdvertisers = new Dictionary<string, Dictionary<string, bool>>();

            var nonZero = new List<string>();

            foreach (var line in lines)
            {
                var split = line.Split('\t');
                var advertiser = split[0];
                var keyword = split[1];
                var weight = float.Parse(split[2]);
                if (weight > 0)
                {
                    nonZero.Add(line);
                }
            }

            foreach (var line in nonZero)
            {
                var split = line.Split('\t');
                var advertiser = split[0];
                var keyword = split[1];
                if (!keywordAdvertisers.ContainsKey(keyword)) { keywordAdvertisers[keyword] = new Dictionary<string,bool>(); }
                keywordAdvertisers[keyword][advertiser] = true;
            }

            foreach (var line in nonZero)
            {
                var split = line.Split('\t');
                var advertiser = split[0];
                var keyword = split[1];
                if (keywordAdvertisers[keyword].Count > 1)
                {
                    strict.Add(line);
                }
            }

            return strict;
        }

        private Graph FromFile(List<string> lines)
        {
            var graph = new Graph();
            foreach (var line in lines)
            {
                Add(line, graph);
            }
            return graph;
        }

        private void Add(string line, Graph graph)
        {
            var split = _options.HasFlag(ParserOptions.SpaceDelimited) ? line.Split(' ') : line.Split('\t');
            var advertiser = split.First();

            var keyword = split[1];
            
            if (advertiser == "5316051" && keyword == "38096")
            {
                Console.Write("");
            }
            
            var weight = split.Length > 2 ? float.Parse(split[2]) : 1;
            if (weight > 0)
            {
                graph.AddLine(line);
                graph.AddEdge(advertiser, true, keyword, false, weight);
            }
        }

        public void Clean(string input, string output)
        {
            var lines = new List<string>();
            using (var reader = new StreamReader(input))
            {
                var line = reader.ReadLine();
                while (line != null)
                {
                    var split = line.Split('\t');
                    var advertiser = int.Parse(split[0]);
                    var keyword = int.Parse(split[1]);
                    var weight = float.Parse(split[2]);
                    if (!weight.Is(0))
                    {
                        lines.Add(line);
                    }
                    line = reader.ReadLine();
                }
            }

            using (var writer = new StreamWriter(output))
            {
                foreach (var line in lines)
                {
                    writer.WriteLine(line);
                }
            }
        }

        private bool Has(List<string> lines, string id)
        {
            foreach (var line in lines)
            {
                var split = line.Split('\t');
                var adv = split[0];
                if (adv == id) { return true; }
            }
            return false;
        }

        public Tuple<Graph, Graph> FromFile(List<string> input, List<string> input2, bool filter, bool filterStrict, bool byKeyword = true)
        {
            var graph = new Graph();
            var graph2 = new Graph();
            var lines = new List<string>();
            var lines2 = new List<string>();
            lines = FilterStrict(input, input2, byKeyword, ref lines2);
            var same = Same(lines, lines2);
            lines.ForEach(f => Add(f, graph));
            lines2.ForEach(f => Add(f, graph2));

            var same2 = Same(graph.ToFile(), graph2.ToFile());

            var ret = Tuple.Create(graph, graph2);

            return ret;
        }

        public Graph FromFile(string input, bool filter, bool filterStrict)
        {
            var graph = new Graph();
            var lines = new List<string>();
            using (var reader = new StreamReader(input))
            {
                var line = reader.ReadLine();
                while (line != null)
                {
                    lines.Add(line);
                    line = reader.ReadLine();
                }
            }
            throw new Exception("bad");
            //lines = FilterStrict(lines);
            lines.ForEach(f => Add(f, graph));
            return graph;
        }


        public Graph FromFile(string input, bool filter)
        {
            var graph = new Graph();
            var lines = new List<string>();
            using (var reader = new StreamReader(input))
            {
                var line = reader.ReadLine();
                while (line != null)
                {
                    lines.Add(line);
                    line = reader.ReadLine();
                }
            }

            lines = Filter(lines);
            lines.ForEach(f => Add(f, graph));
            return graph;
        }

        public Graph FromFile(string input)
        {
            var graph = new Graph();
            var lines = new List<string>();
            using (var reader = new StreamReader(input))
            {
                var line = reader.ReadLine();
                while (line != null)
                {
                    Add(line, graph);
                    line = reader.ReadLine();
                }
            }
            return graph;
        }
    }
}

