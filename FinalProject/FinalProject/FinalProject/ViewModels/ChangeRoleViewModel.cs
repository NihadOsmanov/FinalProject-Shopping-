using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.ViewModels
{
    public class ChangeRoleViewModel
    {
        public int Id { get; set; }
        public string Username { get; set; }

        [Required]
        public string Role { get; set; }
        public List<string> Roles { get; set; }
    }
}
