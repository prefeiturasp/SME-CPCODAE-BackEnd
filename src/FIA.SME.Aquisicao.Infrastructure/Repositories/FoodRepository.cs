using FIA.SME.Aquisicao.Infrastructure.Models;
using FIA.SME.Aquisicao.Infrastructure.Repositories.Context;
using FIA.SME.Aquisicao.Infrastructure.Interfaces;
using FIA.SME.Aquisicao.Infrastructure.Repositories.Types;
using Microsoft.EntityFrameworkCore;

namespace FIA.SME.Aquisicao.Infrastructure.Repositories
{
    public interface IFoodRepository : IRepository
    {
        Task<Food?> Get(Guid id, bool keepTrack);
        Task<List<Food>> GetAll();
        Task Save(Food food);
    }

    internal class FoodRepository : IFoodRepository
    {
        #region [ Propriedades ]

        private readonly SMEContext _context;
        public IUnitOfWork UnitOfWork => _context;

        #endregion [ FIM - Propriedades ]

        #region [ Construtores ]

        public FoodRepository(SMEContext context)
        {
            this._context = context;
        }

        #endregion [ FIM - Construtores ]

        #region [ Metodos ]

        public async Task<Food?> Get(Guid id, bool keepTrack)
        {
            var query = this._context.Alimento!.Where(d => d.id == id);

            if (!keepTrack)
                query = query.AsNoTracking();

            var food = await query.FirstOrDefaultAsync();

            return (food != null) ? new Food(food) : null;
        }

        public async Task<List<Food>> GetAll()
        {
            return await this._context.Alimento.AsNoTracking().Select(d => new Food(d)).ToListAsync();
        }

        public async Task Save(Food food)
        {
            var toSave = await this._context.Alimento.FirstOrDefaultAsync(d => d.id == food.id);

            if (toSave == null)
            {
                toSave = new Alimento();
                this._context.Alimento.Add(toSave);
            }

            toSave.id = food.id;
            toSave.categoria_id = food.category_id;
            toSave.unidade_medida = food.measure_unit;
            toSave.nome = food.name;
            toSave.ativo = food.is_active;
        }

        public void Dispose()
        {
            this._context.Dispose();
        }

        #endregion [ FIM - Metodos ]
    }
}
