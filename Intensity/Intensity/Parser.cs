using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;

namespace Intensity
{
    public class Parser
    {
        private class KeywordInfo
        {
            public int N = 0;
            public int Sum = 0;
            public Dictionary<int, bool> Advertisers = new Dictionary<int, bool>();
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

            using (var writer = new StreamWriter(output))
            {
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
                        writer.WriteLine(writeLine);
                    }
                }
            }
        }

        public Graph FromFile(string input)
        {
            var graph = new Graph();
            using (var reader = new StreamReader(input))
            {
                var line = reader.ReadLine();
                while (line != null)
                {
                    var split = line.Split('\t');
                    var advertiser = int.Parse(split.First());
                    var keyword = int.Parse(split[1]);
                    //var weight = Math.Round(decimal.Parse(split[2]), 2).ToFloat();
                    //if (weight > 0)
                    {
                        graph.AddEdge(advertiser, true, keyword, false, 1);
                    }
                    line = reader.ReadLine();
                }
            }
            return graph;
        }
    }
}
