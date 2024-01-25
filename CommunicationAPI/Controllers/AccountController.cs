
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using CommunicationAPI.Data;
using CommunicationAPI.DTO;
using CommunicationAPI.Entities;
using CommunicationAPI.Interface;
using CommunicationAPI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CommunicationAPI.Controllers
{

    public class AccountController : BaseApiController
    {

        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AccountController(UserManager<AppUser> userManager, ITokenService tokenService, IMapper mapper)
        {

            _userManager = userManager;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        [HttpPost("Register")]
        public async Task<ActionResult<UserDTO>> Register(RegisterDto objRegister)
        {
            if (await UserExists(objRegister.userName)) return BadRequest("User already exists");

            var user = _mapper.Map<AppUser>(objRegister);

            // using var hmac = new HMACSHA512();
            //user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(objRegister.password));
            //user.PasswordSalt = hmac.Key;
            //_context.Users.Add(user);
            //await _context.SaveChangesAsync();

            user.UserName = objRegister.userName.ToLower();
            var result = await _userManager.CreateAsync(user, objRegister.password);
            if (!result.Succeeded) return BadRequest(result.Errors.ToString());

            var roleResult = await _userManager.AddToRoleAsync(user, "Member");
            if (!roleResult.Succeeded) return BadRequest(result.Errors);

            return new UserDTO
            {
                UserName = user.UserName,
                Token = await _tokenService.CreateToken(user),
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };


        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(LoginDto objLogin)
        {
            var user = await _userManager.Users.Include(p => p.Photos).
                FirstOrDefaultAsync(u => u.UserName.Equals(objLogin.userName));
            if (user == null) return Unauthorized("User did not exists");
            var result = await _userManager.CheckPasswordAsync(user, objLogin.password);
            if (!result) return Unauthorized("Not a valid credentials");
            //using var hmac = new HMACSHA512(user.PasswordSalt);
            //var computedHASH = hmac.ComputeHash(Encoding.UTF8.GetBytes(objLogin.password));
            //for (int i = 0; i < computedHASH.Length; i++)
            //{
            //    if (computedHASH[i] != user.PasswordHash[i]) return Unauthorized("credentials not vaid");
            //}
            var objLoggedUser = new UserDTO
            {
                UserName = user.UserName,
                Token = await _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain) != null ? user.Photos.FirstOrDefault(x => x.IsMain).Url : null,
                KnownAs = user.KnownAs,
                Gender = user.Gender

            };
            return objLoggedUser;
        }

        private async Task<bool> UserExists(string userName)
        {

            return await _userManager.Users.AnyAsync(u => u.UserName.ToLower() == userName.ToLower());
        }
    }
}