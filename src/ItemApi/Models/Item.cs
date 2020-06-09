using System;
using System.Collections.Generic;

namespace ItemApi.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double UnitPrice { get; set; }
        public string OwnerId { get; set; }
        public string PictureUrl { get; set; }
        public string Description { get; set; }
        public ItemStatus ItemStatus { get; set; }
        public DbStatus DbStatus { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }

    }

    public enum ItemStatus
    {
        Submitted = 0,
        Approved = 1,
        Rejected = 2,
    }

    public enum DbStatus
    {
        Active = 0,
        Disabled = 1,
        All = 2
    }
}