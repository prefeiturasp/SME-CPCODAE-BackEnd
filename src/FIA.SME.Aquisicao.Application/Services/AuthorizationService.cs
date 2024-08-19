using FIA.SME.Aquisicao.Core.Helpers;
using FIA.SME.Aquisicao.Infrastructure.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace FIA.SME.Aquisicao.Application.Services
{
    public interface IAuthorizationSMEService
    {
        Task<User?> Authenticate(string email, string password);
        Task<string> CreatePasswordResetHmacCode(string email, bool withouExpiration = false);
        Task<string> GenerateJwtToken(User user);
        Task<Guid?> GetLoggedUserId(System.Security.Claims.ClaimsIdentity userIdentity);
        Task<bool> VerifyPasswordResetHmacCode(string codeBase64Url, string email);
    }

    internal class AuthorizationSMEService : IAuthorizationSMEService
    {
        #region [ Propriedades ]

        private static readonly Byte[] _privateKey = new Byte[] { 0xDE, 0xAD, 0xBE, 0xEF };
        private static readonly TimeSpan _passwordResetExpiry = TimeSpan.FromMinutes(5);
        private readonly IConfiguration _configuration;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUserService _userService;
        private const byte _version = 1;

        #endregion [ FIM - Propriedades ]

        #region [ Construtores ]

        public AuthorizationSMEService(IConfiguration configuration, IPasswordHasher passwordHasher, IUserService userService)
        {
            this._configuration = configuration;
            this._passwordHasher = passwordHasher;
            this._userService = userService;
        }

        #endregion [ FIM - Construtores ]

        #region [ Metodos ]

        public async Task<User?> Authenticate(string email, string password)
        {
            var user = await this._userService.Get(email);

            if (user == null)
                return null;

            var modelPassword = this._passwordHasher.HashPassword(user.id.ToString(), password);

            if (!user.password.Equals(modelPassword))
                return null;

            return user;
        }

        public async Task<string> CreatePasswordResetHmacCode(string email, bool withouExpiration = false)
        {
            var expirationDate = withouExpiration ? DateTime.UtcNow.AddYears(1) : DateTime.UtcNow;

            byte[] message = Enumerable.Empty<byte>()
                .Append(_version)
                .Concat(BitConverter.GetBytes(expirationDate.ToBinary()))
                .Concat(Encoding.UTF8.GetBytes(email))
                .ToArray();
            
            using (HMACSHA256 hmacSha256 = new HMACSHA256(key: _privateKey))
            {
                byte[] hash = hmacSha256.ComputeHash(buffer: message, offset: 0, count: message.Length);

                byte[] outputMessage = message.Concat(hash).ToArray();
                String outputCodeB64 = Convert.ToBase64String(outputMessage);
                String outputCode = outputCodeB64.Replace('+', '-').Replace('/', '_');

                return await Task.FromResult(outputCode);
            }
        }

        public async Task<string> GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this._configuration["Jwt:Secret"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.email),
                new Claim(ClaimTypes.Email, user.email),
                new Claim(ClaimTypes.GivenName, user.name),
                new Claim(ClaimTypes.Role, user.role),
                new Claim(ClaimTypes.Sid, user.id.ToString())
            };

            var token = new JwtSecurityToken(
                this._configuration["Jwt:Issuer"],
                this._configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddDays(15),
                signingCredentials: credentials
            );

            return await Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
        }

        public async Task<Guid?> GetLoggedUserId(System.Security.Claims.ClaimsIdentity userIdentity)
        {
            if (userIdentity == null)
                return null;

            var claim = userIdentity.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/sid");

            if (claim?.Value == null || !Guid.TryParse(claim.Value, out Guid userId))
                return null;

            return userId;
        }

        public async Task<bool> VerifyPasswordResetHmacCode(string codeBase64Url, string email)
        {
            string base64 = codeBase64Url.Replace('-', '+').Replace('_', '/');
            byte[] message = Convert.FromBase64String(base64);

            byte version = message[0];

            if (version < _version)
                return false;

            var createdUtcBinary = BitConverter.ToInt64(message, startIndex: 1);
            DateTime createdUtc = DateTime.FromBinary(createdUtcBinary);
            
            if (createdUtc.Add(_passwordResetExpiry) < DateTime.UtcNow)
                return false;

            var emailLength = Encoding.UTF8.GetBytes(email).Length;

            var emailStartIndex = 1 + sizeof(Int64);
            var emailByteArray = new byte[emailLength];
            Array.Copy(message, emailStartIndex, emailByteArray, 0, emailLength);
            var emailInToken = Encoding.UTF8.GetString(emailByteArray);

            if (!emailInToken.Equals(email, StringComparison.InvariantCultureIgnoreCase))
                return false;

            var _messageLength = 1 + sizeof(Int64) + emailLength;

            using (HMACSHA256 hmacSha256 = new HMACSHA256(key: _privateKey))
            {
                byte[] hash = hmacSha256.ComputeHash(message, offset: 0, count: _messageLength);
                byte[] messageHash = message.Skip(_messageLength).ToArray();

                return await Task.FromResult(Enumerable.SequenceEqual(hash, messageHash));
            }
        }

        #endregion [ FIM - Metodos ]
    }
}
