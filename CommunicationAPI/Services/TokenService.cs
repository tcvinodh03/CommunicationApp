using CommunicationAPI.Entities;
using CommunicationAPI.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CommunicationAPI.Services
{
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _symmetricSecurityKey;
        private readonly UserManager<AppUser> _userManager;

        public TokenService(IConfiguration config,UserManager<AppUser> userManager)
        {
            _symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
            _userManager = userManager;
        }
        public async Task<string> CreateToken(AppUser appUser)
        {
            var _claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId,appUser.Id.ToString()),
                 new Claim(JwtRegisteredClaimNames.UniqueName,appUser.UserName),
            };
            var roles = await _userManager.GetRolesAsync(appUser);
            _claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));


            var _signCredentials = new SigningCredentials(_symmetricSecurityKey, SecurityAlgorithms.HmacSha512Signature);

            var _tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(_claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = _signCredentials
            };
            var _tokenHandler = new JwtSecurityTokenHandler();
            var _token = _tokenHandler.CreateToken(_tokenDescriptor);
            return _tokenHandler.WriteToken(_token);
        }
    }
}
