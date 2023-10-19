using FIA.SME.Aquisicao.Core.Helpers;
using FIA.SME.Aquisicao.Infrastructure.Models;
using FIA.SME.Aquisicao.Infrastructure.Repositories;

namespace FIA.SME.Aquisicao.Application.Services
{
    public interface IUserService
    {
        Task<Guid> Add(User user);
        Task ChangePassword(Guid id, string password);
        Task<User?> Get(Guid id, bool keepTrack);
        Task<User?> Get(string email);
        Task<List<User>> GetAll();
        Task Update(User user);
    }

    internal class UserService : IUserService
    {
        #region [ Propriedades ]

        private readonly IPasswordHasher _passwordHasher;
        private readonly IUserRepository _userRepository;

        #endregion [ FIM - Propriedades ]

        #region [ Construtores ]

        public UserService(IPasswordHasher passwordHasher, IUserRepository userRepository)
        {
            this._passwordHasher = passwordHasher;
            this._userRepository = userRepository;
        }

        #endregion [ FIM - Construtores ]

        #region [ Metodos ]

        public async Task<Guid> Add(User user)
        {
            var user_id = Guid.NewGuid();
            user.SetId(user_id);
            user.HashPassword(this._passwordHasher, user.password);

            await this._userRepository.Save(user);
            await this._userRepository.UnitOfWork.Commit();

            return user_id;
        }

        public async Task ChangePassword(Guid id, string password)
        {
            var user = await this._userRepository.Get(id, true);

            if (user == null)
                return;

            user.HashPassword(this._passwordHasher, password);

            await this._userRepository.Save(user);
            await this._userRepository.UnitOfWork.Commit();
        }

        public async Task<User?> Get(Guid id, bool keepTrack)
        {
            return await this._userRepository.Get(id, keepTrack);
        }

        public async Task<User?> Get(string email)
        {
            return await this._userRepository.Get(email);
        }

        public async Task<List<User>> GetAll()
        {
            return await this._userRepository.GetAll();
        }

        public async Task Update(User user)
        {
            await this._userRepository.Save(user);
            await this._userRepository.UnitOfWork.Commit();
        }

        #endregion [ FIM - Metodos ]
    }
}
