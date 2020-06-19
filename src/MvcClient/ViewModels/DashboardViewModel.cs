using MvcClient.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
namespace MvcClient.ViewModels
{
    public class DashboardViewModel
    {
        public double TotalRevenue { get; set; }
        public IList<DataChart> Data { get; set; }
        public IList<LineItem> CommonItems { get; set; }
        public int CountApproved { get; set; }
        public int CountSubmitted { get; set; }
        public int CountRejected { get; set; }
    }
    public class DataChart
    {
        public string Month { get; set; }
        public double Revenue { get; set; }
        public DataChart() { }
        public DataChart(string Month, double Revenue)
        {
            this.Month = Month;
            this.Revenue = Revenue;
        }
    }
    public class LineItem
    {
        public string ItemName { get; set; }
        public string OwnerName { get; set; }
        public int Total { get; set; }
        public string PictureURL { get; set; }
        public double UnitPrice { get; set; }
    }
}