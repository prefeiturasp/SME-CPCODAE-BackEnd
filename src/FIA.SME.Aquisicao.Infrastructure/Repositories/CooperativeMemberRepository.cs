using FIA.SME.Aquisicao.Core.Helpers;
using FIA.SME.Aquisicao.Infrastructure.Models;
using FIA.SME.Aquisicao.Infrastructure.Repositories.Context;
using FIA.SME.Aquisicao.Infrastructure.Interfaces;
using FIA.SME.Aquisicao.Infrastructure.Repositories.Types;
using Microsoft.EntityFrameworkCore;

namespace FIA.SME.Aquisicao.Infrastructure.Repositories
{
    public interface ICooperativeMemberRepository : IRepository
    {
        Task<CooperativeMember?> Get(Guid id, bool keepTrack);
        Task<List<CooperativeMember>> GetAllByCooperative(Guid cooperative_id);
        Task<List<CooperativeMember>> GetAllByDapCaf(List<string> dap_caf_list);
        Task<CooperativeMember?> GetByCpf(string cpf);
        Task<CooperativeMember?> GetByDapCafCpf(string dap_caf_cpf);
        Task Remove(CooperativeMember cooperativeMember);
        Task Save(CooperativeMember cooperativeMember);
        Task TryAdd(CooperativeMember cooperativeMember);
    }

    internal class CooperativeMemberRepository : ICooperativeMemberRepository
    {
        #region [ Propriedades ]

        private readonly SMEContext _context;
        public IUnitOfWork UnitOfWork => _context;

        #endregion [ FIM - Propriedades ]

        #region [ Construtores ]

        public CooperativeMemberRepository(SMEContext context)
        {
            this._context = context;
        }

        #endregion [ FIM - Construtores ]

        #region [ Metodos ]

        public async Task<CooperativeMember?> Get(Guid id, bool keepTrack)
        {
            var query = this._context.Cooperado!.Include(c => c.Endereco).Where(c => c.id == id);

            if (!keepTrack)
                query = query.AsNoTracking();

            var cooperativeMember = await query.FirstOrDefaultAsync();

            return (cooperativeMember != null) ? new CooperativeMember(cooperativeMember) : null;
        }

        public async Task<List<CooperativeMember>> GetAllByCooperative(Guid cooperative_id)
        {
            return await this._context.Cooperado.Where(c => c.cooperativa_id == cooperative_id).AsNoTracking().Select(c => new CooperativeMember(c)).ToListAsync();
        }

        public async Task<List<CooperativeMember>> GetAllByDapCaf(List<string> dap_caf_list)
        {
            return await this._context.Cooperado.Where(c => dap_caf_list.Contains(c.codigo_dap_caf)).AsNoTracking().Select(c => new CooperativeMember(c)).ToListAsync();
        }

        public async Task<CooperativeMember?> GetByCpf(string cpf)
        {
            var cooperativeMember = await this._context.Cooperado!.Include(c => c.Endereco).Where(c => c.cpf == cpf).AsNoTracking().FirstOrDefaultAsync();

            return (cooperativeMember != null) ? new CooperativeMember(cooperativeMember) : null;
        }

        public async Task<CooperativeMember?> GetByDapCafCpf(string dap_caf_cpf)
        {
            var cpf = dap_caf_cpf.ToOnlyNumbers();
            var cooperativeMember = await this._context.Cooperado!.Include(c => c.Endereco).Where(c => c.codigo_dap_caf.ToLower() == dap_caf_cpf.ToLower() || c.cpf == cpf).AsNoTracking().FirstOrDefaultAsync();

            return (cooperativeMember != null) ? new CooperativeMember(cooperativeMember) : null;
        }

        public async Task Remove(CooperativeMember cooperativeMember)
        {
            var toRemove = await this._context.Cooperado.FirstOrDefaultAsync(c => c.id == cooperativeMember.id);

            if (toRemove != null)
            {
                this._context.Cooperado.Remove(toRemove);
            }
        }

        public async Task Save(CooperativeMember cooperativeMember)
        {
            var toSave = await this._context.Cooperado.FirstOrDefaultAsync(c => c.id == cooperativeMember.id);

            if (toSave == null)
            {
                toSave = new Cooperado();
                this._context.Cooperado.Add(toSave);
            }

            toSave.id = cooperativeMember.id;
            toSave.cooperativa_id = cooperativeMember.cooperative_id!.Value;
            toSave.endereco_id = cooperativeMember.address_id;
            toSave.codigo_dap_caf = cooperativeMember.dap_caf_code;
            toSave.cpf = cooperativeMember.cpf;
            toSave.nome = cooperativeMember.name;
            toSave.tipo_agricultor_familiar = cooperativeMember.pf_type.HasValue ? (int)cooperativeMember.pf_type : null;
            toSave.tipo_producao = cooperativeMember.production_type.HasValue ? (int)cooperativeMember.production_type : null;
            toSave.data_emissao_dap_caf = cooperativeMember.dap_caf_registration_date.SetKindUtc();
            toSave.data_validade_dap_caf = cooperativeMember.dap_caf_expiration_date.SetKindUtc();
            toSave.is_dap = cooperativeMember.is_dap;
            toSave.is_masculino = cooperativeMember.is_male;
            toSave.ativo = cooperativeMember.is_active;
        }

        public async Task TryAdd(CooperativeMember cooperativeMember)
        {
            var member = await GetByDapCafCpf(cooperativeMember.dap_caf_code);

            if (member != null && member.cooperative_id == cooperativeMember.cooperative_id)
                return;

            await Save(cooperativeMember);
        }

        public void Dispose()
        {
            this._context.Dispose();
        }

        #endregion [ FIM - Metodos ]
    }
}
