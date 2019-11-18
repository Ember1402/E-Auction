using E_Auction.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace E_Auction.Context
{
    public class AuctionContext: DbContext
    {
        public AuctionContext(): base("DefaultConnection") 
        {

        }
        public DbSet<Lot> _lots { get; set; }
        public DbSet<Bid> _bids { get; set; }
        public DbSet<Config> Configs { get; set; }
    }
}