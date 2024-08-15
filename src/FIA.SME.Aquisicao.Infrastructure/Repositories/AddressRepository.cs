using FIA.SME.Aquisicao.Infrastructure.Models;
using FIA.SME.Aquisicao.Infrastructure.Repositories.Context;
using FIA.SME.Aquisicao.Infrastructure.Interfaces;
using FIA.SME.Aquisicao.Infrastructure.Repositories.Types;
using Microsoft.EntityFrameworkCore;

namespace FIA.SME.Aquisicao.Infrastructure.Repositories
{
    public interface IAddressRepository : IRepository
    {
        Task<Address?> Get(Guid id, bool keepTrack);
        Task<List<Address>> GetAll();
        Task Remove(Address address);
        Task Save(Address address);
    }

    internal class AddressRepository : IAddressRepository
    {
        #region [ Propriedades ]

        private readonly SMEContext _context;
        public IUnitOfWork UnitOfWork => _context;

        #endregion [ FIM - Propriedades ]

        #region [ Construtores ]

        public AddressRepository(SMEContext context)
        {
            this._context = context;
        }

        #endregion [ FIM - Construtores ]

        #region [ Metodos ]

        public async Task<Address?> Get(Guid id, bool keepTrack)
        {
            var query = this._context.Endereco!.Where(d => d.id == id);

            if (!keepTrack)
                query = query.AsNoTracking();

            var address = await query.FirstOrDefaultAsync();

            return (address != null) ? new Address(address) : null;
        }

        public async Task<List<Address>> GetAll()
        {
            return await this._context.Endereco.AsNoTracking().Select(d => new Address(d)).ToListAsync();
        }

        public async Task Remove(Address address)
        {
            var toRemove = await this._context.Endereco.FirstOrDefaultAsync(c => c.id == address.id);

            if (toRemove != null)
            {
                this._context.Endereco.Remove(toRemove);
            }
        }

        public async Task Save(Address address)
        {
            var toSave = await this._context.Endereco.FirstOrDefaultAsync(d => d.id == address.id);

            if (toSave == null)
            {
                toSave = new Endereco();
                this._context.Endereco.Add(toSave);
            }

            toSave.id = address.id;
            toSave.bairro = address.district.Trim();
            toSave.cep = address.cep.Trim();
            toSave.codigo_cidade_ibge = address.city_id;
            toSave.complemento = address.complement?.Trim();
            toSave.numero = address.number.Trim();
            toSave.logradouro = address.street.Trim();
        }

        public void Dispose()
        {
            this._context.Dispose();
        }

        #endregion [ FIM - Metodos ]
    }
}
