using Core.Identity;
using Core.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class TokenServices : ITokenServices
    {
        private readonly IConfiguration _Config;
        private readonly SymmetricSecurityKey _Key;


        public TokenServices(IConfiguration Config)
        {
            _Config = Config;


            _Key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_Config["Token:Key"]));

        }

        public string CreateToken(AppUser user)
        {
            throw new NotImplementedException();
        }

        public string CreatToken(AppUser user)
        {
            var Claims = new List<Claim> {

                new Claim(ClaimTypes.Email,user.Email),
                new Claim(ClaimTypes.GivenName,user.DisplayName)






            };
            var cred = new SigningCredentials(_Key, SecurityAlgorithms.HmacSha512Signature);
            var TokenDiscriptor = new SecurityTokenDescriptor
            {

                Subject = new ClaimsIdentity(Claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = cred,
                Issuer = _Config["Token:Issure"]






            };
            var TokenHandiler = new JwtSecurityTokenHandler();
            var Token = TokenHandiler.CreateToken(TokenDiscriptor);

            return TokenHandiler.WriteToken(Token);


        }
    }
}
