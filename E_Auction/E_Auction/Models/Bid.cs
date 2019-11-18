using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace E_Auction.Models
{
    public class Bid
    {
        public int BidId { get; set; }
        [DisplayName("Ставка")]
        public decimal BidRate { get; set; }
        public string UserId { get; set; }
        public int LotId { get; set; }
        [DisplayName("Дата ставки")]
        public DateTime BidDate { get; set; }
    }
}