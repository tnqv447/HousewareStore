using MvcClient.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
namespace MvcClient.ViewModels
{
    public class DashboardViewModel
    {
        public decimal TotalRevenue { get; set; }
        public IList<decimal> Revenues { get; set; }
        public IList<Item> CommonItems { get; set; }
        public int CountApproved { get; set; }
        public int CountSubmitted { get; set; }
        public int CountRejected { get; set; }
    }
}