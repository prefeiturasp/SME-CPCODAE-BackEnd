using FIA.SME.Aquisicao.Infrastructure.Models;
using FIA.SME.Aquisicao.Infrastructure.Repositories;

namespace FIA.SME.Aquisicao.Application.Services
{
    public interface IPublicCallDocumentService
    {
        Task<PublicCallDocument?> Get(Guid id);
        Task<List<PublicCallDocument>> GetAll(Guid publicCallId);
    }

    internal class PublicCallDocumentService : IPublicCallDocumentService
    {
        #region [ Propriedades ]

        private readonly IPublicCallDocumentRepository _publicCallDocumentRepository;

        #endregion [ FIM - Propriedades ]

        #region [ Construtores ]

        public PublicCallDocumentService(IPublicCallDocumentRepository publicCallDocumentRepository)
        {
            this._publicCallDocumentRepository = publicCallDocumentRepository;
        }

        #endregion [ FIM - Construtores ]

        #region [ Metodos ]

        public async Task<PublicCallDocument?> Get(Guid id)
        {
           return await this._publicCallDocumentRepository.Get(id, false);
        }

        public async Task<List<PublicCallDocument>> GetAll(Guid cooperativeId)
        {
            return await this._publicCallDocumentRepository.GetAll(cooperativeId);
        }

        #endregion [ FIM - Metodos ]
    }
}
