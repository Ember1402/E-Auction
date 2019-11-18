using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace E_Auction.Models
{
    public class Message
    {
        public string Subject { get; set; }
        public string MailTo { get; set; }
        public string MailContent { get; set; }
    }
}