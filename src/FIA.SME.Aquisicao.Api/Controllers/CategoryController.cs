using FIA.SME.Aquisicao.Api.Models;
using FIA.SME.Aquisicao.Application.Services;
using FIA.SME.Aquisicao.Core.Domain;
using FIA.SME.Aquisicao.Infrastructure.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FIA.SME.Aquisicao.Api.Controllers
{
    [Route("api/categoria-alimento")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IConfiguration _configuration;

        public CategoryController(
            ICategoryService categoryService,
            IConfiguration configuration)
        {
            this._categoryService = categoryService;
            this._configuration = configuration;
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CategoryRegister model)
        {
            var category = new Category(model.name);
            var id = await this._categoryService.Add(category);

            return new ApiResult(new Saida((int)HttpStatusCode.OK, true, "Cadastro realizado com sucesso", new { id = id, category = new CategoryResponse(category) }));
        }

        [Authorize(Roles = "admin")]
        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            // Busca a categoria
            var category = await this._categoryService.Get(id, true);

            if (category == null)
                return new ApiResult(new BadRequestApiResponse("Categoria não encontrada"));

            category.Disable();

            await this._categoryService.Update(category);

            return new ApiResult(new Saida((int)HttpStatusCode.OK, true, "Categoria removida com sucesso", new { id }));
        }

        [Authorize(Roles = "admin")]
        [HttpGet()]
        [Route("{id:guid}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var category = new CategoryResponse(await this._categoryService.Get(id, false));

            if (category != null)
            {
                return new ApiResult(new Saida((int)HttpStatusCode.OK, true, String.Empty, category));
            }

            return new ApiResult(new NoContentApiResponse());
        }

        [Authorize(Roles = "admin")]
        [HttpGet()]
        public async Task<IActionResult> GetAll()
        {
            var categories = (await this._categoryService.GetAll()).ConvertAll(d => new CategoryResponse(d));

            return new ApiResult(new Saida((int)HttpStatusCode.OK, true, String.Empty, categories));
        }

        [Authorize(Roles = "admin")]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] CategoryUpdate model)
        {
            // Busca a categoria
            var category = await this._categoryService.Get(model.id, true);

            if (category == null)
                return new ApiResult(new BadRequestApiResponse("Categoria não encontrada"));

            var updatedCategory = new Category(model.id, model.name, model.is_active);
            await this._categoryService.Update(updatedCategory);

            return new ApiResult(new Saida((int)HttpStatusCode.OK, true, "Categoria atualizada com sucesso", new CategoryResponse(updatedCategory)));
        }
    }
}
