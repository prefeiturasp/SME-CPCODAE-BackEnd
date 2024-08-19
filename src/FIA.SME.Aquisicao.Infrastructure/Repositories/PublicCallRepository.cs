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
        Task<List<PublicCall>> GetAllOnGoingByCooperative(Guid cooperativeId);
        Task<List<PublicCallReport>> GetAllReport(DateTime startDate, DateTime endDate, int? statusId, string? filterNameNumberProcess);
        Task<List<PublicCallReportCooperative>> GetAllReportCooperatives(DateTime startDate, DateTime endDate, int? statusId, string? filterNameNumberProcess);
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

        public async Task<List<PublicCall>> GetAllOnGoingByCooperative(Guid cooperativeId)
        {
            var chamadasPublicas = await this._context.ChamadaPublicaResposta
                                                .AsNoTracking()
                                                .Where(cpr => cpr.cooperativa_id == cooperativeId)
                                                .Select(cpr => cpr.ChamadaPublica)
                                                .Distinct()
                                                .ToListAsync();

            return chamadasPublicas.ConvertAll(cp => new PublicCall(cp)).OrderBy(c => c.name).ThenBy(c => c.process).ToList();
        }

        public async Task<List<PublicCallReport>> GetAllReport(DateTime startDate, DateTime endDate, int? statusId, string? filterNameNumberProcess)
        {
            var dataInicioFormatada = startDate.Date;
            var dataFimFormatada = endDate.Date.AddDays(1).AddSeconds(-1);

            FormattableString query = $"""
                        SELECT	cp.id AS chamada_publica_id,
                                a.id AS alimento_id,
                                cp.numero,
                                cp.nome,
                                cp.processo,
                                a.nome AS item,
                          		a.unidade_medida AS unidade_medida_enum,
                                cpa.quantidade,
                                cpa.preco AS preco_unitario,
                                cpa.quantidade * cpa.preco AS preco_total,
                                c.cnpj AS cnpj_proponente,
                                c.razao_social AS nome_proponente,
                                c.sigla AS sigla_proponente,
                                ar.nome AS item_projeto_venda,
                                COALESCE(cprcomp.quantidade_comprada, 0) AS quantidade_comprada,
                                cpr.quantidade AS quantidade_item_projeto_venda,
                                0 AS preco_unitario_item_projeto_venda,
                                cpr.preco AS preco_total_item_projeto_venda,
                                CASE WHEN cpr.organico = true THEN 'Organico' ELSE 'Convencional' END AS sistema_producao,
                                CASE WHEN cpr.somente_mulheres = true THEN 'Sim' ELSE 'Não' END AS somente_mulheres,
                                COALESCE(cprc.total_associados, 0) AS total_associados,
                                cpr.total_assentamento_pnra AS porcentagem_pnra,
                                cpr.total_comunidade_quilombola AS porcentagem_quilombola,
                                cpr.total_comunidade_indigena AS porcentagem_indigena,
                                100 - cpr.total_assentamento_pnra - cpr.total_comunidade_quilombola - cpr.total_comunidade_indigena AS porcentagem_demais_associados,
                                cpr.total_outros_agricultores_familiares AS porcentagem_daps_cafs,
                                cpr.codigo_cidade_ibge,
                                cp.codigo_cidade_ibge AS proposal_city_id,
                                cp.data_sessao_publica,
                                cp.data_inscricao_inicio AS data_abertura,
                                cp.data_habilitacao,
                                cp.data_homologacao,
                                cp.data_contratacao,
                                cp.data_contrato_executado,
                                '' AS localizacao,
                                '' AS municipio_maior_numero_associados,
                                cp.status_id,
                                CASE WHEN cpr.foi_escolhida = true THEN cpr.quantidade ELSE NULL END AS quantidade_homologada
                        FROM	chamada_publica cp
                        INNER JOIN chamada_publica_alimento cpa ON cp.id = cpa.chamada_publica_id
                        INNER JOIN alimento a ON cpa.alimento_id = a.id
                        LEFT JOIN (
                            SELECT 	cpr.chamada_publica_id,
                                	cpr.alimento_id,
                                	SUM(cpr.quantidade) AS quantidade_comprada
                            FROM 	chamada_publica_resposta cpr
                            WHERE	cpr.foi_escolhida = true
                            GROUP BY cpr.chamada_publica_id, cpr.alimento_id
                        ) cprcomp ON cp.id = cprcomp.chamada_publica_id AND a.id = cprcomp.alimento_id
                        LEFT JOIN chamada_publica_resposta cpr ON cp.id = cpr.chamada_publica_id AND a.id = cpr.alimento_id AND cpr.foi_escolhida = true
                        LEFT JOIN cooperativa c ON cpr.cooperativa_id = c.id
                        LEFT JOIN alimento ar ON cpr.alimento_id = ar.id
                        LEFT JOIN (SELECT chamada_publica_resposta_id, COUNT(*) AS total_associados FROM chamada_publica_resposta_cooperado GROUP BY chamada_publica_resposta_id) cprc ON cpr.id = cprc.chamada_publica_resposta_id
                        ORDER BY cp.numero, a.nome
                        """;

            var result = await this._context.Database
                                                .SqlQuery<PublicCallReport>(query)
                                                .Where(
                                                    r => r.data_sessao_publica >= dataInicioFormatada && r.data_sessao_publica <= dataFimFormatada
                                                    && (!statusId.HasValue || statusId.Value == r.status_id)
                                                    && (String.IsNullOrEmpty(filterNameNumberProcess) || (r.numero.ToLower().StartsWith(filterNameNumberProcess) || r.nome.ToLower().StartsWith(filterNameNumberProcess) || r.processo.ToLower().StartsWith(filterNameNumberProcess)))
                                                )
                                                .ToListAsync();
            return result;
        }

        public async Task<List<PublicCallReportCooperative>> GetAllReportCooperatives(DateTime startDate, DateTime endDate, int? statusId, string? filterNameNumberProcess)
        {
            var dataInicioFormatada = startDate.Date;
            var dataFimFormatada = endDate.Date.AddDays(1).AddSeconds(-1);

            FormattableString query = $"""
                        SELECT	cp.id AS chamada_publica_id,
                        		cp.numero,
                        		cp.nome,
                        		cp.processo,
                        		cp.data_sessao_publica,
                        		cp.status_id,
                        		COUNT(DISTINCT cpr.cooperativa_id) AS cooperativas_inscritas,
                        		SUM(CASE WHEN cpr.foi_escolhida = true THEN 1 ELSE 0 END) AS cooperativas_habilitadas,
                        		SUM(CASE WHEN cpr.foi_escolhida <> true THEN 1 ELSE 0 END) AS cooperativas_inabilitadas
                        FROM	chamada_publica cp
                        INNER JOIN (
                        	SELECT 	DISTINCT
                        			cpr.chamada_publica_id,
                        			cpr.cooperativa_id,
                        			cpr.foi_escolhida
                        	FROM	chamada_publica_resposta cpr
                        ) cpr ON cp.id = cpr.chamada_publica_id
                        GROUP BY cp.id, cp.numero, cp.nome, cp.processo, cp.data_sessao_publica, cp.status_id
                        ORDER BY cp.numero
                        """;

            var result = await this._context.Database
                                                .SqlQuery<PublicCallReportCooperative>(query)
                                                .Where(
                                                    r => r.data_sessao_publica >= dataInicioFormatada && r.data_sessao_publica <= dataFimFormatada
                                                    && (!statusId.HasValue || statusId.Value == r.status_id)
                                                    && (String.IsNullOrEmpty(filterNameNumberProcess) || (r.numero.ToLower().StartsWith(filterNameNumberProcess) || r.nome.ToLower().StartsWith(filterNameNumberProcess) || r.processo.ToLower().StartsWith(filterNameNumberProcess)))
                                                )
                                                .ToListAsync();
            return result;
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
            toSave.data_cancelamento = publicCall.data_cancelamento?.SetKindUtc();
            toSave.data_deserta = publicCall.data_deserta?.SetKindUtc();
            toSave.data_contratacao = publicCall.data_contratacao?.SetKindUtc();
            toSave.data_contrato_executado = publicCall.data_contrato_executado?.SetKindUtc();
            toSave.data_habilitacao = publicCall.data_habilitacao?.SetKindUtc();
            toSave.data_homologacao = publicCall.data_homologacao?.SetKindUtc();
            toSave.data_suspensao = publicCall.data_suspensao?.SetKindUtc();
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
