using FIA.SME.Aquisicao.Infrastructure.Models;
using FIA.SME.Aquisicao.Infrastructure.Repositories;

namespace FIA.SME.Aquisicao.Application.Services
{
    public interface IConfigurationService
    {
        Task<Configuration?> Get(Guid id, bool keepTrack);
        Task<List<Configuration>> GetAll();
        Task<decimal> GetMaximumYearSuppliedValue();
        Task Update(Configuration configuration);
    }

    internal class ConfigurationService : IConfigurationService
    {
        #region [ Propriedades ]

        private readonly IConfigurationRepository _configurationRepository;

        #endregion [ FIM - Propriedades ]

        #region [ Construtores ]

        public ConfigurationService(IConfigurationRepository configurationRepository)
        {
            this._configurationRepository = configurationRepository;
        }

        #endregion [ FIM - Construtores ]

        #region [ Metodos ]

        public async Task<Configuration?> Get(Guid id, bool keepTrack)
        {
            return await this._configurationRepository.Get(id, keepTrack);
        }

        public async Task<List<Configuration>> GetAll()
        {
            return await this._configurationRepository.GetAll();
        }

        public async Task<decimal> GetMaximumYearSuppliedValue()
        {
            var config = await this._configurationRepository.GetMaximumYearSuppliedValue();

            if (Decimal.TryParse(config?.value, out decimal maximumYearSuppliedValue))
                return maximumYearSuppliedValue;

            return 40000;
        }

        public async Task Update(Configuration configuration)
        {
            await this._configurationRepository.Save(configuration);
            await this._configurationRepository.UnitOfWork.Commit();
        }

        #endregion [ FIM - Metodos ]
    }
}
