using System;
using ItemApi.Models;

namespace ItemApi.DTOs {
    public class ItemDTO {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal UnitPrice { get; set; }
        public string PictureUrl { get; set; }
        public string Description { get; set; }
        public string OwnerId { get; set; }
        public ItemStatus ItemStatus { get; set; }
        public DbStatus DbStatus { get; set; }

        public int CategoryId { get; set; }

    }
}