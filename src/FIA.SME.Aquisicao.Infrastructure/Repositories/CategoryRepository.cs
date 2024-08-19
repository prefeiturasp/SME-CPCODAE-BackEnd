using FIA.SME.Aquisicao.Infrastructure.Models;
using FIA.SME.Aquisicao.Infrastructure.Repositories.Context;
using FIA.SME.Aquisicao.Infrastructure.Interfaces;
using FIA.SME.Aquisicao.Infrastructure.Repositories.Types;
using Microsoft.EntityFrameworkCore;

namespace FIA.SME.Aquisicao.Infrastructure.Repositories
{
    public interface ICategoryRepository : IRepository
    {
        Task<Category?> Get(Guid id, bool keepTrack);
        Task<List<Category>> GetAll();
        Task Save(Category category);
    }

    internal class CategoryRepository : ICategoryRepository
    {
        #region [ Propriedades ]

        private readonly SMEContext _context;
        public IUnitOfWork UnitOfWork => _context;

        #endregion [ FIM - Propriedades ]

        #region [ Construtores ]

        public CategoryRepository(SMEContext context)
        {
            this._context = context;
        }

        #endregion [ FIM - Construtores ]

        #region [ Metodos ]

        public async Task<Category?> Get(Guid id, bool keepTrack)
        {
            var query = this._context.Categoria!.Where(d => d.id == id);

            if (!keepTrack)
                query = query.AsNoTracking();

            var category = await query.FirstOrDefaultAsync();

            return (category != null) ? new Category(category) : null;
        }

        public async Task<List<Category>> GetAll()
        {
            return await this._context.Categoria.AsNoTracking().Select(d => new Category(d)).ToListAsync();
        }

        public async Task Save(Category category)
        {
            var toSave = await this._context.Categoria.FirstOrDefaultAsync(d => d.id == category.id);

            if (toSave == null)
            {
                toSave = new Categoria();
                this._context.Categoria.Add(toSave);
            }

            toSave.id = category.id;
            toSave.nome = category.name;
            toSave.ativa = category.is_active;
        }

        public void Dispose()
        {
            this._context.Dispose();
        }

        #endregion [ FIM - Metodos ]
    }
}
