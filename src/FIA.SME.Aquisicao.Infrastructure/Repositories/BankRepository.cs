using FIA.SME.Aquisicao.Infrastructure.Models;
using FIA.SME.Aquisicao.Infrastructure.Repositories.Context;
using FIA.SME.Aquisicao.Infrastructure.Interfaces;
using FIA.SME.Aquisicao.Infrastructure.Repositories.Types;
using Microsoft.EntityFrameworkCore;

namespace FIA.SME.Aquisicao.Infrastructure.Repositories
{
    public interface IBankRepository : IRepository
    {
        Task<Bank?> Get(Guid id, bool keepTrack);
        Task<List<Bank>> GetAll();
        Task Save(Bank bank);
    }

    internal class BankRepository : IBankRepository
    {
        #region [ Propriedades ]

        private readonly SMEContext _context;
        public IUnitOfWork UnitOfWork => _context;

        #endregion [ FIM - Propriedades ]

        #region [ Construtores ]

        public BankRepository(SMEContext context)
        {
            this._context = context;
        }

        #endregion [ FIM - Construtores ]

        #region [ Metodos ]

        public async Task<Bank?> Get(Guid id, bool keepTrack)
        {
            var query = this._context.Banco!.Where(d => d.id == id);

            if (!keepTrack)
                query = query.AsNoTracking();

            var bank = await query.FirstOrDefaultAsync();

            return (bank != null) ? new Bank(bank) : null;
        }

        public async Task<List<Bank>> GetAll()
        {
            return await this._context.Banco.AsNoTracking().Select(d => new Bank(d)).ToListAsync();
        }

        public async Task Save(Bank bank)
        {
            var toSave = await this._context.Banco.FirstOrDefaultAsync(d => d.id == bank.id);

            if (toSave == null)
            {
                toSave = new Banco();
                this._context.Banco.Add(toSave);
            }

            toSave.id = bank.id;
            toSave.agencia = bank.agency;
            toSave.codigo = bank.code;
            toSave.nome = bank.name;
            toSave.numero_conta = bank.account_number;
        }

        public void Dispose()
        {
            this._context.Dispose();
        }

        #endregion [ FIM - Metodos ]
    }
}
