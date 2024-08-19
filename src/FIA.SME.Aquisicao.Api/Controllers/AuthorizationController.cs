using FIA.SME.Aquisicao.Api.Models;
using FIA.SME.Aquisicao.Application.Services;
using FIA.SME.Aquisicao.Core.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FIA.SME.Aquisicao.Api.Controllers
{
    [Route("api")]
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        private readonly IAuthorizationSMEService _authorizationService;
        private readonly IConfiguration _configuration;
        private readonly IMailService _mailService;
        private readonly IUserService _userService;

        public AuthorizationController(
            IAuthorizationSMEService authorizationService,
            IConfiguration configuration,
            IMailService mailService,
            IUserService userService)
        {
            this._authorizationService = authorizationService;
            this._configuration = configuration;
            this._mailService = mailService;
            this._userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task <IActionResult> Login([FromBody] UserLogin userLogin)
        {
            var user = await this._authorizationService.Authenticate(userLogin.email, userLogin.password);

            if (user != null)
            {
                var token = await this._authorizationService.GenerateJwtToken(user);
                var response = new UserLoginResponse(user, token);
                return new ApiResult(new Saida((int)HttpStatusCode.OK, true, "Usuário autenticado com sucesso", response));
            }

            return new ApiResult(new NoContentApiResponse());
        }

        [AllowAnonymous]
        [HttpPost("recuperar-senha")]
        public async Task<IActionResult> SendRecoverPasswordEmail([FromBody] UserEmail userEmail)
        {
            // Valida que o usuário existe
            var user = await this._userService.Get(userEmail.email);

            if (user == null)
                return new ApiResult(new BadRequestApiResponse("Não foram encontrados dados deste usuário"));

            var token = await this._authorizationService.CreatePasswordResetHmacCode(userEmail!.email);

            if (String.IsNullOrEmpty(token))
                return new ApiResult(new BadRequestApiResponse("Ocorreu um erro ao tentar recuperar sua senha"));

            var urlEncodedToken = WebUtility.UrlEncode(token);

            var urlRecoverPassword = $"{this._configuration["Frontend:Url"]}/recuperar-senha/{urlEncodedToken}";
            _ = this._mailService.SendRecoverPasswordEmail(user, urlRecoverPassword);

            return new ApiResult(new Saida((int)HttpStatusCode.OK, true, "E-mail enviado com sucesso", new { token = token }));
        }

        [Authorize]
        [HttpPost("trocar-senha")]
        public async Task<IActionResult> ChangePassword([FromBody] UserChangePassword userChangePassword)
        {
            // Valida que o usuário existe
            var user = await this._userService.Get(userChangePassword.email);

            if (user == null)
                return new ApiResult(new NoContentApiResponse("Não foi possível trocar sua senha"));

            await this._userService.ChangePassword(user.id, userChangePassword.password);

            return new ApiResult(new Saida((int)HttpStatusCode.OK, true, "Senha alterada com sucesso", new { email = user.email }));
        }

        [AllowAnonymous]
        [HttpPost("trocar-senha-token")]
        public async Task<IActionResult> ChangePasswordToken([FromBody] UserRecoverPassword userRecoverPassword)
        {
            // Valida que o usuário existe
            var user = await this._userService.Get(userRecoverPassword.email);

            if (user == null)
                return new ApiResult(new BadRequestApiResponse("Não foram encontrados dados deste usuário"));

            var result = await this._authorizationService.VerifyPasswordResetHmacCode(userRecoverPassword.token, user.email);

            if (!result)
                return new ApiResult(new BadRequestApiResponse("O token utilizado está expirado ou inválido. Clique em recuperar senha novamente"));

            await this._userService.ChangePassword(user.id, userRecoverPassword.password);

            return new ApiResult(new Saida((int)HttpStatusCode.OK, true, "Senha alterada com sucesso", new { email = user.email }));
        }
    }
}
