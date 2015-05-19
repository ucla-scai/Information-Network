using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intensity
{
    public class Advertiser
    {
        //col1:subuid, advertiser’s id with sector id, say, “Google_IT"
        //col2:cmatch, show positions of the ads
        //col3:date, same as the file date
        //col4:shw, total show times, impressions
        //col5:clk, clicks for different positions
        //col6:csm, total consumption of the advertiser
        //col7:bid, “average” bidding price
        //col8:Q, quality of the ads
        //col9:acp, average click price, average cost per click
        //col10: rank, average rank of all the show ads by one advertiser
        //col11: asn, won’t use this attribute
        public int Id;
        public int Sector;
        public int ShowPositions;
        public DateTime Date;
        public int Impressions;
        public int Clicks;
        public int Consumption;
        public float BiddingPrice;
        public float Quality;
        public float ClickPrice;
        public float Rank;
        public float Asn;
        public Dictionary<int, Keyword> Keywords = new Dictionary<int,Keyword>();
    }
}
