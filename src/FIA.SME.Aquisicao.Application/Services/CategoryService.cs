using FIA.SME.Aquisicao.Infrastructure.Models;
using FIA.SME.Aquisicao.Infrastructure.Repositories;

namespace FIA.SME.Aquisicao.Application.Services
{
    public interface ICategoryService
    {
        Task<Guid> Add(Category category);
        Task<Category?> Get(Guid id, bool keepTrack);
        Task<List<Category>> GetAll();
        Task Update(Category category);
    }

    internal class CategoryService : ICategoryService
    {
        #region [ Propriedades ]

        private readonly ICategoryRepository _categoryRepository;

        #endregion [ FIM - Propriedades ]

        #region [ Construtores ]

        public CategoryService(ICategoryRepository categoryRepository)
        {
            this._categoryRepository = categoryRepository;
        }

        #endregion [ FIM - Construtores ]

        #region [ Metodos ]

        public async Task<Guid> Add(Category category)
        {
            var category_id = Guid.NewGuid();
            category.SetId(category_id);

            await this._categoryRepository.Save(category);
            await this._categoryRepository.UnitOfWork.Commit();

            return category_id;
        }

        public async Task<Category?> Get(Guid id, bool keepTrack)
        {
           return await this._categoryRepository.Get(id, keepTrack);
        }

        public async Task<List<Category>> GetAll()
        {
            return await this._categoryRepository.GetAll();
        }

        public async Task Update(Category category)
        {
            await this._categoryRepository.Save(category);
            await this._categoryRepository.UnitOfWork.Commit();
        }

        #endregion [ FIM - Metodos ]
    }
}
