using FIA.SME.Aquisicao.Api.Extensions;
using FIA.SME.Aquisicao.Api.Models;
using FIA.SME.Aquisicao.Application.Services;
using FIA.SME.Aquisicao.Core.Domain;
using FIA.SME.Aquisicao.Infrastructure.Models;
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
        private readonly IPublicCallService _publicCallService;
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;

        public FaleConoscoController(
            ICooperativeService cooperativeService,
            IFaleConoscoService faleConoscoService,
            IPublicCallService publicCallService,
            IUserService userService,
            IConfiguration configuration)
        {
            this._cooperativeService = cooperativeService;
            this._faleConoscoService = faleConoscoService;
            this._publicCallService = publicCallService;
            this._userService = userService;
            this._configuration = configuration;
        }

        [Authorize(Roles = "admin")]
        [HttpGet("{contact_id}")]
        public async Task<IActionResult> Get(Guid contact_id)
        {
            var contact = await this._faleConoscoService.Get(contact_id, false);

            if (contact == null)
                return new ApiResult(new BadRequestApiResponse("Mensagem Fale Conosco não encontrada"));

            var response = contact.ToResponseModel();

            return new ApiResult(new Saida((int)HttpStatusCode.OK, true, String.Empty, response));
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] FaleConoscoGetAllRequestModel request)
        {
            var contacts = await this._faleConoscoService.GetAll(request.cooperative_id, request.public_call_id, request.start_date, request.end_date);

            var response = contacts.ConvertAll(fc => fc.ToListResponseModel());

            return new ApiResult(new Saida((int)HttpStatusCode.OK, true, String.Empty, response));
        }

        [Authorize(Roles = "cooperativa")]
        [HttpGet("public-calls/{cooperative_id}")]
        public async Task<IActionResult> GetAllPublicCalls(Guid cooperative_id)
        {
            var allPublicCalls = (await this._publicCallService.GetAllOnGoingByCooperative(cooperative_id)).ToList();
            var chamadas = allPublicCalls.ConvertAll(pc => new PublicCallListResponse(pc));

            return new ApiResult(new Saida((int)HttpStatusCode.OK, true, String.Empty, chamadas));
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

            // Busca a chamada pública
            var publicCall = await this._publicCallService.Get(model.public_call_id, false);

            if (publicCall == null)
                return new ApiResult(new BadRequestApiResponse("Chamada Pública não encontrada"));

            var contact = new Contact(model.cooperative_id, model.public_call_id, model.title, model.message);
            var success = await this._faleConoscoService.Add(contact);

            if (!success)
                return new ApiResult(new BadRequestApiResponse("Não foi possível criar a mensagem de fale conosco"));

            var faleConosco = new FaleConosco(model.title, model.message, user.name, cooperative.name!, user.email, cooperative.email!, publicCall.name, publicCall.number, publicCall.process);
            await this._faleConoscoService.Send(faleConosco);

            return new ApiResult(new Saida((int)HttpStatusCode.OK, true, "Mensagem enviada com sucesso", true));
        }
    }
}
