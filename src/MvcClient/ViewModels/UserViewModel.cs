using MvcClient.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
namespace MvcClient.ViewModels
{
    public class UserViewModel
    {
        public IEnumerable<User> Users { get; set; }
        public PaginatedList<User> UsersPaging { get; set; }
        public int PageIndex { get; set; }
        public int PageTotal { get; set; }
        public string SortOrder { get; set; }
        //search
        public string SearchName { get; set; }
    }
}