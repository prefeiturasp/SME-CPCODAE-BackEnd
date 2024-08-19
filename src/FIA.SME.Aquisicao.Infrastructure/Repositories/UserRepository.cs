using FIA.SME.Aquisicao.Infrastructure.Models;
using FIA.SME.Aquisicao.Infrastructure.Repositories.Context;
using FIA.SME.Aquisicao.Infrastructure.Interfaces;
using FIA.SME.Aquisicao.Infrastructure.Repositories.Types;
using Microsoft.EntityFrameworkCore;

namespace FIA.SME.Aquisicao.Infrastructure.Repositories
{
    public interface IUserRepository : IRepository
    {
        Task<User?> Get(Guid id, bool keepTrack);
        Task<User?> Get(string username);
        Task<List<User>> GetAll();
        Task Save(User user);
    }

    internal class UserRepository : IUserRepository
    {
        #region [ Propriedades ]

        private readonly SMEContext _context;
        public IUnitOfWork UnitOfWork => _context;

        #endregion [ FIM - Propriedades ]

        #region [ Construtores ]

        public UserRepository(SMEContext context)
        {
            this._context = context;
        }

        #endregion [ FIM - Construtores ]

        #region [ Metodos ]

        public async Task<User?> Get(Guid id, bool keepTrack)
        {
            var query = this._context.Usuario!.Where(c => c.id == id);

            if (!keepTrack)
                query = query.AsNoTracking();

            var user = await query.FirstOrDefaultAsync();

            return (user != null) ? new User(user) : null;
        }

        public async Task<User?> Get(string email)
        {
            var user = await this._context.Usuario!.Where(u => u.email.ToLower().Equals(email.ToLower())).AsNoTracking().FirstOrDefaultAsync();

            return (user != null) ? new User(user) : null;
        }

        public async Task<List<User>> GetAll()
        {
            return await this._context.Usuario.AsNoTracking().Select(u => new User(u)).ToListAsync();
        }

        public async Task Save(User user)
        {
            var toSave = await this._context.Usuario.FirstOrDefaultAsync(u => u.id == user.id);

            if (toSave == null)
            {
                toSave = new Usuario();
                this._context.Usuario.Add(toSave);
            }

            toSave.id = user.id;
            toSave.email = user.email.Trim();
            toSave.perfil = user.role.Trim();
            toSave.nome = user.name.Trim();
            toSave.senha = user.password;
            toSave.ativo = user.is_active;
        }

        public void Dispose()
        {
            this._context.Dispose();
        }

        #endregion [ FIM - Metodos ]
    }
}
