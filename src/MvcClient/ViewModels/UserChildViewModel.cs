using MvcClient.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace MvcClient.ViewModels
{
    public class UserChildViewModel
    {
        public User User { get; set; }
        public List<string> RoleList { get; set; }

    }
}