using MvcClient.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace MvcClient.ViewModels{
    public class AnalysisViewModel
    {
        // public IEnumerable<Management> CountSale { get; set; }
        public User Sale { get; set; }
        public IEnumerable<Management> Count { get; set; }
    }

}