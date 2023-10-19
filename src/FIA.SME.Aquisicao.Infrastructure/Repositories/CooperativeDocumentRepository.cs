using FIA.SME.Aquisicao.Infrastructure.Models;
using FIA.SME.Aquisicao.Infrastructure.Repositories.Context;
using FIA.SME.Aquisicao.Infrastructure.Interfaces;
using FIA.SME.Aquisicao.Infrastructure.Repositories.Types;
using Microsoft.EntityFrameworkCore;
using FIA.SME.Aquisicao.Core.Helpers;

namespace FIA.SME.Aquisicao.Infrastructure.Repositories
{
    public interface ICooperativeDocumentRepository : IRepository
    {
        Task DeleteByPublicCallIdCooperativeId(Guid publicCallId, Guid cooperativeId);
        Task<CooperativeDocument?> Get(Guid id, bool keepTrack);
        Task<List<CooperativeDocument>> GetAll(Guid cooperativeId);
        Task<List<CooperativeDocument>> GetAllCooperativeDocumentsByPublicCall(Guid publicCallId, List<Guid> cooperativeIds);
        Task<bool> Remove(Guid id);
        Task Save(CooperativeDocument document);
    }

    internal class CooperativeDocumentRepository : ICooperativeDocumentRepository
    {
        #region [ Propriedades ]

        private readonly SMEContext _context;
        public IUnitOfWork UnitOfWork => _context;

        #endregion [ FIM - Propriedades ]

        #region [ Construtores ]

        public CooperativeDocumentRepository(SMEContext context)
        {
            this._context = context;
        }

        #endregion [ FIM - Construtores ]

        #region [ Metodos ]

        public async Task DeleteByPublicCallIdCooperativeId(Guid publicCallId, Guid cooperativeId)
        {
            var documentos = this._context.DocumentoCooperativa.Where(cd => cd.chamada_publica_id == publicCallId && cd.cooperativa_id == cooperativeId);

            foreach (var documento in documentos)
            {
                this._context.DocumentoCooperativa.Remove(documento);
            }
        }

        public async Task<CooperativeDocument?> Get(Guid id, bool keepTrack)
        {
            var query = this._context.DocumentoCooperativa!.Where(d => d.id == id);

            if (!keepTrack)
                query = query.AsNoTracking();

            var document = await query.FirstOrDefaultAsync();

            return (document != null) ? new CooperativeDocument(document) : null;
        }

        public async Task<List<CooperativeDocument>> GetAll(Guid cooperativeId)
        {
            return await this._context.DocumentoCooperativa.Include(dc => dc.TipoDocumento)
                            .Where(dc => dc.cooperativa_id == cooperativeId)
                            .AsNoTracking()
                            .Select(d => new CooperativeDocument(d))
                            .ToListAsync();
        }

        public async Task<List<CooperativeDocument>> GetAllCooperativeDocumentsByPublicCall(Guid publicCallId, List<Guid> cooperativeIds)
        {
            return await this._context.DocumentoCooperativa.Include(d => d.TipoDocumento)
                                .AsNoTracking()
                                .Where(d => cooperativeIds.Contains(d.cooperativa_id) && (d.chamada_publica_id == null || d.chamada_publica_id == publicCallId))
                                .OrderBy(d => d.cooperativa_id)
                                .ThenByDescending(d => d.data_criacao)
                                .Select(d => new CooperativeDocument(d))
                                .ToListAsync();
        }

        public async Task<bool> Remove(Guid id)
        {
            var document = await this._context.DocumentoCooperativa.FirstOrDefaultAsync(d => d.id == id);

            if (document == null)
                return false;

            this._context.DocumentoCooperativa.Remove(document);

            return true;
        }

        public async Task Save(CooperativeDocument document)
        {
            var toSave = await this._context.DocumentoCooperativa.FirstOrDefaultAsync(d => d.id == document.id);

            if (toSave == null)
            {
                toSave = new DocumentoCooperativa();
                toSave.data_criacao = DateTime.UtcNow.SetKindUtc();
                this._context.DocumentoCooperativa.Add(toSave);
            }

            toSave.id = document.id;
            toSave.alimento_id = document.food_id;
            toSave.chamada_publica_id = document.public_call_id;
            toSave.cooperativa_id = document.cooperative_id;
            toSave.tipo_documento_id = document.document_type_id;
            toSave.documento_path = document.document_path;
            toSave.documento_tamanho = document.file_size;
            toSave.is_atual = document.is_current;
            toSave.is_revisado = document.is_reviewed;
        }

        public void Dispose()
        {
            this._context.Dispose();
        }

        #endregion [ FIM - Metodos ]
    }
}
