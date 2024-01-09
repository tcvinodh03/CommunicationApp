using CommunicationAPI.DTO;
using CommunicationAPI.Entities;
using CommunicationAPI.Helpers;

namespace CommunicationAPI.Interface
{
    public interface ILikesRepo
    {
        public Task<UserLike> GetUserLike(int sourceUserId, int targetUserId);

        public Task<AppUser> GetUserWithLikes(int userId);

        public Task<PagedList<LikeDto>> GetUserLikes(LikeParams likeParams);

    }
}
