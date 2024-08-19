using FIA.SME.Aquisicao.Infrastructure.Interfaces;
using FIA.SME.Aquisicao.Infrastructure.Models;
using FIA.SME.Aquisicao.Infrastructure.Repositories.Context;
using FIA.SME.Aquisicao.Infrastructure.Repositories.Types;
using Microsoft.EntityFrameworkCore;

namespace FIA.SME.Aquisicao.Infrastructure.Repositories
{
    public interface IPublicCallDocumentRepository : IRepository
    {
        Task<PublicCallDocument?> Get(Guid id, bool keepTrack);
        Task<List<PublicCallDocument>> GetAll(Guid publicCallId);
        Task RemoveAll(Guid publicCallId);
        Task Save(PublicCallDocument document);
    }

    internal class PublicCallDocumentRepository : IPublicCallDocumentRepository
    {
        #region [ Propriedades ]

        private readonly SMEContext _context;
        public IUnitOfWork UnitOfWork => _context;

        #endregion [ FIM - Propriedades ]

        #region [ Construtores ]

        public PublicCallDocumentRepository(SMEContext context)
        {
            this._context = context;
        }

        #endregion [ FIM - Construtores ]

        #region [ Metodos ]

        public async Task<PublicCallDocument?> Get(Guid id, bool keepTrack)
        {
            var query = this._context.ChamadaPublicaDocumento!.Where(d => d.id == id);

            if (!keepTrack)
                query = query.AsNoTracking();

            var document = await query.FirstOrDefaultAsync();

            return (document != null) ? new PublicCallDocument(document) : null;
        }

        public async Task<List<PublicCallDocument>> GetAll(Guid publicCallId)
        {
            return await this._context.ChamadaPublicaDocumento.Include(dc => dc.TipoDocumento)
                            .Where(dc => dc.chamada_publica_id == publicCallId)
                            .AsNoTracking()
                            .Select(d => new PublicCallDocument(d))
                            .ToListAsync();
        }

        public Task RemoveAll(Guid publicCallId)
        {
            var documents = this._context.ChamadaPublicaDocumento.Where(dc => dc.chamada_publica_id == publicCallId);

            this._context.ChamadaPublicaDocumento.RemoveRange(documents);

            return Task.CompletedTask;
        }

        public async Task Save(PublicCallDocument document)
        {
            var toSave = await this._context.ChamadaPublicaDocumento.FirstOrDefaultAsync(d => d.id == document.id);

            if (toSave == null)
            {
                toSave = new ChamadaPublicaDocumento();
                this._context.ChamadaPublicaDocumento.Add(toSave);
            }

            toSave.id = document.id;
            toSave.alimento_id = document.food_id;
            toSave.chamada_publica_id = document.public_call_id;
            toSave.tipo_documento_id = document.document_type_id;
        }

        public void Dispose()
        {
            this._context.Dispose();
        }

        #endregion [ FIM - Metodos ]
    }
}
