using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E_Auction.ViewModels
{
    public class LotView
    {
        public string LotName { get; set; }
        public string LotOwner { get; set; }
        public decimal MinRate { get; set; }
        public decimal CurrentRate { get; set; }
        public string Description { get; set; }
        public byte[] Image { get; set; }
    }
}