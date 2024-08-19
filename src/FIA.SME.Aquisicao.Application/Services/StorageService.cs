using FIA.SME.Aquisicao.Core.Helpers;
using FIA.SME.Aquisicao.Infrastructure.Interfaces;

namespace FIA.SME.Aquisicao.Application.Services
{
    public interface IStorageService
    {
        Task<List<string>> GetAll(Guid cooperative_id);
        Task Remove(Guid cooperative_id, string filename);
        Task<string> Save(Guid cooperative_id, string document_type_name, string base64File);
    }

    internal class StorageService : IStorageService
    {
        #region [ Propriedades ]

        private readonly ISMEStorage _smeStorage;

        #endregion [ FIM - Propriedades ]

        #region [ Construtores ]

        public StorageService(ISMEStorage smeStorage)
        {
            this._smeStorage = smeStorage;
        }

        #endregion [ FIM - Construtores ]

        #region [ Metodos ]

        public async Task<List<string>> GetAll(Guid cooperative_id)
        {
            return await this._smeStorage.GetAll(cooperative_id);
        }

        public async Task Remove(Guid cooperative_id, string filename)
        {
            await this._smeStorage.Remove(cooperative_id, filename);
        }

        public async Task<string> Save(Guid cooperative_id, string document_type_name, string base64File)
        {
            var fileBytes = Convert.FromBase64String(base64File);
            var fileName = $"{document_type_name.GenerateSlug()}_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.pdf";
            fileName = fileName.Replace('-', '_');

            using (var memStream = new MemoryStream(fileBytes))
            {
                return await this._smeStorage.Save(cooperative_id, fileName, memStream);
            }
        }

        #endregion [ FIM - Metodos ]
    }
}
