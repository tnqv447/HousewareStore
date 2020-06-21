using MvcClient.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System;

namespace MvcClient.ViewModels
{
    public class AnalysisViewModel
    {
        // public IEnumerable<Management> CountSale { get; set; }
        public IList<BaseAnalysis> SalesCount { get; set; }
        public IList<BaseAnalysis> AllBuyers { get; set; }
        public IEnumerable<ItemAnalysis> AllItems { get; set; }
        public IEnumerable<ItemAnalysis> BuyersCount { get; set; }
        public IList<SaleData> SalesChart { get; set; }
    }
    public class SaleData{
        public DateTime Date { get; set; }
        public IList<BaseAnalysis> Sales { get; set; }
    }
    public class BaseAnalysis
    {
        public User User { get; set; }
        public IEnumerable<ItemAnalysis> Count { get; set; }
        public Item item { get; set; }
        public int TotalUnits { get; set; }
        public double TotalPrices { get; set; }
    }
}