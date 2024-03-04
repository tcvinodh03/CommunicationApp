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

        
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;


        public UserController(IUnitOfWork unitOfWork, IMapper mapper, IPhotoService photoService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _photoService = photoService;
        }

       
        [HttpGet]
        public async Task<ActionResult<PagedList<MemberDTO>>> GetUsers([FromQuery] UserParams userParams)
        {
            string userName = User.getUserName();
            var gender = await _unitOfWork.userRepo.GetUserGender(userName);
            userParams.CurrentUserName = userName;
            if (string.IsNullOrEmpty(userParams.Gender))
            {
                userParams.Gender = gender == "male" ? "female" : "male";
            }
            var userList = await _unitOfWork.userRepo.GetMemberAsync(userParams);
            Response.AddPaginationHeader(new PaginationHeader(userList.CurrentPage, userList.PageSize, userList.TotalCount, userList.TotalPages));
            return Ok(userList);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<MemberDTO>> GetUserById(int id)
        {

            return await _unitOfWork.userRepo.GetMemberByIdAsync(id);
        }

        
        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDTO>> GetUserByName(string userName)
        {
            var abc = await _unitOfWork.userRepo.GetMemberByNameAsync(userName);
            return abc;

        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto objMemberUpdateDto)
        {
            var user = await _unitOfWork.userRepo.GetUserByNameAsync(User.getUserName());
            if (user == null) return NotFound();
            _mapper.Map(objMemberUpdateDto, user);
            if (await _unitOfWork.Complete()) return NoContent();
            return BadRequest("Failed to update user");

        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            var user = await _unitOfWork.userRepo.GetUserByNameAsync(User.getUserName());
            if (user == null) return NotFound();
            var result = await _photoService.AddPhotoAsync(file);
            if (result.Error != null) return BadRequest(result.Error.Message);

            var photoObj = new Photo();
            photoObj.Url = result.SecureUrl.AbsoluteUri;
            photoObj.PublicId = result.PublicId;
            if (user.Photos.Count == 0) photoObj.IsMain = true;
            user.Photos.Add(photoObj);
            if (await _unitOfWork.Complete())
            {
                return CreatedAtAction(nameof(GetUsers), new { username = User.getUserName() }, _mapper.Map<PhotoDto>(photoObj));
            }
            return BadRequest("Problem adding Photos");
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoid)
        {
            var user = await _unitOfWork.userRepo.GetUserByNameAsync(User.getUserName());
            if (user == null) return NotFound();
            var photo = user.Photos.FirstOrDefault(x => x.Id == photoid);
            if (photo == null) return NotFound();
            if (photo.IsMain) return BadRequest("This is already your main photo");
            var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);
            if (currentMain != null) currentMain.IsMain = false;
            photo.IsMain = true;
            if (await _unitOfWork.Complete()) return NoContent();
            return BadRequest("Issue in setting main photo");

        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var user = await _unitOfWork.userRepo.GetUserByNameAsync(User.getUserName());
            if (user == null) return NotFound();
            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
            if (photo == null) return NotFound();
            if (photo.IsMain) return BadRequest("This is the main photo, so not possible to delete photo");
            if (photo.PublicId != null)
            {
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);
                if (result.Error != null) return BadRequest(result.Error.Message);
            }
            user.Photos.Remove(photo);
            if (await _unitOfWork.Complete()) return Ok();
            return BadRequest("Issue in deleting Photo");
        }



    }

}


