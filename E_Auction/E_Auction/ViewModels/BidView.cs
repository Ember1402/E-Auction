using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E_Auction.ViewModels
{
    public class BidView
    {
        public int BidId { get; set; }
        public int LotId { get; set; }
        public decimal BidRate { get; set; }
        public string LotName { get; set; }
        public string BidOwnerName { get; set; }
        public DateTime BidDate { get; set; }
    }
}