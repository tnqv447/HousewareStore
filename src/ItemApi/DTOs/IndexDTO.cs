using System.Collections.Generic;

namespace ItemApi.DTOs
{
    public class IndexDTO
    {
        public string SearchString { get; set; }
        public string ItemCategory { get; set; }
        public string SortOrder { get; set; }

        public double MinPrice { get; set; }
        public double MaxPrice { get; set; }
        public IEnumerable<string> Categories { get; set; }
        public IEnumerable<int> CategoriesId { get; set; }
        public IEnumerable<ItemDTO> Items { get; set; }
    }
}