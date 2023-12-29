using CommunicationAPI.Entities;
using CommunicationAPI.Interface;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CommunicationAPI.Services
{
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _symmetricSecurityKey;
        public TokenService(IConfiguration config)
        {
            _symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
        }
        public string CreateToken(AppUser appUser)
        {
            var _claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId,appUser.Id.ToString()),
                 new Claim(JwtRegisteredClaimNames.UniqueName,appUser.UserName),
            };

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
