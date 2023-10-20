using CommunicationAPI.DTO;
using CommunicationAPI.Entities;

namespace CommunicationAPI.Interface
{
    public interface IuserRepo
    {
        void Update(AppUser user);
        Task<bool> SaveAllAsync();
        Task<IEnumerable<AppUser>> GetUserAsync();
        Task<AppUser> GetUserByIdAsync(int id);
        Task<AppUser> GetUserByNameAsync(string userName);

        Task<IEnumerable<MemberDTO>> GetMemberAsync();
        Task<MemberDTO> GetMemberByIdAsync(int id);
        Task<MemberDTO> GetMemberByNameAsync(string userName);
    }
}
