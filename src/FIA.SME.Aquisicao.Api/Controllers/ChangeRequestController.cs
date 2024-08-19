using FIA.SME.Aquisicao.Api.Models;
using FIA.SME.Aquisicao.Application.Services;
using FIA.SME.Aquisicao.Core.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FIA.SME.Aquisicao.Api.Controllers
{
    [Route("api/solicitacao-alteracao")]
    [ApiController]
    public class ChangeRequestController : ControllerBase
    {
        private readonly IChangeRequestService _changeRequestService;

        public ChangeRequestController(IChangeRequestService changeRequestService)
        {
            this._changeRequestService = changeRequestService;
        }

        [Authorize(Roles = "admin")]
        [HttpGet()]
        public async Task<IActionResult> GetAllActive()
        {
            var changeRequests = (await this._changeRequestService.GetAllActive());

            if (changeRequests.Count== 0)
                return new ApiResult(new Saida((int)HttpStatusCode.OK, true, "Nenhuma solicitação de alteração foi encontrada", new List<int>()));

            var changeRequestsGrouped = new ChangeRequestGroupedResponse(changeRequests);

            return new ApiResult(new Saida((int)HttpStatusCode.OK, true, String.Empty, changeRequestsGrouped));
        }
    }
}
