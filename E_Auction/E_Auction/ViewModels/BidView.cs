using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace E_Auction.ViewModels
{
    public class BidView
    {
        public int BidId { get; set; }
        public int LotId { get; set; }
        [Display(Name ="Сумма ставки")]
        public decimal BidRate { get; set; }
        [Display(Name ="Лот")]
        public string LotName { get; set; }
        public string BidOwnerName { get; set; }
        [Display(Name ="Дата ставки")]
        public DateTime BidDate { get; set; }
        [Display(Name ="Актуальная сумма")]
        public decimal? CurrentLotRate { get; set; }
    }
}