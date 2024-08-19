using FIA.SME.Aquisicao.Api.Models;
using FIA.SME.Aquisicao.Application.Services;
using FIA.SME.Aquisicao.Core.Domain;
using FIA.SME.Aquisicao.Infrastructure.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FIA.SME.Aquisicao.Api.Controllers
{
    [Route("api/tipo-documento")]
    [ApiController]
    public class DocumentTypeController : ControllerBase
    {
        private readonly IDocumentTypeService _documentTypeService;
        private readonly IConfiguration _configuration;

        public DocumentTypeController(
            IDocumentTypeService documentTypeService,
            IConfiguration configuration)
        {
            this._documentTypeService = documentTypeService;
            this._configuration = configuration;
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] DocumentTypeRegister model)
        {
            var document = new DocumentType(model.name, model.application);
            var id = await this._documentTypeService.Add(document);

            return new ApiResult(new Saida((int)HttpStatusCode.OK, true, "Cadastro realizado com sucesso", new { id = id, document = new DocumentTypeResponse(document) }));
        }

        [Authorize(Roles = "admin")]
        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            // Busca o tipo de documento
            var document = await this._documentTypeService.Get(id);

            if (document == null)
                return new ApiResult(new BadRequestApiResponse("Tipo de Documento não encontrado"));

            document.Disable();

            await this._documentTypeService.Update(document);

            return new ApiResult(new Saida((int)HttpStatusCode.OK, true, "Tipo de Documento removido com sucesso", new { id }));
        }

        [Authorize(Roles = "admin")]
        [HttpGet()]
        [Route("{id:guid}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var document = new DocumentTypeResponse(await this._documentTypeService.Get(id));

            if (document != null)
            {
                return new ApiResult(new Saida((int)HttpStatusCode.OK, true, String.Empty, document));
            }

            return new ApiResult(new NoContentApiResponse());
        }

        [Authorize(Roles = "admin,cooperativa")]
        [HttpGet()]
        public async Task<IActionResult> GetAll()
        {
            var documents = (await this._documentTypeService.GetAll()).ConvertAll(d => new DocumentTypeResponse(d));

            return new ApiResult(new Saida((int)HttpStatusCode.OK, true, String.Empty, documents));
        }

        [Authorize(Roles = "admin")]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] DocumentTypeUpdate model)
        {
            // Busca o tipo de documento
            var document = await this._documentTypeService.Get(model.id);

            if (document == null)
                return new ApiResult(new BadRequestApiResponse("Tipo de Documento não encontrado"));

            var updatedDocument = new DocumentType(model.id, model.name, model.application, true, model.is_active);
            await this._documentTypeService.Update(updatedDocument);

            return new ApiResult(new Saida((int)HttpStatusCode.OK, true, "Tipo de Documento atualizado com sucesso", new DocumentTypeResponse(updatedDocument)));
        }
    }
}
