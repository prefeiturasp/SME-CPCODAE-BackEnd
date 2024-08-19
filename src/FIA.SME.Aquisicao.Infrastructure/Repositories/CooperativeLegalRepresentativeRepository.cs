using FIA.SME.Aquisicao.Infrastructure.Models;
using FIA.SME.Aquisicao.Infrastructure.Repositories.Context;
using FIA.SME.Aquisicao.Infrastructure.Interfaces;
using FIA.SME.Aquisicao.Infrastructure.Repositories.Types;
using Microsoft.EntityFrameworkCore;
using FIA.SME.Aquisicao.Core.Helpers;

namespace FIA.SME.Aquisicao.Infrastructure.Repositories
{
    public interface ICooperativeLegalRepresentativeRepository : IRepository
    {
        Task<CooperativeLegalRepresentative?> Get(Guid id, bool keepTrack);
        Task<List<CooperativeLegalRepresentative>> GetAll();
        Task Save(CooperativeLegalRepresentative legalRepresentative);
    }

    internal class CooperativeLegalRepresentativeRepository : ICooperativeLegalRepresentativeRepository
    {
        #region [ Propriedades ]

        private readonly SMEContext _context;
        public IUnitOfWork UnitOfWork => _context;

        #endregion [ FIM - Propriedades ]

        #region [ Construtores ]

        public CooperativeLegalRepresentativeRepository(SMEContext context)
        {
            this._context = context;
        }

        #endregion [ FIM - Construtores ]

        #region [ Metodos ]

        public async Task<CooperativeLegalRepresentative?> Get(Guid id, bool keepTrack)
        {
            var query = this._context.RepresentanteLegal!.Where(d => d.id == id);

            if (!keepTrack)
                query = query.AsNoTracking();

            var legalRepresentative = await query.FirstOrDefaultAsync();

            return (legalRepresentative != null) ? new CooperativeLegalRepresentative(legalRepresentative) : null;
        }

        public async Task<List<CooperativeLegalRepresentative>> GetAll()
        {
            return await this._context.RepresentanteLegal.AsNoTracking().Select(d => new CooperativeLegalRepresentative(d)).ToListAsync();
        }

        public async Task Save(CooperativeLegalRepresentative legalRepresentative)
        {
            var toSave = await this._context.RepresentanteLegal.FirstOrDefaultAsync(d => d.id == legalRepresentative.id);

            if (toSave == null)
            {
                toSave = new RepresentanteLegal();
                this._context.RepresentanteLegal.Add(toSave);
            }

            toSave.id = legalRepresentative.id;
            toSave.endereco_id = legalRepresentative.address_id;
            toSave.cpf = legalRepresentative.cpf.Trim();
            toSave.nome = legalRepresentative.name.Trim();
            toSave.telefone = legalRepresentative.phone.Trim();
            toSave.estado_civil = (int)legalRepresentative.marital_status;
            toSave.cargo = legalRepresentative.position;
            toSave.data_vigencia_mandato = legalRepresentative.position_expiration_date?.SetKindUtc();
        }

        public void Dispose()
        {
            this._context.Dispose();
        }

        #endregion [ FIM - Metodos ]
    }
}
