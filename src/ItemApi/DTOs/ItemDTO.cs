using System;
using System.ComponentModel.DataAnnotations;
using ItemApi.Models;

namespace ItemApi.DTOs
{
    public class ItemDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double UnitPrice { get; set; }
        public string PictureUrl { get; set; }
        public string Description { get; set; }
        public string OwnerId { get; set; }
        public ItemStatus ItemStatus { get; set; }
        public DbStatus DbStatus { get; set; }

        public int CategoryId { get; set; }
        public string Category { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{dd/MM/yyyy}")]
        public DateTime PublishDate { get; set; }

        public ItemDTO() { }
        public ItemDTO(Item i, string CategoryName)
        {
            Id = i.Id;
            Name = i.Name;
            UnitPrice = i.UnitPrice;
            PictureUrl = i.PictureUrl;
            Description = i.Description;
            OwnerId = i.OwnerId;
            ItemStatus = i.ItemStatus;
            DbStatus = i.DbStatus;
            CategoryId = i.CategoryId;
            Category = CategoryName;
            PublishDate = i.PublishDate;
        }

    }
}