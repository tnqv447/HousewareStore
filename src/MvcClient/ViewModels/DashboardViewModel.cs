using MvcClient.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
namespace MvcClient.ViewModels
{
    public class DashboardViewModel
    {
        public decimal TotalRevenue { get; set; }
        public IList<DataChart> Data { get; set; }
        public IList<Item> CommonItems { get; set; }
        public int CountApproved { get; set; }
        public int CountSubmitted { get; set; }
        public int CountRejected { get; set; }
    }
    public class DataChart
    {
        public string Month { get; set; }
        public decimal Revenue { get; set; }
        public DataChart() { }
        public DataChart(string Month, decimal Revenue)
        {
            this.Month = Month;
            this.Revenue = Revenue;
        }
    }
}