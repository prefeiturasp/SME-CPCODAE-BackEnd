using FIA.SME.Aquisicao.Api.Models;
using FIA.SME.Aquisicao.Application.Services;
using FIA.SME.Aquisicao.Core.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FIA.SME.Aquisicao.Api.Controllers
{
    [Route("api/fale-conosco")]
    [ApiController]
    public class FaleConoscoController : ControllerBase
    {
        private readonly ICooperativeService _cooperativeService;
        private readonly IFaleConoscoService _faleConoscoService;
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;

        public FaleConoscoController(
            ICooperativeService cooperativeService,
            IFaleConoscoService faleConoscoService,
            IUserService userService,
            IConfiguration configuration)
        {
            this._cooperativeService = cooperativeService;
            this._faleConoscoService = faleConoscoService;
            this._userService = userService;
            this._configuration = configuration;
        }

        [Authorize(Roles = "cooperativa")]
        [HttpPost("send-message")]
        public async Task<IActionResult> Send([FromBody] FaleConoscoModel model)
        {
            // Busca a cooperativa
            var cooperative = await this._cooperativeService.Get(model.cooperative_id);

            if (cooperative == null)
                return new ApiResult(new BadRequestApiResponse("Cooperativa não encontrada"));

            // Busca o usuário
            var user = await this._userService.Get(model.user_id, true);

            if (user == null)
                return new ApiResult(new BadRequestApiResponse("Usuário não encontrado"));

            var faleConosco = new FaleConosco(model.title, model.message, user.name, cooperative.name, user.email, cooperative.email);
            await this._faleConoscoService.Send(faleConosco);

            return new ApiResult(new Saida((int)HttpStatusCode.OK, true, "Mensagem enviada com sucesso", true));
        }
    }
}
