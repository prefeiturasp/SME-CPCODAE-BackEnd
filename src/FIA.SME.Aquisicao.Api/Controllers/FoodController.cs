using FIA.SME.Aquisicao.Api.Models;
using FIA.SME.Aquisicao.Application.Services;
using FIA.SME.Aquisicao.Core.Domain;
using FIA.SME.Aquisicao.Core.Enums;
using FIA.SME.Aquisicao.Infrastructure.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FIA.SME.Aquisicao.Api.Controllers
{
    [Route("api/alimento")]
    [ApiController]
    public class FoodController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IFoodService _foodService;
        private readonly IConfiguration _configuration;

        public FoodController(
            ICategoryService categoryService,
            IFoodService foodService,
            IConfiguration configuration)
        {
            this._categoryService = categoryService;
            this._foodService = foodService;
            this._configuration = configuration;
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] FoodRegister model)
        {
            // Busca a categoria
            var category = await this._categoryService.Get(model.category_id, false);

            if (category == null)
                return new ApiResult(new BadRequestApiResponse("Categoria não encontrada"));

            var food = new Food(model.category_id, model.name, (MeasureUnitEnum)model.measure_unit);
            var id = await this._foodService.Add(food);

            return new ApiResult(new Saida((int)HttpStatusCode.OK, true, "Cadastro realizado com sucesso", new { id = id, food = new FoodResponse(food) }));
        }

        [Authorize(Roles = "admin")]
        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            // Busca o alimento
            var food = await this._foodService.Get(id, true);

            if (food == null)
                return new ApiResult(new BadRequestApiResponse("Alimento não encontrado"));

            food.Disable();

            await this._foodService.Update(food);

            return new ApiResult(new Saida((int)HttpStatusCode.OK, true, "Alimento removido com sucesso", new { id }));
        }

        [Authorize(Roles = "admin")]
        [HttpGet()]
        [Route("{id:guid}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var food = new FoodResponse(await this._foodService.Get(id, false));

            if (food != null)
            {
                return new ApiResult(new Saida((int)HttpStatusCode.OK, true, String.Empty, food));
            }

            return new ApiResult(new NoContentApiResponse());
        }

        [Authorize(Roles = "admin")]
        [HttpGet()]
        public async Task<IActionResult> GetAll()
        {
            var foods = (await this._foodService.GetAll()).ConvertAll(d => new FoodResponse(d));

            return new ApiResult(new Saida((int)HttpStatusCode.OK, true, String.Empty, foods));
        }

        [Authorize(Roles = "admin")]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] FoodUpdate model)
        {
            // Busca o alimento
            var food = await this._foodService.Get(model.id, true);

            if (food == null)
                return new ApiResult(new BadRequestApiResponse("Alimento não encontrado"));

            // Busca a categoria
            var category = await this._categoryService.Get(model.category_id, false);

            if (category == null)
                return new ApiResult(new BadRequestApiResponse("Categoria não encontrada"));

            var updatedFood = new Food(model.id, model.category_id, model.name, (MeasureUnitEnum)model.measure_unit, model.is_active);
            await this._foodService.Update(updatedFood);

            return new ApiResult(new Saida((int)HttpStatusCode.OK, true, "Alimento atualizado com sucesso", new FoodResponse(updatedFood)));
        }
    }
}
