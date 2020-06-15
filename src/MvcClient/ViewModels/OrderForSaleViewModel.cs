using MvcClient.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
namespace MvcClient.ViewModels
{
    public class OrderForSaleViewModel
    {
        public IEnumerable<OrderItemForSales> OrderItems { get; set; }
        public IList<string> Statuses { get; set; }
        public string SearchItemName { get; set; }
        public string SortOrder { get; set; }
        public PaginatedList<OrderItemForSales> OrderItemsPaging { get; set; }
        public int PageIndex { get; set; }
        public int PageTotal { get; set; }
    }
}