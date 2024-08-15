using FIA.SME.Aquisicao.Core.Helpers;
using FIA.SME.Aquisicao.Infrastructure.Models;
using FIA.SME.Aquisicao.Infrastructure.Repositories.Context;
using FIA.SME.Aquisicao.Infrastructure.Interfaces;
using FIA.SME.Aquisicao.Infrastructure.Repositories.Types;
using Microsoft.EntityFrameworkCore;

namespace FIA.SME.Aquisicao.Infrastructure.Repositories
{
    public interface ICooperativeRepository : IRepository
    {
        Task<Cooperative?> Get(Guid id, bool keepTrack);
        Task<List<Cooperative>> GetAll();
        Task<Cooperative?> GetByCnpj(string cnpj);
        Task<Cooperative?> GetByDapCaf(string dap_caf_code);
        Task<Cooperative?> GetByUserId(Guid userId);
        Task Save(Cooperative cooperative);
    }

    internal class CooperativeRepository : ICooperativeRepository
    {
        #region [ Propriedades ]

        private readonly SMEContext _context;
        public IUnitOfWork UnitOfWork => _context;

        #endregion [ FIM - Propriedades ]

        #region [ Construtores ]

        public CooperativeRepository(SMEContext context)
        {
            this._context = context;
        }

        #endregion [ FIM - Construtores ]

        #region [ Metodos ]

        public async Task<Cooperative?> Get(Guid id, bool keepTrack)
        {
            var query = this._context.Cooperativa!
                                        .Include(c => c.Endereco)
                                        .Include(c => c.Banco)
                                        .Include(c => c.RepresentanteLegal.Endereco)
                                        .Include(c => c.Cooperados.OrderBy(c => c.nome))
                                        .Include(c => c.Documentos.OrderBy(d => d.documento_path))
                                        .Where(c => c.id == id);

            if (!keepTrack)
                query = query.AsNoTracking();

            var cooperative = await query.FirstOrDefaultAsync();

            return (cooperative != null) ? new Cooperative(cooperative) : null;
        }

        public async Task<List<Cooperative>> GetAll()
        {
            return await this._context.Cooperativa.AsNoTracking().Select(c => new Cooperative(c)).ToListAsync();
        }

        public async Task<Cooperative?> GetByCnpj(string cnpj)
        {
            var cooperative = await this._context.Cooperativa!.Where(c => c.cnpj == cnpj).AsNoTracking().FirstOrDefaultAsync();

            return (cooperative != null) ? new Cooperative(cooperative) : null;
        }

        public async Task<Cooperative?> GetByDapCaf(string dap_caf_code)
        {
            var cooperative = await this._context.Cooperativa!.Where(c => c.codigo_dap_caf == dap_caf_code).AsNoTracking().FirstOrDefaultAsync();

            return (cooperative != null) ? new Cooperative(cooperative) : null;
        }

        public async Task<Cooperative?> GetByUserId(Guid userId)
        {
            var cooperative = await this._context.Cooperativa!.Where(c => c.usuario_id == userId).AsNoTracking().FirstOrDefaultAsync();

            return (cooperative != null) ? new Cooperative(cooperative) : null;
        }

        public async Task Save(Cooperative cooperative)
        {
            var toSave = await this._context.Cooperativa.FirstOrDefaultAsync(c => c.id == cooperative.id);

            if (toSave == null)
            {
                toSave = new Cooperativa();
                toSave.data_criacao = toSave.data_aceite_termos_uso = DateTime.UtcNow;
                this._context.Cooperativa.Add(toSave);
            }

            toSave.id = cooperative.id;
            toSave.endereco_id = cooperative.address_id;
            toSave.banco_id = cooperative.bank_id;
            toSave.representante_legal_id = cooperative.legal_representative_id;
            toSave.usuario_id = cooperative.user_id;
            toSave.cnpj = cooperative.cnpj.Trim();
            toSave.cnpj_central = cooperative.cnpj_central?.Trim();
            toSave.codigo_dap_caf = cooperative.dap_caf_code.Trim();
            toSave.email = cooperative.email?.ToLower().Trim();
            toSave.logo = cooperative.logo;
            toSave.ip_aceite_termos_uso = cooperative.terms_use_acceptance_ip.Trim();
            toSave.razao_social = cooperative.name?.Trim();
            toSave.sigla = cooperative.acronym?.Trim();
            toSave.telefone = cooperative.phone?.Trim();
            toSave.situacao = (int)cooperative.status;
            toSave.tipo_pessoa_juridica = (int)cooperative.pj_type;
            toSave.tipo_producao = (int)cooperative.production_type;
            toSave.data_emissao_dap_caf = cooperative.dap_caf_registration_date.SetKindUtc();
            toSave.data_validade_dap_caf = cooperative.dap_caf_expiration_date.SetKindUtc();
            toSave.is_dap = cooperative.is_dap;
            toSave.ativa = cooperative.is_active;
        }

        public void Dispose()
        {
            this._context.Dispose();
        }

        #endregion [ FIM - Metodos ]
    }
}
