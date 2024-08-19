using Azure.Storage.Blobs;
using FIA.SME.Aquisicao.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using PdfSharpCore.Pdf;
using System.IO;

namespace FIA.SME.Aquisicao.Infrastructure.Storages
{
    internal class LocalStorage : ISMEStorage
    {
        #region [ Propriedades ]

        private readonly string _storagePath;

        #endregion [ FIM - Propriedades ]

        #region [ Construtores ]

        public LocalStorage(IConfiguration configuration)
        {
            this._storagePath = configuration.GetSection("Storage:LocalPath").Value;
        }

        #endregion [ FIM - Construtores ]

        #region [ Metodos ]

        public async Task<List<string>> GetAll(Guid cooperative_id)
        {
            var cooperativePath = ValidateFolderStructureIsOk(cooperative_id);
            var documentList = Directory.GetFiles(cooperativePath).ToList();

            return await Task.FromResult(documentList);
        }

        public async Task<string> GetFilePath(Guid cooperative_id, string fileName)
        {
            var cooperativePath = $"{cooperative_id.ToString()}/{fileName}";
            var filePath = Path.Combine(this._storagePath, cooperativePath);

            return filePath;
        }

        public async Task Remove(Guid cooperative_id, string fileName)
        {
            var cooperativePath = ValidateFolderStructureIsOk(cooperative_id);
            var filePath = Path.Combine(cooperativePath, fileName);

            if (File.Exists(filePath))
                File.Delete(filePath);
        }

        public async Task<string> Save(Guid cooperative_id, string fileName, MemoryStream fileMemStream)
        {
            var cooperativePath = ValidateFolderStructureIsOk(cooperative_id);
            var filePath = Path.Combine(cooperativePath, fileName);

            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                fileMemStream.CopyTo(fileStream);
            }

            return filePath;
        }

        private string ValidateFolderStructureIsOk(Guid cooperative_id)
        {
            var folder = this._storagePath;

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            var cooperativePath = Path.Combine(folder, cooperative_id.ToString());

            if (!Directory.Exists(cooperativePath))
                Directory.CreateDirectory(cooperativePath);

            return cooperativePath;
        }

        #endregion [ FIM - Metodos ]
    }
}
