using FIA.SME.Aquisicao.Infrastructure.Models;
using FIA.SME.Aquisicao.Infrastructure.Repositories;

namespace FIA.SME.Aquisicao.Application.Services
{
    public interface IFoodService
    {
        Task<Guid> Add(Food food);
        Task<Food?> Get(Guid id, bool keepTrack);
        Task<List<Food>> GetAll();
        Task Update(Food food);
    }

    internal class FoodService : IFoodService
    {
        #region [ Propriedades ]

        private readonly IFoodRepository _foodRepository;

        #endregion [ FIM - Propriedades ]

        #region [ Construtores ]

        public FoodService(IFoodRepository foodRepository)
        {
            this._foodRepository = foodRepository;
        }

        #endregion [ FIM - Construtores ]

        #region [ Metodos ]

        public async Task<Guid> Add(Food food)
        {
            var food_id = Guid.NewGuid();
            food.SetId(food_id);

            await this._foodRepository.Save(food);
            await this._foodRepository.UnitOfWork.Commit();

            return food_id;
        }

        public async Task<Food?> Get(Guid id, bool keepTrack)
        {
           return await this._foodRepository.Get(id, keepTrack);
        }

        public async Task<List<Food>> GetAll()
        {
            return await this._foodRepository.GetAll();
        }

        public async Task Update(Food food)
        {
            await this._foodRepository.Save(food);
            await this._foodRepository.UnitOfWork.Commit();
        }

        #endregion [ FIM - Metodos ]
    }
}
