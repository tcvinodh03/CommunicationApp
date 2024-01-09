using CommunicationAPI.DTO;
using CommunicationAPI.Entities;
using CommunicationAPI.Extension;
using CommunicationAPI.Helpers;
using CommunicationAPI.Interface;
using Microsoft.EntityFrameworkCore;

namespace CommunicationAPI.Data
{
    public class LikesRepo : ILikesRepo
    {
        private readonly DataContext _context;

        public LikesRepo(DataContext context)
        {
            _context = context;
        }
        public async Task<UserLike> GetUserLike(int sourceUserId, int targetUserId)
        {
            return await _context.Likes.FindAsync(sourceUserId, targetUserId);
        }

        public async Task<PagedList<LikeDto>> GetUserLikes(LikeParams likeParams)
        {
            var users = _context.Users.OrderBy(u => u.UserName).AsQueryable();
            var likes = _context.Likes.AsQueryable();
            if (likeParams.Predicate == "liked")
            {
                likes = likes.Where(like => like.SourceUserId == likeParams.UserId);
                users = likes.Select(like => like.TargetUser);
            }
            if (likeParams.Predicate == "likedBy")
            {
                likes = likes.Where(like => like.TargetUserId == likeParams.UserId);
                users = likes.Select(like => like.SourceUser);
            }

            var likedUser = users.Select(user => new LikeDto
            {
                UserName = user.UserName,
                Knownas = user.KnownAs,
                Age = user.DateOfBirth.CalculateAge(),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain).Url,
                City = user.City,
                Id = user.Id
            });

            return await PagedList<LikeDto>.CreateAsync(likedUser, likeParams.PageNumber, likeParams.PageSize);
        }

        public async Task<AppUser> GetUserWithLikes(int userId)
        {
            return await _context.Users.Include(x => x.LikedUsers).FirstOrDefaultAsync(x => x.Id == userId);
        }
    }
}
