using FIA.SME.Aquisicao.Core.Enums;
using FIA.SME.Aquisicao.Infrastructure.Interfaces;
using FIA.SME.Aquisicao.Infrastructure.Models;
using FIA.SME.Aquisicao.Infrastructure.Repositories.Context;
using FIA.SME.Aquisicao.Infrastructure.Repositories.Types;
using Microsoft.EntityFrameworkCore;

namespace FIA.SME.Aquisicao.Infrastructure.Repositories
{
    public interface IConfigurationRepository : IRepository
    {
        Task<Configuration?> Get(Guid id, bool keepTrack);
        Task<List<Configuration>> GetAll();
        Task<Configuration?> GetMaximumYearSuppliedValue();
        Task Save(Configuration configuration);
    }

    internal class ConfigurationRepository : IConfigurationRepository
    {
        #region [ Propriedades ]

        private readonly SMEContext _context;
        public IUnitOfWork UnitOfWork => _context;

        #endregion [ FIM - Propriedades ]

        #region [ Construtores ]

        public ConfigurationRepository(SMEContext context)
        {
            this._context = context;
        }

        #endregion [ FIM - Construtores ]

        #region [ Metodos ]

        public async Task<Configuration?> Get(Guid id, bool keepTrack)
        {
            var query = this._context.Configuracao!.Where(d => d.id == id);

            if (!keepTrack)
                query = query.AsNoTracking();

            var configuration = await query.FirstOrDefaultAsync();

            return (configuration != null) ? new Configuration(configuration) : null;
        }

        public async Task<List<Configuration>> GetAll()
        {
            return await this._context.Configuracao.AsNoTracking().Select(c => new Configuration(c)).ToListAsync();
        }

        public async Task<Configuration?> GetMaximumYearSuppliedValue()
        {
            var name = ConfigurationEnum.MaximumYearSuppliedValue.ToString();

            return await this._context.Configuracao
                                        .AsNoTracking()
                                        .Where(c => c.ativa && c.nome.ToLower() == name.ToLower())
                                        .Select(c => new Configuration(c))
                                        .FirstOrDefaultAsync();
        }

        public async Task Save(Configuration configuration)
        {
            var toSave = await this._context.Configuracao.FirstOrDefaultAsync(d => d.id == configuration.id);

            if (toSave == null)
            {
                toSave = new Configuracao();
                this._context.Configuracao.Add(toSave);
            }

            toSave.id = configuration.id;
            toSave.nome = configuration.name;
            toSave.valor = configuration.value;
            toSave.ativa = configuration.is_active;
        }

        public void Dispose()
        {
            this._context.Dispose();
        }

        #endregion [ FIM - Metodos ]
    }
}
