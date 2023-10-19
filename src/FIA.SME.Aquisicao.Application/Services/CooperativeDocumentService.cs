using FIA.SME.Aquisicao.Core.Helpers;
using FIA.SME.Aquisicao.Infrastructure.Components;
using FIA.SME.Aquisicao.Infrastructure.Models;
using FIA.SME.Aquisicao.Infrastructure.Repositories;
using System.Reflection.Metadata;

namespace FIA.SME.Aquisicao.Application.Services
{
    public interface ICooperativeDocumentService
    {
        Task<Guid> Add(CooperativeDocument document);
        Task<CooperativeDocument?> Get(Guid id);
        Task<List<CooperativeDocument>> GetAll(Guid cooperativeId);
        Task<List<CooperativeDocument>> GetAllCooperativeDocumentsByPublicCall(Guid publicCallId, List<Guid> cooperativeIds);
        Task<string> GetCooperativeCurrentZippedDocumentsByPublicCall(Guid publicCallId, Guid cooperativeId);
        Task<string> GetDocumentFileBase64(Guid documentId);
        Task Remove(Guid id);
        Task SetAsReviewed(Guid id, bool isReviewed);
    }

    internal class CooperativeDocumentService : ICooperativeDocumentService
    {
        #region [ Propriedades ]

        private readonly ICooperativeDocumentRepository _cooperativeDocumentRepository;

        #endregion [ FIM - Propriedades ]

        #region [ Construtores ]

        public CooperativeDocumentService(ICooperativeDocumentRepository cooperativeDocumentRepository)
        {
            this._cooperativeDocumentRepository = cooperativeDocumentRepository;
        }

        #endregion [ FIM - Construtores ]

        #region [ Metodos ]

        public async Task<Guid> Add(CooperativeDocument document)
        {
            var document_id = Guid.NewGuid();
            document.SetId(document_id);

            await this._cooperativeDocumentRepository.Save(document);
            await this._cooperativeDocumentRepository.UnitOfWork.Commit();

            return document_id;
        }

        public async Task<CooperativeDocument?> Get(Guid id)
        {
           return await this._cooperativeDocumentRepository.Get(id, false);
        }

        public async Task<List<CooperativeDocument>> GetAll(Guid cooperativeId)
        {
            return await this._cooperativeDocumentRepository.GetAll(cooperativeId);
        }

        public async Task<List<CooperativeDocument>> GetAllCooperativeDocumentsByPublicCall(Guid publicCallId, List<Guid> cooperativeIds)
        {
            return await this._cooperativeDocumentRepository.GetAllCooperativeDocumentsByPublicCall(publicCallId, cooperativeIds);
        }

        public async Task<string> GetCooperativeCurrentZippedDocumentsByPublicCall(Guid publicCallId, Guid cooperativeId)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            var allDocuments = (await this._cooperativeDocumentRepository.GetAllCooperativeDocumentsByPublicCall(publicCallId, new List<Guid>() { cooperativeId }));
            var documents = allDocuments.Where(d => d.is_current).OrderBy(d => d.document_type_name).ToList();
            var oldDocuments = allDocuments.Where(d => !d.is_current).OrderByDescending(d => d.creation_date).ThenBy(d => d.document_type_name).ToList();
            documents.AddRange(oldDocuments);

            if (!documents.Any())
                return String.Empty;

            var listOfPDFBytes = new List<(string, byte[])>();

            foreach (var document in documents)
            {
                byte[] bytes;
                using (var memoryStream = new MemoryStream())
                {
                    document.document_path.GetStreamFromUrl().CopyTo(memoryStream);
                    bytes = memoryStream.ToArray();

                    var headerText = $"{document.document_type_name} - {document.creation_date.ToString("dd/MM/yyyy HH:mm")}";
                    listOfPDFBytes.Add((headerText, bytes));
                }
            }

            var byteMergedPdf = PDFHelperComponent.MergePdf(listOfPDFBytes);
            return Convert.ToBase64String(byteMergedPdf);

            //using (MemoryStream zipStream = new MemoryStream())
            //{
            //    using (ZipArchive zip = new ZipArchive(zipStream, ZipArchiveMode.Create))
            //    {
            //        foreach (var document in documents)
            //        {
            //            ZipArchiveEntry entry = zip.CreateEntry(document.document_path);

            //            using (Stream entryStream = entry.Open())
            //            {
            //                document.document_path.GetStreamFromUrl().CopyTo(entryStream);
            //            }
            //        }

            //        zipStream.Position = 0;

            //        return zipStream.ConvertToBase64();
            //    }
            //}
        }

        public async Task<string> GetDocumentFileBase64(Guid documentId)
        {
            byte[] bytes;
            var document = (await this._cooperativeDocumentRepository.Get(documentId, false));

            if (document == null)
                return String.Empty;

            using (var memoryStream = new MemoryStream())
            {
                document.document_path.GetStreamFromUrl().CopyTo(memoryStream);
                bytes = memoryStream.ToArray();
            }

            return Convert.ToBase64String(bytes);
        }

        public async Task Remove(Guid id)
        {
            var document = await this._cooperativeDocumentRepository.Get(id, true);

            if (document != null)
            {
                document.Disable();

                await this._cooperativeDocumentRepository.Save(document);
                await this._cooperativeDocumentRepository.UnitOfWork.Commit();
            }
        }

        public async Task SetAsReviewed(Guid id, bool isReviewed)
        {
            var document = await this._cooperativeDocumentRepository.Get(id, true);

            if (document != null)
            {
                document.SetAsReviewed(isReviewed);

                await this._cooperativeDocumentRepository.Save(document);
                await this._cooperativeDocumentRepository.UnitOfWork.Commit();
            }
        }

        #endregion [ FIM - Metodos ]
    }
}
