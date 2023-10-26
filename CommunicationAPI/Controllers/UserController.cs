using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
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

    }
}


