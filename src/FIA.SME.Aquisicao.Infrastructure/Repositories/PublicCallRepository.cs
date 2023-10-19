using FIA.SME.Aquisicao.Core.Helpers;
using FIA.SME.Aquisicao.Infrastructure.Models;
using FIA.SME.Aquisicao.Infrastructure.Repositories.Context;
using FIA.SME.Aquisicao.Infrastructure.Interfaces;
using FIA.SME.Aquisicao.Infrastructure.Repositories.Types;
using Microsoft.EntityFrameworkCore;
using FIA.SME.Aquisicao.Core.Enums;

namespace FIA.SME.Aquisicao.Infrastructure.Repositories
{
    public interface IPublicCallRepository : IRepository
    {
        Task<PublicCall?> Get(Guid id, bool keepTrack);
        Task<List<PublicCall>> GetAll();
        Task<List<PublicCall>> GetAllDashboard();
        Task Save(PublicCall publicCall);
    }

    internal class PublicCallRepository : IPublicCallRepository
    {
        #region [ Propriedades ]

        private readonly SMEContext _context;
        public IUnitOfWork UnitOfWork => _context;

        #endregion [ FIM - Propriedades ]

        #region [ Construtores ]

        public PublicCallRepository(SMEContext context)
        {
            this._context = context;
        }

        #endregion [ FIM - Construtores ]

        #region [ Metodos ]

        public async Task<PublicCall?> Get(Guid id, bool keepTrack)
        {
            var query = this._context.ChamadaPublica!
                                .Include(c => c.ChamadaPublicaAlimentos.OrderBy(c => c.Alimento.nome)).ThenInclude(c => c.Alimento.Categoria)
                                .Include(c => c.ChamadaPublicaDocumentos).ThenInclude(cpd => cpd.TipoDocumento)
                                .Where(c => c.id == id);

            if (!keepTrack)
                query = query.AsNoTracking();

            var publicCall = await query.FirstOrDefaultAsync();

            return (publicCall != null) ? new PublicCall(publicCall) : null;
        }

        public async Task<List<PublicCall>> GetAll()
        {
            return await this._context.ChamadaPublica
                                        .Include(pc => pc.ChamadaPublicaAlimentos)
                                        .ThenInclude(cpa => cpa.Alimento)
                                        .Include(c => c.ChamadaPublicaDocumentos)
                                        .AsNoTracking()
                                        .Select(c => new PublicCall(c)).ToListAsync();
        }

        public async Task<List<PublicCall>> GetAllDashboard()
        {
            var chamadas = await this._context.ChamadaPublica
                                            .Include(cp => cp.ChamadaPublicaAlimentos).ThenInclude(cpa => cpa.Alimento)
                                            .Include(c => c.ChamadaPublicaDocumentos).ThenInclude(cpd => cpd.TipoDocumento)
                                            .AsNoTracking()
                                            .Select(c => new PublicCall(c))
                                            .ToListAsync();

            return chamadas;
        }

        public async Task Save(PublicCall publicCall)
        {
            var toSave = await this._context.ChamadaPublica.FirstOrDefaultAsync(c => c.id == publicCall.id);

            if (toSave == null)
            {
                publicCall.SetStatus(PublicCallStatusEnum.EmAndamento);

                toSave = new ChamadaPublica();
                toSave.data_criacao = DateTime.UtcNow.SetKindUtc();
                this._context.ChamadaPublica.Add(toSave);
            }

            toSave.id = publicCall.id;
            toSave.codigo_cidade_ibge = publicCall.city_id;
            toSave.edital_url = publicCall.notice_url;
            toSave.estimativa = publicCall.delivery_information;
            toSave.informacoes_adicionais = publicCall.extra_information;
            toSave.nome = publicCall.name;
            toSave.numero = publicCall.number;
            toSave.objeto = publicCall.notice_object;
            toSave.orgao = publicCall.agency;
            toSave.processo = publicCall.process;
            toSave.sessao_publica_local = publicCall.public_session_place;
            toSave.sessao_publica_url = publicCall.public_session_url;
            toSave.tipo = publicCall.type;
            toSave.data_inscricao_inicio = publicCall.registration_start_date.SetKindUtc();
            toSave.data_inscricao_termino = publicCall.registration_end_date.SetKindUtc();
            toSave.data_sessao_publica = publicCall.public_session_date.SetKindUtc();
            toSave.status_id = publicCall.status_id;
            toSave.ativa = publicCall.is_active;
        }

        public void Dispose()
        {
            this._context.Dispose();
        }

        #endregion [ FIM - Metodos ]
    }
}
