using MvcClient.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace MvcClient.ViewModels{
    public class BuyerViewModel
    {
        public IEnumerable<Order> order { get; set; }
        public Buyer buyer { get; set; }

        public IFormFile ImageURL { get; set; }
    }
}