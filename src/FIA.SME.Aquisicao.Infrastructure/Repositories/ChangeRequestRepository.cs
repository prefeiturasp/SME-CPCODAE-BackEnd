using FIA.SME.Aquisicao.Infrastructure.Models;
using FIA.SME.Aquisicao.Infrastructure.Repositories.Context;
using FIA.SME.Aquisicao.Infrastructure.Interfaces;
using FIA.SME.Aquisicao.Infrastructure.Repositories.Types;
using Microsoft.EntityFrameworkCore;
using FIA.SME.Aquisicao.Core.Helpers;
using FIA.SME.Aquisicao.Core.Enums;

namespace FIA.SME.Aquisicao.Infrastructure.Repositories
{
    public interface IChangeRequestRepository : IRepository
    {
        Task<List<ChangeRequest>> GetAllActive();
        Task<List<ChangeRequest>> GetAllByCooperative(Guid cooperativeId);
        Task<List<ChangeRequest>> GetAllByPublicCall(Guid publicCallId);
        Task Save(ChangeRequest changeRequest);
    }

    internal class ChangeRequestRepository : IChangeRequestRepository
    {
        #region [ Propriedades ]

        private readonly SMEContext _context;
        public IUnitOfWork UnitOfWork => _context;

        #endregion [ FIM - Propriedades ]

        #region [ Construtores ]

        public ChangeRequestRepository(SMEContext context)
        {
            this._context = context;
        }

        #endregion [ FIM - Construtores ]

        #region [ Metodos ]

        public async Task<List<ChangeRequest>> GetAllActive()
        {
            return await this._context.SolicitacaoAlteracao.Include(sa => sa.ChamadaPublica).Include(sa => sa.Cooperativa)
                                .AsNoTracking()
                                .Where(sa => sa.ChamadaPublica.status_id <= (int)PublicCallStatusEnum.EmAndamento)
                                .OrderBy(sa => sa.chamada_publica_id)
                                .ThenBy(sa => sa.cooperativa_id)
                                .ThenByDescending(sa => sa.data_criacao)
                                .Select(sa => new ChangeRequest(sa))
                                .ToListAsync();
        }

        public async Task<List<ChangeRequest>> GetAllByCooperative(Guid cooperativeId)
        {
            return await this._context.SolicitacaoAlteracao
                                .AsNoTracking()
                                .Where(sa => sa.cooperativa_id == cooperativeId
                                    && sa.ChamadaPublica.ativa
                                    && sa.ChamadaPublica.status_id <= (int)PublicCallStatusEnum.EmAndamento)
                                .OrderBy(sa => sa.chamada_publica_id)
                                .ThenByDescending(sa => sa.data_criacao)
                                .Select(sa => new ChangeRequest(sa))
                                .ToListAsync();
        }

        public async Task<List<ChangeRequest>> GetAllByPublicCall(Guid publicCallId)
        {
            return await this._context.SolicitacaoAlteracao
                                .AsNoTracking()
                                .Where(sa => sa.chamada_publica_id == publicCallId)
                                .OrderBy(sa => sa.cooperativa_id)
                                .ThenByDescending(sa => sa.data_criacao)
                                .Select(sa => new ChangeRequest(sa))
                                .ToListAsync();
        }

        public async Task Save(ChangeRequest changeRequest)
        {
            var toSave = await this._context.SolicitacaoAlteracao.FirstOrDefaultAsync(sa => sa.id == changeRequest.id);

            if (toSave == null)
            {
                toSave = new SolicitacaoAlteracao();
                toSave.data_criacao = DateTime.UtcNow.SetKindUtc();
                this._context.SolicitacaoAlteracao.Add(toSave);
            }

            toSave.id = changeRequest.id;
            toSave.chamada_publica_id = changeRequest.public_call_id;
            toSave.alimento_id = changeRequest.food_id;
            toSave.cooperativa_id = changeRequest.cooperative_id;
            toSave.mensagem = changeRequest.message;
            toSave.titulo = changeRequest.title;
            toSave.data_maxima_resposta = changeRequest.response_date?.SetKindUtc();
            toSave.exige_upload = changeRequest.requires_new_upload;
            toSave.is_invisivel = changeRequest.not_visible;
            toSave.is_resposta = changeRequest.is_response;
        }

        public void Dispose()
        {
            this._context.Dispose();
        }

        #endregion [ FIM - Metodos ]
    }
}
