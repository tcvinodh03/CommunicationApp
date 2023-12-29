using CommunicationAPI.DTO;
using CommunicationAPI.Entities;
using CommunicationAPI.Helpers;

namespace CommunicationAPI.Interface
{
    public interface IuserRepo
    {
        void Update(AppUser user);
        Task<bool> SaveAllAsync();
        Task<IEnumerable<AppUser>> GetUserAsync();
        Task<AppUser> GetUserByIdAsync(int id);
        Task<AppUser> GetUserByNameAsync(string userName);
        Task<PagedList<MemberDTO>> GetMemberAsync(UserParams userParams);
        Task<MemberDTO> GetMemberByIdAsync(int id);
        Task<MemberDTO> GetMemberByNameAsync(string userName);
    }
}
