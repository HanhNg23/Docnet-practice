using DocnetCorePractice.Data.Entity;
using DocnetCorePractice.Model;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DocnetCorePractice.Services
{
    public interface IAuthenticationService
    {
        ResponseLoginModel Authenticator(RequestLoginModel model);
    }
    public class AuthenticationService : IAuthenticationService
    {
        private readonly string Key = "suifbweudfwqudgweufgewufgwefcgweiudgweidgwed";
        public AuthenticationService()
        {

        }

        public ResponseLoginModel Authenticator(RequestLoginModel model)
        {
            //var account =             //viết repository get UserEntity
            //if (account == null)
            //{
            // kiểm tra nếu user == null thì thorw ex
            //}
            var account = new UserEntity();

            var token = CreateJwtToken(account);
            var refreshToken = CreateRefreshToken(account);
            var result = new ResponseLoginModel
            {
                FullName = account.FirstName,
                UserId = account.Id,
                Token = token,
                RefreshToken = refreshToken.Token
            };
            return result;
        }

        private RefreshTokens CreateRefreshToken(UserEntity account)
        {
            var randomByte = new byte[64];
            var token = ""; // Viết hàm tạo chuỗi random string
            var refreshToken = new RefreshTokens
            {
                UserId = account.Id,
                Expires = DateTime.Now.AddDays(1),
                IsActive = true,
                Token = token
            };
            // viết code insert refreshToken vào DB
            return refreshToken;
        }

        private string CreateJwtToken(UserEntity account)
        {
            var tokenHanler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(Key);
            var securityKey = new SymmetricSecurityKey(key);
            var credential = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, account.FirstName),
                    new Claim(ClaimTypes.Email, "sdasdasd"),
                    new Claim("CarNumber", "1")
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = credential
            };
            var token = tokenHanler.CreateToken(tokenDescription);
            return tokenHanler.WriteToken(token);
        }
    }
}
