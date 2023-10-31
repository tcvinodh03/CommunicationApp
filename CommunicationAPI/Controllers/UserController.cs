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

namespace CommunicationAPI.Controllers
{
    [Authorize]
    public class UserController : BaseApiController
    {

        private readonly IuserRepo _userRepo;
        private readonly IMapper _mapper;

        public UserController(IuserRepo userRepo, IMapper mapper)
        {
            _userRepo = userRepo;
            _mapper = mapper;
        }

        //[AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDTO>>> GetUsers()
        {
            return Ok(await _userRepo.GetMemberAsync());
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
            var userName = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = await _userRepo.GetUserByNameAsync(userName);
            if (user == null) return NotFound();
            _mapper.Map(objMemberUpdateDto, user);
            if (await _userRepo.SaveAllAsync()) return NoContent();
            return BadRequest("Failed to update user");

        }

    }
}


