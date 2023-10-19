using FIA.SME.Aquisicao.Infrastructure.Models;
using FIA.SME.Aquisicao.Infrastructure.Repositories.Context;
using FIA.SME.Aquisicao.Infrastructure.Interfaces;
using FIA.SME.Aquisicao.Infrastructure.Repositories.Types;
using Microsoft.EntityFrameworkCore;
using FIA.SME.Aquisicao.Core.Helpers;

namespace FIA.SME.Aquisicao.Infrastructure.Repositories
{
    public interface IPublicCallFoodRepository : IRepository
    {
        Task RemoveAllNotInIdList(Guid publicCallId, List<Guid> publicCallFoodIds);
        Task Save(PublicCallFood publicCallFood);
    }

    internal class PublicCallFoodRepository : IPublicCallFoodRepository
    {
        #region [ Propriedades ]

        private readonly SMEContext _context;
        public IUnitOfWork UnitOfWork => _context;

        #endregion [ FIM - Propriedades ]

        #region [ Construtores ]

        public PublicCallFoodRepository(SMEContext context)
        {
            this._context = context;
        }

        #endregion [ FIM - Construtores ]

        #region [ Metodos ]

        public Task RemoveAllNotInIdList(Guid publicCallId, List<Guid> publicCallFoodIds)
        {
            var foods = this._context.ChamadaPublicaAlimento.Where(dc => dc.chamada_publica_id == publicCallId && !publicCallFoodIds.Contains(dc.id));

            this._context.ChamadaPublicaAlimento.RemoveRange(foods);

            return Task.CompletedTask;
        }

        public async Task Save(PublicCallFood publicCallFood)
        {
            var toSave = await this._context.ChamadaPublicaAlimento.FirstOrDefaultAsync(c => c.id == publicCallFood.id);

            if (toSave == null)
            {
                toSave = new ChamadaPublicaAlimento();
                this._context.ChamadaPublicaAlimento.Add(toSave);
            }

            toSave.id = publicCallFood.id;
            toSave.alimento_id = publicCallFood.food_id;
            toSave.chamada_publica_id = publicCallFood.public_call_id;
            toSave.preco = publicCallFood.price;
            toSave.quantidade = publicCallFood.quantity;
            toSave.data_criacao = publicCallFood.creation_date.SetKindUtc();
            toSave.aceita_organico = publicCallFood.accepts_organic;
            toSave.organico = publicCallFood.is_organic;
            toSave.ativo = publicCallFood.is_active;
        }

        public void Dispose()
        {
            this._context.Dispose();
        }

        #endregion [ FIM - Metodos ]
    }
}
