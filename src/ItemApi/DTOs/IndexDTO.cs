using System.Collections.Generic;

namespace ItemApi.DTOs
{
    public class IndexDTO
    {
        public string SearchString { get; set; }
        public string ItemCategory { get; set; }
        public IEnumerable<string> Categories { get; set; }
        public IEnumerable<ItemDTO> Items { get; set; }
    }
}