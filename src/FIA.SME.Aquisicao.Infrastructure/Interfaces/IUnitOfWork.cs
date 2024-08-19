namespace FIA.SME.Aquisicao.Infrastructure.Interfaces
{
    public interface IUnitOfWork
    {
        Task<bool> Commit();
        Task<int> CommitReturningAggregateRootId();
        void ReloadContext();
    }
}
