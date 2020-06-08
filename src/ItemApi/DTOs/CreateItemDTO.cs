using System.Collections.Generic;
using ItemApi.Models;
namespace ItemApi.DTOs
{
    public class CreateItemDTO
    {
        public Item Item { get; set; }
        public IEnumerable<Category> Categories { get; set; }
    }
}