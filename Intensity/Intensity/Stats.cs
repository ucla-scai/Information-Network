using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;

namespace Intensity
{
    public class Stats
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

        public static void GetMaxSectorAdvertiser(string advertiserFile, string output)
        {
            var lines = new List<string>();
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

            var maxAdv = -1;
            var maxCount = -1;

            foreach (var data in sectorAdvertisers)
            {
                if (data.Value.Count > maxCount)
                {
                    maxCount = data.Value.Count;
                    maxAdv = data.Key;
                }
            }

            Console.Write("");

        }

        public static List<string> GetOneSectorAdvertisersFile(string advertiserFile)
        {
            var lines = new List<string>();
            var advertiserSectors = new Dictionary<int, Dictionary<int, bool>>();
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
                    if (!advertiserSectors.ContainsKey(id)) { advertiserSectors[id] = new Dictionary<int, bool>(); }
                    advertiserSectors[id][sector] = true;
                    line = reader.ReadLine();
                }
            }

            using (var reader = new StreamReader(advertiserFile))
            {
                var line = reader.ReadLine();
                while (line != null)
                {
                    var split = line.Split('\t');
                    var id_sector = split.First();
                    var id = int.Parse(id_sector.Split('_').First());
                    var sector = int.Parse(id_sector.Split('_').Last());
                    if (advertiserSectors[id].Count == 1)
                    {
                        lines.Add(line);
                    }
                    line = reader.ReadLine();
                }
            }

            return lines;
        }

        public static void GetOneSectorAdvertisers(string advertiserFile, string output)
        {
            var lines = new List<string>();
            var advertiserSectors = new Dictionary<int, Dictionary<int, bool>>();
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
                    if (!advertiserSectors.ContainsKey(id)) { advertiserSectors[id] = new Dictionary<int, bool>(); }
                    advertiserSectors[id][sector] = true;
                    line = reader.ReadLine();
                }
            }

            using (var reader = new StreamReader(advertiserFile))
            {
                var line = reader.ReadLine();
                while (line != null)
                {
                    var split = line.Split('\t');
                    var id_sector = split.First();
                    var id = int.Parse(id_sector.Split('_').First());
                    var sector = int.Parse(id_sector.Split('_').Last());
                    if (advertiserSectors[id].Count == 1)
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

        public static void GetSectorAdvertisers(string advertiserFile, string output)
        {
            var lines = new List<string>();
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
                    if (sector == 5905)
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

        public static void GetKeywordFile(string advertiserFile, string keywordFile, string output)
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

            var luckySectors = new List<int>(new[] { 5905 });
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
            var lines = new List<string>();
            using (var reader = new StreamReader(keywordFile))
            {
                var line = reader.ReadLine();
                while (line != null)
                {
                    var split = line.Split('\t');
                    var advertiser = int.Parse(split.First());
                    if (luckyAdvertisers.ContainsKey(advertiser))
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

        public static void GetCpC(string targetSector, string advertiserFile, string output)
        {
            var advertiserSectorCpc = new Dictionary<string, float>();
            using (var reader = new StreamReader(advertiserFile))
            {
                var line = reader.ReadLine();
                while (line != null)
                {
                    var split = line.Split('\t');
                    var id_sector = split.First();
                    var id = int.Parse(id_sector.Split('_').First());
                    var sector = int.Parse(id_sector.Split('_').Last());
                    if (sector.ToString() == targetSector)
                    {
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
                        advertiserSectorCpc[id_sector] = clickPrice;
                    }
                    line = reader.ReadLine();
                }
            }

            using (var writer = new StreamWriter(output))
            {
                foreach (var data in advertiserSectorCpc)
                {
                    var writeLine = string.Format("{0}\t{1}", data.Key, data.Value);
                    writer.WriteLine(writeLine);
                }
            }
        }

        public static void GetMultiSectorAdvertisers(string advertiserFile, string output)
        {
            var advertiserSectors = new Dictionary<int, Dictionary<int, bool>>();
            using (var reader = new StreamReader(advertiserFile))
            {
                var line = reader.ReadLine();
                while (line != null)
                {
                    var split = line.Split('\t');
                    var id_sector = split.First();
                    var id = int.Parse(id_sector.Split('_').First());
                    var sector = int.Parse(id_sector.Split('_').Last());
                    if (!advertiserSectors.ContainsKey(id)) { advertiserSectors[id] = new Dictionary<int, bool>(); }
                    advertiserSectors[id][sector] = true;
                    line = reader.ReadLine();
                }
            }

            using (var writer = new StreamWriter(output))
            {
                foreach (var data in advertiserSectors)
                {
                    if (data.Value.Count > 1)
                    {
                        var writeLine = string.Format("{0}\t{1}", data.Key, string.Join(",", data.Value.Keys.ToList().ToArray()));
                        writer.WriteLine(writeLine);
                    }
                }
            }
        }

        public static void GetSector(string input, string advertiserFile)
        {
            var advertisers = new Dictionary<int, bool>();

            using (var reader = new StreamReader(input))
            {
                var line = reader.ReadLine();
                while (line != null)
                {
                    var split = line.Split('\t');
                    var advertiser = split.First();
                    advertisers[int.Parse(advertiser)] = true;
                    line = reader.ReadLine();
                }
            }

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

            var potentials = new List<int>();

            foreach (var sector in sectorAdvertisers.Keys.ToList())
            {
                var luckySectors = new List<int>();
                luckySectors.Add(sector);
                var luckyAdvertisers = new Dictionary<int, bool>();
                foreach (var luckySector in luckySectors)
                {
                    foreach (var advertiser in sectorAdvertisers[luckySector].Keys)
                    {
                        luckyAdvertisers[advertiser] = true;
                    }
                }
                var isPotential = true;
                foreach (var advertiser in advertisers.Keys)
                {
                    if (!luckyAdvertisers.ContainsKey(advertiser)) { isPotential = false; break; }
                }
                if (isPotential) { potentials.Add(sector); }
            }

            Console.Write("");
        }
    }
}
