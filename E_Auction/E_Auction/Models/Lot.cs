using E_Auction.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace E_Auction.Models
{
    public class Lot
    {
        public int LotId { get; set; }

        public string UserId { get; set; }

        [DisplayName("Название")]
        public string Name { get; set; }

        [DisplayName("Начальная ставка")]
        public decimal BeginCost { get; set; }

        [DisplayName("Конечная ставка")]
        public decimal? CurrentCost { get; set; }

        [DisplayName("Дата открытия")]
        [DataType(DataType.DateTime)]
        public DateTime StartDate { get; set; }

        [DisplayName("Дата окончания")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }

        [DisplayName("Описание")]
        public string Description { get; set; }

        [DisplayName("Статус")]
        public LotStatus Status { get; set; }

        public byte[] Image { get; set; }
        public string DateString { get; set; }
    }
}