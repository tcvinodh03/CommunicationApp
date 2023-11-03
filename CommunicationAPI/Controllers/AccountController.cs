
using System.Security.Cryptography;
using System.Text;
using CommunicationAPI.Data;
using CommunicationAPI.DTO;
using CommunicationAPI.Entities;
using CommunicationAPI.Interface;
using CommunicationAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CommunicationAPI.Controllers
{

    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;
        public AccountController(DataContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        [HttpPost("Register")]
        public async Task<ActionResult<AppUser>> Register(RegisterDto objRegister)
        {
            if (await UserExists(objRegister.userName))
            {
                return BadRequest("User already exists");
            }
            using var hmac = new HMACSHA512();

            var user = new AppUser
            {
                UserName = objRegister.userName.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(objRegister.password)),
                PasswordSalt = hmac.Key
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(LoginDto objLogin)
        {
            var user = await _context.Users.Include(p=>p.Photos).
                FirstOrDefaultAsync(u => u.UserName.Equals(objLogin.userName));
            if (user == null) return Unauthorized("User did not exists");
            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHASH = hmac.ComputeHash(Encoding.UTF8.GetBytes(objLogin.password));
            for (int i = 0; i < computedHASH.Length; i++)
            {
                if (computedHASH[i] != user.PasswordHash[i]) return Unauthorized("credentials not vaid");
            }
            var objLoggedUser=  new UserDTO
            {
                UserName = user.UserName,
                Token = _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain).Url
            };
            return objLoggedUser;
        }

        private async Task<bool> UserExists(string userName)
        {

            return await _context.Users.AnyAsync(u => u.UserName.ToLower() == userName.ToLower());
        }
    }
}