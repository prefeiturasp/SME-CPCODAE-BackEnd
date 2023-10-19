using FIA.SME.Aquisicao.Infrastructure.Models;
using FIA.SME.Aquisicao.Infrastructure.Repositories;

namespace FIA.SME.Aquisicao.Application.Services
{
    public interface IDocumentTypeService
    {
        Task<Guid> Add(DocumentType document);
        Task<DocumentType?> Get(Guid id);
        Task<List<DocumentType>> GetAll();
        Task Update(DocumentType document);
    }

    internal class DocumentTypeService : IDocumentTypeService
    {
        #region [ Propriedades ]

        private readonly IDocumentTypeRepository _documentTypeRepository;

        #endregion [ FIM - Propriedades ]

        #region [ Construtores ]

        public DocumentTypeService(IDocumentTypeRepository documentTypeRepository)
        {
            this._documentTypeRepository = documentTypeRepository;
        }

        #endregion [ FIM - Construtores ]

        #region [ Metodos ]

        public async Task<Guid> Add(DocumentType document)
        {
            var documentType_id = Guid.NewGuid();
            document.SetId(documentType_id);

            await this._documentTypeRepository.Save(document);
            await this._documentTypeRepository.UnitOfWork.Commit();

            return documentType_id;
        }

        public async Task<DocumentType?> Get(Guid id)
        {
           return await this._documentTypeRepository.Get(id, false);
        }

        public async Task<List<DocumentType>> GetAll()
        {
            return await this._documentTypeRepository.GetAll();
        }

        public async Task Update(DocumentType document)
        {
            await this._documentTypeRepository.Save(document);
            await this._documentTypeRepository.UnitOfWork.Commit();
        }

        #endregion [ FIM - Metodos ]
    }
}
