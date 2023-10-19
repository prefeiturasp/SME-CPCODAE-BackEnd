using FIA.SME.Aquisicao.Infrastructure.Interfaces;
using FIA.SME.Aquisicao.Infrastructure.Repositories.Types;
using Microsoft.EntityFrameworkCore;

namespace FIA.SME.Aquisicao.Infrastructure.Repositories.Context
{
    internal class SMEContext : DbContext, IDbContext
    {
        #region [ Propriedades ]

        public DbSet<Alimento> Alimento                                                 { get; set; }
        public DbSet<Banco> Banco                                                       { get; set; }
        public DbSet<Categoria> Categoria                                               { get; set; }
        public DbSet<ChamadaPublica> ChamadaPublica                                     { get; set; }
        public DbSet<ChamadaPublicaAlimento> ChamadaPublicaAlimento                     { get; set; }
        public DbSet<ChamadaPublicaDocumento> ChamadaPublicaDocumento                   { get; set; }
        public DbSet<ChamadaPublicaEntrega> ChamadaPublicaEntrega                       { get; set; }
        public DbSet<ChamadaPublicaResposta> ChamadaPublicaResposta                     { get; set; }
        public DbSet<ChamadaPublicaRespostaCooperado> ChamadaPublicaRespostaCooperado   { get; set; }
        public DbSet<Cooperado> Cooperado                                               { get; set; }
        public DbSet<Cooperativa> Cooperativa                                           { get; set; }
        public DbSet<DocumentoCooperativa> DocumentoCooperativa                         { get; set; }
        public DbSet<Endereco> Endereco                                                 { get; set; }
        public DbSet<LocalidadeRegiao> LocalidadeRegiao                                 { get; set; }
        public DbSet<RepresentanteLegal> RepresentanteLegal                             { get; set; }
        public DbSet<SolicitacaoAlteracao> SolicitacaoAlteracao                         { get; set; }
        public DbSet<TipoDocumento> TipoDocumento                                       { get; set; }
        public DbSet<Usuario> Usuario                                                   { get; set; }

        #endregion [ FIM - Propriedades ]

        #region [ Construtores ]

        internal SMEContext() { }

        public SMEContext(DbContextOptions<SMEContext> options) : base(options) { }

        #endregion [ FIM - Construtores ]

        #region [ Metodos ]

        public async Task<bool> Commit()
        {
            try
            {
                var entries = this.ChangeTracker.Entries();
                var sucesso = base.SaveChanges() > 0;

                return await Task.FromResult(sucesso);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Task<int> CommitReturningAggregateRootId()
        {
            throw new NotImplementedException();
        }

        public void ReloadContext()
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                entry.Reload();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cooperativa>().HasMany(c => c.Cooperados).WithOne(c => c.Cooperativa);
            modelBuilder.Entity<ChamadaPublica>().HasMany(c => c.ChamadaPublicaAlimentos).WithOne(c => c.ChamadaPublica);

            base.OnModelCreating(modelBuilder);
        }

        #endregion [ FIM - Metodos ]
    }
}
