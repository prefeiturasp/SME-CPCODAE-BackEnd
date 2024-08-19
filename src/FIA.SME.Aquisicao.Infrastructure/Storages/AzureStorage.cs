using Azure.Storage.Blobs;
using FIA.SME.Aquisicao.Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;

namespace FIA.SME.Aquisicao.Infrastructure.Storages
{
    internal class AzureStorage : ISMEStorage
    {
        #region [ Propriedades ]

        private readonly string _blobConnectionString;
        private readonly string _blobContainerName;

        #endregion [ FIM - Propriedades ]

        #region [ Construtores ]

        public AzureStorage(IConfiguration configuration)
        {
            this._blobConnectionString = configuration.GetSection("Storage:ConnectionString").Value;
            this._blobContainerName = configuration.GetSection("Storage:ContainerName").Value;
        }

        #endregion [ FIM - Construtores ]

        #region [ Metodos ]

        public async Task<List<string>> GetAll(Guid cooperative_id)
        {
            var prefix = cooperative_id.ToString();
            var container = new BlobContainerClient(this._blobConnectionString, this._blobContainerName);
            container.CreateIfNotExists();

            var blobs = container.GetBlobs(prefix: prefix);
            var documentList = new List<string>();

            foreach (var blob in blobs)
            {
                documentList.Add($"{container.Uri.AbsoluteUri}/{blob.Name}");
            }

            return await Task.FromResult(documentList);
        }

        public async Task<string> GetFilePath(Guid cooperative_id, string fileName)
        {
            var cooperativePath = $"{cooperative_id.ToString()}/{fileName}";

            return cooperativePath;
        }

        public async Task Remove(Guid cooperative_id, string fileName)
        {
            var path = $"{cooperative_id.ToString()}/{fileName}";
            var container = new BlobContainerClient(this._blobConnectionString, this._blobContainerName);
            container.CreateIfNotExists();

            var blob = container.GetBlobClient(path);
            await blob.DeleteIfExistsAsync();
        }

        public async Task<string> Save(Guid cooperative_id, string fileName, MemoryStream fileMemStream)
        {
            var path = $"{cooperative_id.ToString()}/{fileName}";
            var container = new BlobContainerClient(this._blobConnectionString, this._blobContainerName);
            container.CreateIfNotExists();

            var blob = container.GetBlobClient(path);
            await blob.UploadAsync(fileMemStream);

            return blob.Uri.AbsoluteUri;
        }

        #endregion [ FIM - Metodos ]
    }
}
