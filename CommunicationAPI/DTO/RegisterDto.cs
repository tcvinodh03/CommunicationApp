using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CommunicationAPI.DTO
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Name is required")]
        public string userName { get; set; }

        [Required]
        public string password { get; set; }
    }
}