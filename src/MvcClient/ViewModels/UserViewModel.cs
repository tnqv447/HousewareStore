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
        //search
        public string SearchName { get; set; }
        public List<string> RoleList { get; set; }


    }
}