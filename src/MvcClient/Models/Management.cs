using System;
using System.Collections.Generic;

namespace MvcClient.Models
{
    public class Management
    {
        public int ItemId { get; set; }
        public string Name { get; set; }
        public double UnitPrice { get; set; }
        public int TotalUnits { get; set; }
    }
}