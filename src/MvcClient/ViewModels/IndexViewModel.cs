using System.Collections.Generic;
using MvcClient.Models;

namespace MvcClient.ViewModels
{
    public class IndexViewModel
    {
        public string SearchString { get; set; }
        public string ItemCategory { get; set; }
        public IEnumerable<string> Categories { get; set; }
        public IList<Item> Items { get; set; }        
    }
}