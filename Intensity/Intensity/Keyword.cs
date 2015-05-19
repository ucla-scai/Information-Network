using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intensity
{
    public class Keyword
    {
        //col1: subuid, advertiser’s id
        //col2: wid, bidding keyword id
        //col3: cmatch, show positions of the ad which is triggered by the keyword
        //col4: csm, total consumption on this keyword
        public int Advertiser;
        public int Id;
        public int Position;
        public int Consumption;
        public Dictionary<int, Advertiser> Advertisers = new Dictionary<int,Advertiser>();
    }
}
