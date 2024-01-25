using CommunicationAPI.Entities;

namespace CommunicationAPI.Interface
{
    public interface ITokenService
    {
      Task<string> CreateToken(AppUser appUser);
    }
}
