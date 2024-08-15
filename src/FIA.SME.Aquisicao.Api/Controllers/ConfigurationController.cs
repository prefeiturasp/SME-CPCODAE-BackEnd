using FIA.SME.Aquisicao.Api.Models;
using FIA.SME.Aquisicao.Application.Services;
using FIA.SME.Aquisicao.Core.Domain;
using FIA.SME.Aquisicao.Infrastructure.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FIA.SME.Aquisicao.Api.Controllers
{
    [Route("api/configuracoes")]
    [ApiController]
    public class ConfigurationController : ControllerBase
    {
        private readonly IConfigurationService _configurationService;

        public ConfigurationController(IConfigurationService configurationService)
        {
            this._configurationService = configurationService;
        }

        [Authorize(Roles = "admin")]
        [HttpGet()]
        public async Task<IActionResult> GetAll()
        {
            var configurations = await this._configurationService.GetAll();
            var response = configurations.ConvertAll(c => new ConfigurationResponse(c));

            return new ApiResult(new Saida((int)HttpStatusCode.OK, true, String.Empty, response));
        }

        [Authorize(Roles = "admin,cooperativa")]
        [HttpGet("maximum-year-supplied-value")]
        public async Task<IActionResult> GetMaximumYearSuppliedValue()
        {
            var maximumYearSuppliedValue = await this._configurationService.GetMaximumYearSuppliedValue();
            var response = new ConfigurationMaximumYearSuppliedValueResponseModel(maximumYearSuppliedValue);

            if (response != null)
            {
                return new ApiResult(new Saida((int)HttpStatusCode.OK, true, String.Empty, response));
            }

            return new ApiResult(new NoContentApiResponse());
        }

        [Authorize(Roles = "admin")]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] ConfigurationUpdate model)
        {
            // Busca a configuração
            var configuration = await this._configurationService.Get(model.id, true);

            if (configuration == null)
                return new ApiResult(new BadRequestApiResponse("Configuração não encontrada"));

            var updatedConfiguration = new Configuration(model.id, model.name, model.value, configuration.is_active);
            await this._configurationService.Update(updatedConfiguration);

            return new ApiResult(new Saida((int)HttpStatusCode.OK, true, "Configuração atualizada com sucesso", new ConfigurationResponse(updatedConfiguration)));
        }
    }
}
