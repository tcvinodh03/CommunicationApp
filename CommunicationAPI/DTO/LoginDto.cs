using System.ComponentModel.DataAnnotations;

namespace CommunicationAPI.DTO
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Name is required")]
        public string userName { get; set; }

        [Required]
        public string password { get; set; }
    }
}
