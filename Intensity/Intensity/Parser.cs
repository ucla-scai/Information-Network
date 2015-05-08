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
        public void ToFile(string keywordFile, string advertiserFile, string output)
        {
            var advertisers = new Dictionary<string, Advertiser>();
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
                    var advertiser = new Advertiser()
                    {
                        Asn = asn,
                        BiddingPrice = biddingPrice,
                        ClickPrice = clickPrice,
                        Clicks = clicks,
                        Consumption = consumption,
                        Date = date,
                        Id = id,
                        Impressions = impressions,
                        Quality = quality,
                        Rank = rank,
                        Sector = sector,
                        ShowPositions = showPositions
                    };
                    advertisers[id_sector] = advertiser;
                    if (!advertiserSectors.ContainsKey(id)) { advertiserSectors[id] = new Dictionary<int, bool>(); }
                    advertiserSectors[id][sector] = true;
                    line = reader.ReadLine();
                }
            }
            using (var writer = new StreamWriter(output))
            {
                using (var reader = new StreamReader(keywordFile))
                {
                    var line = reader.ReadLine();
                    while (line != null)
                    {
                        var split = line.Split('\t');
                        var advertiser = int.Parse(split.First());
                        var id = int.Parse(split[1]);
                        var position = int.Parse(split[2]);
                        var consumption = int.Parse(split[3]);

                        var keyword = new Keyword() { Advertiser = advertiser, Consumption = consumption, Id = id, Position = position };

                        foreach (var sector in advertiserSectors[advertiser].Keys)
                        {
                            var id_sector = advertiser.ToString() + "_" + sector.ToString();
                            keyword.Advertisers[sector] = advertisers[id_sector];
                            advertisers[id_sector].Keywords[id] = keyword;
                        }
                        var writeLine = string.Format("{0}\t{1}\t{2}\t{3}", keyword.Advertiser, keyword.Id, 0.5, 1);
                        writer.WriteLine(writeLine);
                        line = reader.ReadLine();
                    }
                }
            }
        }
    }
}
