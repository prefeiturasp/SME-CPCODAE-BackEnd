using FIA.SME.Aquisicao.Infrastructure.Models;
using FIA.SME.Aquisicao.Infrastructure.Repositories;

namespace FIA.SME.Aquisicao.Application.Services
{
    public interface ICooperativeMemberService
    {
        Task<Guid> Add(CooperativeMember cooperativeMember, Guid cooperative_id);
        Task<bool> CheckIfCpfExists(Guid? id, string cpf);
        Task<CooperativeMember?> Get(Guid id, bool keepTrack);
        Task<CooperativeMember?> GetByDapCafCpf(string dap_caf_cpf);
        Task Remove(Guid id);
    }

    internal class CooperativeMemberService : ICooperativeMemberService
    {
        #region [ Propriedades ]

        private readonly IAddressRepository _addressRepository;
        private readonly ICooperativeMemberRepository _cooperativeMemberRepository;

        #endregion [ FIM - Propriedades ]

        #region [ Construtores ]

        public CooperativeMemberService(IAddressRepository addressRepository, ICooperativeMemberRepository cooperativeMemberRepository)
        {
            this._addressRepository = addressRepository;
            this._cooperativeMemberRepository = cooperativeMemberRepository;
        }

        #endregion [ FIM - Construtores ]

        #region [ Metodos ]

        public async Task<Guid> Add(CooperativeMember cooperativeMember, Guid cooperative_id)
        {
            cooperativeMember.SetIds(cooperativeMember.id, cooperative_id);

            if (cooperativeMember.address != null)
                await this._addressRepository.Save(cooperativeMember.address);

            await this._cooperativeMemberRepository.Save(cooperativeMember);
            await this._cooperativeMemberRepository.UnitOfWork.Commit();

            return cooperativeMember.id;
        }

        public async Task<bool> CheckIfCpfExists(Guid? id, string cpf)
        {
            var cooperativeMember = await this._cooperativeMemberRepository.GetByCpf(cpf);

            return cooperativeMember != null && cooperativeMember.id != id;
        }

        public async Task<CooperativeMember?> Get(Guid id, bool keepTrack)
        {
            return await this._cooperativeMemberRepository.Get(id, keepTrack);
        }

        public async Task<CooperativeMember?> GetByDapCafCpf(string dap_caf_cpf)
        {
            return await this._cooperativeMemberRepository.GetByDapCafCpf(dap_caf_cpf);
        }

        public async Task Remove(Guid id)
        {
            var cooperativeMember = await this._cooperativeMemberRepository.Get(id, true);

            if (cooperativeMember == null)
                return;

            if (cooperativeMember.address != null)
                await this._addressRepository.Remove(cooperativeMember.address);

            await this._cooperativeMemberRepository.Remove(cooperativeMember);
            await this._cooperativeMemberRepository.UnitOfWork.Commit();
        }

        #endregion [ FIM - Metodos ]
    }
}
