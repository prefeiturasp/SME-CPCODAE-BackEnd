using FIA.SME.Aquisicao.Infrastructure.Interfaces;
using FIA.SME.Aquisicao.Infrastructure.Models;
using FIA.SME.Aquisicao.Infrastructure.Repositories.Context;
using FIA.SME.Aquisicao.Infrastructure.Repositories.Types;
using Microsoft.EntityFrameworkCore;

namespace FIA.SME.Aquisicao.Infrastructure.Repositories
{
    public interface IPublicCallAnswerRepository : IRepository
    {
        Task Delete(Guid id);
        Task<PublicCallAnswer?> Get(Guid id, bool keepTrack);
        Task<List<PublicCallAnswer>> GetAllByCooperativeId(Guid cooperativeId);
        Task<List<PublicCallAnswer>> GetAllByIds(List<Guid> ids);
        Task<List<PublicCallAnswer>> GetAllByPublicCallId(Guid publicCallId);
        Task<List<PublicCallAnswer>> GetAllChosenByPublicCallId(Guid publicCallId);
        Task<PublicCallAnswer?> GetByCooperativeIdPublicCallId(Guid cooperativeId, Guid publicCallId);
        Task<PublicCallAnswer?> GetByCooperativeIdPublicCallIdFoodId(Guid cooperativeId, Guid publicCallId, Guid foodId);
        Task Save(PublicCallAnswer answer);
    }

    internal class PublicCallAnswerRepository : IPublicCallAnswerRepository
    {
        #region [ Propriedades ]

        private readonly SMEContext _context;
        public IUnitOfWork UnitOfWork => _context;

        #endregion [ FIM - Propriedades ]

        #region [ Construtores ]

        public PublicCallAnswerRepository(SMEContext context)
        {
            this._context = context;
        }

        #endregion [ FIM - Construtores ]

        #region [ Metodos ]

        public async Task Delete(Guid id)
        {
            var toDelete = this._context.ChamadaPublicaResposta.Where(cpr => cpr.id == id).FirstOrDefault();

            if (toDelete == null)
                return;

            this._context.ChamadaPublicaResposta.Remove(toDelete);
        }

        public async Task<PublicCallAnswer?> Get(Guid id, bool keepTrack)
        {
            var query = this._context.ChamadaPublicaResposta!.Where(d => d.id == id);

            if (!keepTrack)
                query = query.AsNoTracking();

            var answer = await query.FirstOrDefaultAsync();

            return (answer != null) ? new PublicCallAnswer(answer) : null;
        }

        public async Task<PublicCallAnswer?> GetByCooperativeIdPublicCallId(Guid cooperativeId, Guid publicCallId)
        {
            var query = this._context.ChamadaPublicaResposta!.Where(d => d.cooperativa_id == cooperativeId && d.chamada_publica_id == publicCallId);

            var answer = await query.FirstOrDefaultAsync();

            return (answer != null) ? new PublicCallAnswer(answer) : null;
        }

        public async Task<PublicCallAnswer?> GetByCooperativeIdPublicCallIdFoodId(Guid cooperativeId, Guid publicCallId, Guid foodId)
        {
            var query = this._context.ChamadaPublicaResposta!.Where(d => d.cooperativa_id == cooperativeId && d.chamada_publica_id == publicCallId && d.alimento_id == foodId);

            var answer = await query.FirstOrDefaultAsync();

            return (answer != null) ? new PublicCallAnswer(answer) : null;
        }

        public async Task<List<PublicCallAnswer>> GetAllByCooperativeId(Guid cooperativeId)
        {
            return await this._context.ChamadaPublicaResposta
                                        .Include(cpr => cpr.Alimento)
                                        .Include(cpr => cpr.ChamadaPublica)
                                        .Include(cpr => cpr.ChamadaPublicaEntregas)
                                        .Where(cpr => cpr.cooperativa_id == cooperativeId)
                                        .AsNoTracking()
                                        .Select(cpr => new PublicCallAnswer(cpr))
                                        .ToListAsync();
        }

        public async Task<List<PublicCallAnswer>> GetAllByIds(List<Guid> ids)
        {
            return await this._context.ChamadaPublicaResposta
                                        .Where(cpr => ids.Contains(cpr.id))
                                        .AsNoTracking()
                                        .Select(cpr => new PublicCallAnswer(cpr))
                                        .ToListAsync();
        }

        public async Task<List<PublicCallAnswer>> GetAllByPublicCallId(Guid publicCallId)
        {
            return await this._context.ChamadaPublicaResposta
                                        .Include(cpr => cpr.Alimento)
                                        .Include(cpr => cpr.ChamadaPublica)
                                        .Include(cpr => cpr.ChamadaPublicaEntregas)
                                        .Where(cpr => cpr.chamada_publica_id == publicCallId)
                                        .AsNoTracking()
                                        .Select(cpr => new PublicCallAnswer(cpr))
                                        .ToListAsync();
        }

        public async Task<List<PublicCallAnswer>> GetAllChosenByPublicCallId(Guid publicCallId)
        {
            return await this._context.ChamadaPublicaResposta
                                        .Include(cpr => cpr.Cooperativa.Endereco)
                                        .Include(cpr => cpr.ChamadaPublica)
                                        .Include(cpr => cpr.ChamadaPublicaEntregas)
                                        .Where(cpr => cpr.chamada_publica_id == publicCallId && cpr.foi_escolhida)
                                        .AsNoTracking()
                                        .Select(cpr => new PublicCallAnswer(cpr))
                                        .ToListAsync();
        }

        public async Task Save(PublicCallAnswer answer)
        {
            var toSave = await this._context.ChamadaPublicaResposta.FirstOrDefaultAsync(c => c.id == answer.id);

            if (toSave == null)
            {
                toSave = new ChamadaPublicaResposta();
                this._context.ChamadaPublicaResposta.Add(toSave);
            }

            toSave.id = answer.id;
            toSave.alimento_id = answer.food_id;
            toSave.ativa = answer.is_active;
            toSave.chamada_publica_id = answer.public_call_id;
            toSave.cooperativa_id = answer.cooperative_id;
            toSave.codigo_cidade_ibge = answer.city_id;
            toSave.total_associados_cidade = answer.city_members_total;
            toSave.total_daps_fisicas = answer.daps_fisicas_total;
            toSave.total_comunidade_indigena = answer.indigenous_community_total;
            toSave.total_assentamento_pnra = answer.pnra_settlement_total;
            toSave.total_comunidade_quilombola = answer.quilombola_community_total;
            toSave.total_outros_agricultores_familiares = answer.other_family_agro_total;
            toSave.foi_confirmada = answer.was_confirmed;
            toSave.foi_escolhida = answer.was_chosen;
            toSave.organico = answer.is_organic;
            toSave.preco = answer.price;
            toSave.quantidade = answer.quantity;
            toSave.quantidade_editada = answer.quantity_edited;
            toSave.validada = answer.members_validated;
        }

        public void Dispose()
        {
            this._context.Dispose();
        }

        #endregion [ FIM - Metodos ]
    }
}
