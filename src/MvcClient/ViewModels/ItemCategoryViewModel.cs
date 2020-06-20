using MvcClient.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
namespace MvcClient.ViewModels
{
    public class ItemCategoryViewModel
    {
        public Item Item { get; set; }
        public IEnumerable<Category> Categories { get; set; }

        [Display(Name = "Item Picutre")]
        [RegularExpression(@"([a-zA-Z0-9\s_\\.\-:])+(.png|.jpg|.gif)$", ErrorMessage = "Only Image files allowed.")]
        public IFormFile ImageURL { get; set; }
    }
}