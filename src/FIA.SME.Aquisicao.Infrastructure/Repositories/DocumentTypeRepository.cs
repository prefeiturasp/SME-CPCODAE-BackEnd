using FIA.SME.Aquisicao.Infrastructure.Models;
using FIA.SME.Aquisicao.Infrastructure.Repositories.Context;
using FIA.SME.Aquisicao.Infrastructure.Interfaces;
using FIA.SME.Aquisicao.Infrastructure.Repositories.Types;
using Microsoft.EntityFrameworkCore;

namespace FIA.SME.Aquisicao.Infrastructure.Repositories
{
    public interface IDocumentTypeRepository : IRepository
    {
        Task<DocumentType?> Get(Guid id, bool keepTrack);
        Task<List<DocumentType>> GetAll();
        Task Save(DocumentType document);
    }

    internal class DocumentTypeRepository : IDocumentTypeRepository
    {
        #region [ Propriedades ]

        private readonly SMEContext _context;
        public IUnitOfWork UnitOfWork => _context;

        #endregion [ FIM - Propriedades ]

        #region [ Construtores ]

        public DocumentTypeRepository(SMEContext context)
        {
            this._context = context;
        }

        #endregion [ FIM - Construtores ]

        #region [ Metodos ]

        public async Task<DocumentType?> Get(Guid id, bool keepTrack)
        {
            var query = this._context.TipoDocumento!.Where(d => d.id == id);

            if (!keepTrack)
                query = query.AsNoTracking();

            var document = await query.FirstOrDefaultAsync();

            return (document != null) ? new DocumentType(document) : null;
        }

        public async Task<List<DocumentType>> GetAll()
        {
            return await this._context.TipoDocumento.AsNoTracking().Where(d => d.visivel).Select(d => new DocumentType(d)).ToListAsync();
        }

        public async Task Save(DocumentType document)
        {
            var toSave = await this._context.TipoDocumento.FirstOrDefaultAsync(d => d.id == document.id);

            if (toSave == null)
            {
                toSave = new TipoDocumento();
                this._context.TipoDocumento.Add(toSave);
            }

            toSave.id = document.id;
            toSave.nome = document.name;
            toSave.aplicacao = document.application;
            toSave.visivel = document.is_visible;
            toSave.ativo = document.is_active;
        }

        public void Dispose()
        {
            this._context.Dispose();
        }

        #endregion [ FIM - Metodos ]
    }
}
