namespace FIA.SME.Aquisicao.Infrastructure.Interfaces
{
    public interface ISMEStorage
    {
        Task<List<string>> GetAll(Guid cooperative_id);
        Task<string> GetFilePath(Guid cooperative_id, string fileName);
        Task Remove(Guid cooperative_id, string fileName);
        Task<string> Save(Guid cooperative_id, string fileName, MemoryStream fileMemStream);
    }
}
