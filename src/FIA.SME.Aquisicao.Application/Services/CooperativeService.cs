using FIA.SME.Aquisicao.Core.Enums;
using FIA.SME.Aquisicao.Core.Helpers;
using FIA.SME.Aquisicao.Infrastructure.Models;
using FIA.SME.Aquisicao.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace FIA.SME.Aquisicao.Application.Services
{
    public interface ICooperativeService
    {
        Task<string> GetHash(Guid id, string password);
        Task<Guid> Add(Cooperative cooperative, User user);
        Task<bool> CheckIfCnpjExists(Guid? id, string cnpj);
        Task<bool> CheckIfDapCafExists(Guid? id, string dap_caf_code);
        Task<string> CreateRegisterHmacCode(string cnpj);
        Task<Cooperative?> Get(Guid id, bool keepTrack = false);
        Task<List<Cooperative>> GetAll();
        Task<Cooperative?> GetByCnpj(string cnpj);
        Task<Cooperative?> GetByUserId(Guid userId);
        Task<string?> GetCnpjByToken(string codeBase64Url);
        Task Update(Cooperative cooperative);
    }

    internal class CooperativeService : ICooperativeService
    {
        #region [ Propriedades ]

        private static readonly Byte[] _privateKey = new Byte[] { 0xDE, 0xAD, 0xBE, 0xEF };
        private readonly IAddressRepository _addressRepository;
        private readonly IBankRepository _bankRepository;
        private readonly ICooperativeLegalRepresentativeRepository _cooperativeLegalRepresentativeRepository;
        private readonly ICooperativeRepository _cooperativeRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUserRepository _userRepository;
        private const byte _version = 1;

        #endregion [ FIM - Propriedades ]

        #region [ Construtores ]

        public CooperativeService(
            IAddressRepository addressRepository,
            IBankRepository bankRepository,
            ICooperativeLegalRepresentativeRepository cooperativeLegalRepresentativeRepository,
            ICooperativeRepository cooperativeRepository,
            IPasswordHasher passwordHasher,
            IUserRepository userRepository)
        {
            this._addressRepository = addressRepository;
            this._bankRepository = bankRepository;
            this._cooperativeLegalRepresentativeRepository = cooperativeLegalRepresentativeRepository;
            this._cooperativeRepository = cooperativeRepository;
            this._passwordHasher = passwordHasher;
            this._userRepository = userRepository;
        }

        #endregion [ FIM - Construtores ]

        #region [ Metodos ]

        public async Task<string> GetHash(Guid id, string password)
        {
            return HashPassword(this._passwordHasher, id, password);
        }

        public string HashPassword(IPasswordHasher passwordHasher, Guid id, string password)
        {
            var hashedPassword = passwordHasher.HashPassword(id.ToString(), password);
            return hashedPassword;
        }

        public async Task<Guid> Add(Cooperative cooperative, User user)
        {
            var user_id = Guid.NewGuid();
            user.SetId(user_id);
            user.SetRole(RoleEnum.Cooperativa);
            user.HashPassword(this._passwordHasher, user.password);

            var cooperative_id = Guid.NewGuid();
            cooperative.SetId(cooperative_id);
            cooperative.SetUserId(user_id);

            await this._userRepository.Save(user);
            await this._cooperativeRepository.Save(cooperative);

            await this._cooperativeRepository.UnitOfWork.Commit();

            return cooperative_id;
        }

        public async Task<bool> CheckIfCnpjExists(Guid? id, string cnpj)
        {
            var cooperative = await this._cooperativeRepository.GetByCnpj(cnpj);

            return cooperative != null && cooperative.id != id;
        }

        public async Task<bool> CheckIfDapCafExists(Guid? id, string dap_caf_code)
        {
            var cooperative = await this._cooperativeRepository.GetByDapCaf(dap_caf_code);

            return cooperative != null && cooperative.id != id;
        }

        public async Task<string> CreateRegisterHmacCode(string cnpj)
        {
            // Deixa somente números
            var cnpjOnlyNumbers = cnpj.ToOnlyNumbers();

            byte[] message = Enumerable.Empty<byte>()
                .Append(_version)
                .Concat(Encoding.UTF8.GetBytes(cnpjOnlyNumbers))
                .ToArray();

            using (HMACSHA256 hmacSha256 = new HMACSHA256(key: _privateKey))
            {
                byte[] hash = hmacSha256.ComputeHash(buffer: message, offset: 0, count: message.Length);

                byte[] outputMessage = message.Concat(hash).ToArray();
                String outputCodeB64 = Convert.ToBase64String(outputMessage);
                String outputCode = outputCodeB64.Replace('+', '-').Replace('/', '_');

                return await Task.FromResult(outputCode);
            }
        }

        public async Task<Cooperative?> Get(Guid id, bool keepTrack = false)
        {
           return await this._cooperativeRepository.Get(id, keepTrack);
        }

        public async Task<List<Cooperative>> GetAll()
        {
            return await this._cooperativeRepository.GetAll();
        }

        public async Task<Cooperative?> GetByCnpj(string cnpj)
        {
            return await this._cooperativeRepository.GetByCnpj(cnpj);
        }

        public async Task<Cooperative?> GetByUserId(Guid userId)
        {
            return await this._cooperativeRepository.GetByUserId(userId);
        }

        public async Task<string?> GetCnpjByToken(string codeBase64Url)
        {
            string base64 = codeBase64Url.Replace('-', '+').Replace('_', '/');
            byte[] message = Convert.FromBase64String(base64);

            byte version = message[0];

            if (version < _version)
                return null;

            var cnpjLength = Encoding.UTF8.GetBytes("00000000000000").Length; // 14 caracteres

            var cnpjStartIndex = 1;
            var cnpjByteArray = new byte[cnpjLength];
            Array.Copy(message, cnpjStartIndex, cnpjByteArray, 0, cnpjLength);
            var cnpjInToken = Encoding.UTF8.GetString(cnpjByteArray);

            return await Task.FromResult(cnpjInToken);
        }

        public async Task Update(Cooperative cooperative)
        {
            if (cooperative.address != null)
                await this._addressRepository.Save(cooperative.address);

            if (cooperative.bank != null)
                await this._bankRepository.Save(cooperative.bank);

            if (cooperative.legal_representative != null && cooperative.legal_representative.address != null)
            {
                await this._addressRepository.Save(cooperative.legal_representative.address);
                await this._cooperativeLegalRepresentativeRepository.Save(cooperative.legal_representative);
            }

            await this._cooperativeRepository.Save(cooperative);

            await this._cooperativeRepository.UnitOfWork.Commit();
        }

        #endregion [ FIM - Metodos ]
    }
}
