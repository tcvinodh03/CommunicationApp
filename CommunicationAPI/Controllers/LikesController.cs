using CommunicationAPI.DTO;
using CommunicationAPI.Entities;
using CommunicationAPI.Extension;
using CommunicationAPI.Helpers;
using CommunicationAPI.Interface;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace CommunicationAPI.Controllers
{
    public class LikesController : BaseApiController
    {
        private readonly IuserRepo _userRepo;
        private readonly ILikesRepo _likeRepo;

        public LikesController(IuserRepo userRepo, ILikesRepo likeRepo)
        {
            _userRepo = userRepo;
            _likeRepo = likeRepo;
        }

        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike(string userName)
        {
            var sourceUserId = User.getUserId();
            var likedUers = await _userRepo.GetUserByNameAsync(userName);
            var sourceusers = await _likeRepo.GetUserWithLikes(sourceUserId);
            if (likedUers == null) return NotFound();
            if (sourceusers.UserName == userName) return BadRequest("You cant like you");
            var userLikes = await _likeRepo.GetUserLike(sourceUserId, likedUers.Id);
            if (userLikes != null) return BadRequest("You already liked");
            userLikes = new UserLike
            {
                SourceUserId = sourceUserId,
                TargetUserId = likedUers.Id
            };
            sourceusers.LikedUsers.Add(userLikes);

            if (await _userRepo.SaveAllAsync()) return Ok();
            return BadRequest("Faild to like user");

        }

        [HttpGet]
        public async Task<ActionResult<PagedList<LikeDto>>> getUserLikes([FromQuery]LikeParams likesParams)
        {
            likesParams.UserId = User.getUserId();
            var users = await _likeRepo.GetUserLikes(likesParams);
            Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages));
            return Ok(users);
        }



    }
}
