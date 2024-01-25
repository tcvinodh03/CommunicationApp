using Microsoft.AspNetCore.Identity;

namespace CommunicationAPI.Entities
{
    public class AppRole : IdentityRole<int>    
    {
        public ICollection<AppUserRole> UserRoles { get; set; }
    }
}
