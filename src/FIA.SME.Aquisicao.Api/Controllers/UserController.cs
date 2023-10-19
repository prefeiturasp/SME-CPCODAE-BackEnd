using FIA.SME.Aquisicao.Api.Models;
using FIA.SME.Aquisicao.Application.Services;
using FIA.SME.Aquisicao.Core.Domain;
using FIA.SME.Aquisicao.Core.Helpers;
using FIA.SME.Aquisicao.Infrastructure.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FIA.SME.Aquisicao.Api.Controllers
{
    [Route("api/usuario")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IAuthorizationSMEService _authorizationService;
        private readonly ICooperativeService _cooperativeService;
        private readonly IMailService _mailService;
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;

        public UserController(
            IAuthorizationSMEService authorizationService,
            ICooperativeService cooperativeService,
            IMailService mailService,
            IUserService userService,
            IConfiguration configuration)
        {
            this._authorizationService = authorizationService;
            this._cooperativeService = cooperativeService;
            this._mailService = mailService;
            this._userService = userService;
            this._configuration = configuration;
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] UserRegister model)
        {
            // Valida que o e-mail não está cadastrado
            var existingUser = await this._userService.Get(model.email);

            if (existingUser != null)
                return new ApiResult(new BadRequestApiResponse("Este e-mail já está atribuído a outro usuário"));

            var tempPassword = SMEHelper.GetRandomString(8);
            var user = new User(model.name, model.email, tempPassword, model.role);
            var id = await this._userService.Add(user);

            var token = await this._authorizationService.CreatePasswordResetHmacCode(model.email, true);

            if (String.IsNullOrEmpty(token))
                return new ApiResult(new BadRequestApiResponse("Ocorreu um erro ao tentar recuperar sua senha"));

            var urlEncodedToken = WebUtility.UrlEncode(token);

            var urlRecoverPassword = $"{this._configuration["Frontend:Url"]}/recuperar-senha/{urlEncodedToken}";
            _ = this._mailService.SendChangePasswordEmail(user, tempPassword, urlRecoverPassword);

            return new ApiResult(new Saida((int)HttpStatusCode.OK, true, "Cadastro realizado com sucesso", new { id = id, user = new UserResponse(user) }));
        }

        [Authorize(Roles = "admin")]
        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            // Busca o usuário
            var user = await this._userService.Get(id, true);

            if (user == null)
                return new ApiResult(new BadRequestApiResponse("Usuário não encontrado"));

            user.Disable();

            await this._userService.Update(user);

            return new ApiResult(new Saida((int)HttpStatusCode.OK, true, "Usuário removido com sucesso", new { id }));
        }

        [Authorize(Roles = "admin,cooperativa,logistica")]
        [HttpGet()]
        [Route("{id:guid}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var user = new UserResponse(await this._userService.Get(id, false));

            if (user != null)
            {
                return new ApiResult(new Saida((int)HttpStatusCode.OK, true, String.Empty, user));
            }

            return new ApiResult(new NoContentApiResponse());
        }

        [Authorize(Roles = "admin")]
        [HttpGet()]
        public async Task<IActionResult> GetAll()
        {
            var users = (await this._userService.GetAll()).ConvertAll(u => new UserResponse(u));

            return new ApiResult(new Saida((int)HttpStatusCode.OK, true, String.Empty, users));
        }

        [Authorize(Roles = "admin,cooperativa,logistica")]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UserUpdate model)
        {
            var userIdentity = (System.Security.Claims.ClaimsIdentity)User?.Identity;

            if (userIdentity == null)
                return new ApiResult(new BadRequestApiResponse("Usuário não foi encontrado - 401"));

            var loggedUserId = await this._authorizationService.GetLoggedUserId(userIdentity);

            if (loggedUserId == null)
                return new ApiResult(new BadRequestApiResponse("Usuário não foi encontrado - 403"));

            // Busca o usuário
            var user = await this._userService.Get(model.id, true);

            if (user == null)
                return new ApiResult(new BadRequestApiResponse("Usuário não encontrado"));

            var changedEmail = (user.email != model.email);

            var updatedUser = new User(model.name, model.email, user.password, user.role, model.is_active);
            updatedUser.SetId(model.id);

            await this._userService.Update(updatedUser);

            // Caso tenha mudado de e-mail
            if (changedEmail)
            {
                // Valida que o e-mail não está cadastrado
                var existingUser = await this._userService.Get(model.email);

                if (existingUser != null && existingUser.id != user.id)
                    return new ApiResult(new BadRequestApiResponse("Este e-mail já está atribuído a outro usuário"));

                var cooperativa = await this._cooperativeService.GetByUserId(loggedUserId.Value);

                if (cooperativa != null)
                {
                    cooperativa.SetToAwaitingEmailConfirmation();
                    await this._cooperativeService.Update(cooperativa);

                    var token = await this._cooperativeService.CreateRegisterHmacCode(cooperativa.cnpj);
                    var urlEncodedToken = WebUtility.UrlEncode(token);

                    var urlContinueRegistration = $"{this._configuration["Frontend:Url"]}/cooperativa/confirme-email/{urlEncodedToken}";
                    _ = this._mailService.SendConfirmRegistrationEmail(updatedUser, urlContinueRegistration);
                }
            }

            return new ApiResult(new Saida((int)HttpStatusCode.OK, true, "Usuário atualizado com sucesso", new UserResponse(updatedUser)));
        }
    }
}
