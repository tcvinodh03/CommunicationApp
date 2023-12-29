using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CommunicationAPI.Data;
using CommunicationAPI.DTO;
using CommunicationAPI.Entities;
using CommunicationAPI.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CommunicationAPI.Extension;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc.Filters;
using CommunicationAPI.Helpers;

namespace CommunicationAPI.Controllers
{
    [Authorize]
    public class UserController : BaseApiController
    {

        private readonly IuserRepo _userRepo;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;


        public UserController(IuserRepo userRepo, IMapper mapper,IPhotoService photoService)
        {
            _userRepo = userRepo;
            _mapper = mapper;
            _photoService = photoService;
        }

        //[AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<PagedList<MemberDTO>>> GetUsers([FromQuery]UserParams userParams)
        {
            var currentUser = await _userRepo.GetUserByNameAsync(User.getUserName());
            userParams.CurrentUserName = currentUser.UserName;
            if (string.IsNullOrEmpty(userParams.Gender))
            {
                userParams.Gender = currentUser.Gender == "male" ? "female" : "male";
            }
            var userList = await _userRepo.GetMemberAsync(userParams);
            Response.AddPaginationHeader(new PaginationHeader(userList.CurrentPage, userList.PageSize,userList.TotalCount,userList.TotalPages));
            return Ok(userList);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<MemberDTO>> GetUserById(int id)
        {

            return await _userRepo.GetMemberByIdAsync(id);
        }

        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDTO>> GetUserByName(string userName)
        {
            var abc = await _userRepo.GetMemberByNameAsync(userName);
            return abc;

        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto objMemberUpdateDto)
        {
            var user = await _userRepo.GetUserByNameAsync(User.getUserName());
            if (user == null) return NotFound();
            _mapper.Map(objMemberUpdateDto, user);
            if (await _userRepo.SaveAllAsync()) return NoContent();
            return BadRequest("Failed to update user");

        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            var user = await _userRepo.GetUserByNameAsync(User.getUserName());
            if(user==null) return NotFound();
            var result = await _photoService.AddPhotoAsync(file);
            if(result.Error!=null) return BadRequest(result.Error.Message);

            var photoObj = new Photo();
            photoObj.Url = result.SecureUrl.AbsoluteUri;
            photoObj.PublicId = result.PublicId;
            if (user.Photos.Count == 0) photoObj.IsMain = true;
            user.Photos.Add(photoObj);
            if (await _userRepo.SaveAllAsync())
            {
                return CreatedAtAction(nameof(GetUsers), new { username = User.getUserName() },_mapper.Map<PhotoDto>(photoObj));
            }
            return BadRequest("Problem adding Photos");
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoid)
        {
            var user = await _userRepo.GetUserByNameAsync(User.getUserName());
            if(user==null) return NotFound();
            var photo=user.Photos.FirstOrDefault(x=>x.Id==photoid);
            if(photo == null) return NotFound();
            if (photo.IsMain) return BadRequest("This is already your main photo");
            var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);
            if (currentMain != null) currentMain.IsMain = false;
            photo.IsMain = true;
            if(await _userRepo.SaveAllAsync())return NoContent();
            return BadRequest("Issue in setting main photo");

        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var user = await _userRepo.GetUserByNameAsync(User.getUserName());
            if (user == null) return NotFound();
            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
            if (photo == null) return NotFound();
            if (photo.IsMain) return BadRequest("This is the main photo, so not possible to delete photo");
            if (photo.PublicId != null)
            {
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);
                if(result.Error!=null)return BadRequest(result.Error.Message);                
            }
            user.Photos.Remove(photo);
            if(await _userRepo.SaveAllAsync()) return Ok ();
            return BadRequest("Issue in deleting Photo");
        }

         

    }

}


