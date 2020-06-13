using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

namespace MvcClient.Models
{
    public class Item
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [DataType(DataType.Currency)]
        [Range(1, 1000000)]
        public double UnitPrice { get; set; }

        [MaxLength(100)]
        public string Description { get; set; }

        [MaxLength(30)]
        [RegularExpression(@"^[A-Z]+[a-zA-Z""'\s-]*$")]
        public string Category { get; set; }
        public int CategoryId { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{dd/MM/yyyy}")]
        public DateTime PublishDate { get; set; }

        public string PictureUrl { get; set; }
        public string OwnerId { get; set; }
        public ItemStatus ItemStatus { get; set; }
        public DbStatus DbStatus { get; set; }
    }

    public enum ItemStatus
    {
        Submitted = 0,
        Approved = 1,
        Rejected = 2
    }

    public enum DbStatus
    {
        Active = 0,
        Disabled = 1
    }
}