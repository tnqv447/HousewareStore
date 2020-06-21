using System;
using System.Collections.Generic;

namespace MvcClient.Models
{
    public class ItemAnalysis
    {
        public string PictureUrl { get; set; }
        public string UserId { get; set; }
        public int ItemId { get; set; }
        public string Name { get; set; }
        public double UnitPrice { get; set; }
        public int TotalUnits { get; set; }
        public double TotalPrices { get; set; }
    }
}