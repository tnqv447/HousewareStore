using MvcClient.Models;
using System.Collections.Generic;
namespace MvcClient.ViewModels
{
    public class CreateItemViewModel
    {
        public Item Item { get; set; }
        public IEnumerable<Category> Categories { get; set; }
    }
}