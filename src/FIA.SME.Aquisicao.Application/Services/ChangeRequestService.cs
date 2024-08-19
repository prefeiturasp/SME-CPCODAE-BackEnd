using FIA.SME.Aquisicao.Infrastructure.Models;
using FIA.SME.Aquisicao.Infrastructure.Repositories;

namespace FIA.SME.Aquisicao.Application.Services
{
    public interface IChangeRequestService
    {
        Task<Guid> Add(ChangeRequest changeRequest);
        Task<List<ChangeRequest>> GetAllActive();
        Task<List<ChangeRequest>> GetAllByCooperative(Guid cooperativeId);
        Task<List<ChangeRequest>> GetAllByPublicCall(Guid publicCallId);
        Task<List<CooperativeDocument>> GetAllByPublicCallCooperative(Guid publicCallId, Guid cooperativeId, List<Guid> requestedDocuments);
    }

    internal class ChangeRequestService : IChangeRequestService
    {
        #region [ Propriedades ]

        private readonly IChangeRequestRepository _changeRequestRepository;
        private readonly ICooperativeDocumentRepository _cooperativeDocumentRepository;

        #endregion [ FIM - Propriedades ]

        #region [ Construtores ]

        public ChangeRequestService(IChangeRequestRepository changeRequestRepository, ICooperativeDocumentRepository cooperativeDocumentRepository)
        {
            this._changeRequestRepository = changeRequestRepository;
            this._cooperativeDocumentRepository = cooperativeDocumentRepository;
        }

        #endregion [ FIM - Construtores ]

        #region [ Metodos ]

        public async Task<Guid> Add(ChangeRequest changeRequest)
        {
            var change_request_id = Guid.NewGuid();
            changeRequest.SetId(change_request_id);

            if (changeRequest.refused_documents.Any())
            {
                var documents = await GetAllByPublicCallCooperative(changeRequest.public_call_id, changeRequest.cooperative_id, changeRequest.refused_documents);

                foreach (var document in documents)
                {
                    document.Disable();
                    await this._cooperativeDocumentRepository.Save(document);
                }
            }

            await this._changeRequestRepository.Save(changeRequest);
            await this._changeRequestRepository.UnitOfWork.Commit();

            return change_request_id;
        }

        public async Task<List<ChangeRequest>> GetAllActive()
        {
            return await this._changeRequestRepository.GetAllActive();
        }

        public async Task<List<ChangeRequest>> GetAllByCooperative(Guid cooperativeId)
        {
            return await this._changeRequestRepository.GetAllByCooperative(cooperativeId);
        }

        public async Task<List<ChangeRequest>> GetAllByPublicCall(Guid publicCallId)
        {
            return await this._changeRequestRepository.GetAllByPublicCall(publicCallId);
        }

        public async Task<List<CooperativeDocument>> GetAllByPublicCallCooperative(Guid publicCallId, Guid cooperativeId, List<Guid> requestedDocuments)
        {
            var documentsByCooperative = (await this._cooperativeDocumentRepository.GetAllCooperativeDocumentsByPublicCall(publicCallId, new List<Guid>() { cooperativeId }));
            var documents = documentsByCooperative.Where(d => requestedDocuments.Any(r => r == d.document_type_id && d.is_current)).ToList();

            return documents;
        }

        #endregion [ FIM - Metodos ]
    }
}
