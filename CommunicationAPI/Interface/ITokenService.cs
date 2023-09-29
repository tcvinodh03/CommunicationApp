using CommunicationAPI.Entities;

namespace CommunicationAPI.Interface
{
    public interface ITokenService
    {
        string CreateToken(AppUser appUser);
    }
}
