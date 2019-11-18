using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace E_Auction.Models
{
    public class Config
    {
        public int Id { get; set; }
        [Display(Name ="Частота обновления статуса лотов")]
        public string Value { get; set; }

    }
}