namespace FIA.SME.Aquisicao.Infrastructure.Interfaces
{
    public interface IRepository : IDisposable
    {
        IUnitOfWork UnitOfWork { get; }
    }
}
