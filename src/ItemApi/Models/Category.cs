using System.Collections.Generic;
using System;

namespace ItemApi.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }

        public IList<Item> Items { get; set; }
    }
}