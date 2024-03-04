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
        private readonly IUnitOfWork _unitOfWork;

        public LikesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike(string userName)
        {
            var sourceUserId = User.getUserId();
            var likedUers = await _unitOfWork.userRepo.GetUserByNameAsync(userName);
            var sourceusers = await _unitOfWork.likesRepo.GetUserWithLikes(sourceUserId);
            if (likedUers == null) return NotFound();
            if (sourceusers.UserName == userName) return BadRequest("You cant like you");
            var userLikes = await _unitOfWork.likesRepo.GetUserLike(sourceUserId, likedUers.Id);
            if (userLikes != null) return BadRequest("You already liked");
            userLikes = new UserLike
            {
                SourceUserId = sourceUserId,
                TargetUserId = likedUers.Id
            };
            sourceusers.LikedUsers.Add(userLikes);

            if (await _unitOfWork.Complete()) return Ok();
            return BadRequest("Faild to like user");

        }

        [HttpGet]
        public async Task<ActionResult<PagedList<LikeDto>>> getUserLikes([FromQuery]LikeParams likesParams)
        {
            likesParams.UserId = User.getUserId();
            var users = await _unitOfWork.likesRepo.GetUserLikes(likesParams);
            Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages));
            return Ok(users);
        }



    }
}
