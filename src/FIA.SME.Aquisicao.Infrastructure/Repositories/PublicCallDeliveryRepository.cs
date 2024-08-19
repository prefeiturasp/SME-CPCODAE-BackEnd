using FIA.SME.Aquisicao.Infrastructure.Models;
using FIA.SME.Aquisicao.Infrastructure.Repositories.Context;
using FIA.SME.Aquisicao.Infrastructure.Interfaces;
using FIA.SME.Aquisicao.Infrastructure.Repositories.Types;
using Microsoft.EntityFrameworkCore;
using FIA.SME.Aquisicao.Core.Helpers;
using System.Linq;

namespace FIA.SME.Aquisicao.Infrastructure.Repositories
{
    public interface IPublicCallDeliveryRepository : IRepository
    {
        void Delete(Guid id);
        Task DeleteByPublicCallAnswerId(Guid publicCallAnswerId);
        Task<PublicCallDeliveryInfo?> Get(Guid id, bool keepTrack);
        Task<List<PublicCallDeliveryInfo>> GetAll();
        Task<List<PublicCallDeliveryInfo>> GetAllByPublicCallId(Guid publicCallId);
        Task<List<PublicCallDeliveryInfo>> GetAllByPublicCallAnswerId(Guid publicCallAnswerId);
        Task<List<PublicCallDeliveryInfo>> GetAllCooperativesAvailablesForBeChosen(Guid publicCallId);
        Task<List<PublicCallDeliveryInfo>> GetAllCooperativesDeliveryInfo(Guid publicCallId);
        Task<List<PublicCallDeliveryInfo>> GetAllDeliveryInfoByPublicCallAnswerId(Guid publicCallAnswerId);
        Task Save(PublicCallDeliveryInfo delivery);
    }

    internal class PublicCallDeliveryRepository : IPublicCallDeliveryRepository
    {
        #region [ Propriedades ]

        private readonly SMEContext _context;
        public IUnitOfWork UnitOfWork => _context;

        #endregion [ FIM - Propriedades ]

        #region [ Construtores ]

        public PublicCallDeliveryRepository(SMEContext context)
        {
            this._context = context;
        }

        #endregion [ FIM - Construtores ]

        #region [ Metodos ]

        public void Delete(Guid id)
        {
            var toDelete = this._context.ChamadaPublicaEntrega.Where(cpe => cpe.id == id).FirstOrDefault();

            if (toDelete == null)
                return;

            this._context.ChamadaPublicaEntrega.Remove(toDelete);
        }

        public async Task DeleteByPublicCallAnswerId(Guid publicCallAnswerId)
        {
            var entregas = this._context.ChamadaPublicaEntrega.Where(c => c.chamada_publica_resposta_id == publicCallAnswerId);

            foreach (var entrega in entregas)
            {
                this._context.ChamadaPublicaEntrega.Remove(entrega);
            }
        }

        public async Task<PublicCallDeliveryInfo?> Get(Guid id, bool keepTrack)
        {
            var query = this._context.ChamadaPublicaEntrega!.Include(cpe => cpe.ChamadaPublicaResposta).Where(ei => ei.id == id);

            if (!keepTrack)
                query = query.AsNoTracking();

            var deliveryInfo = await query.FirstOrDefaultAsync();

            return (deliveryInfo != null) ? new PublicCallDeliveryInfo(deliveryInfo) : null;
        }

        public async Task<List<PublicCallDeliveryInfo>> GetAll()
        {
            return await this._context.ChamadaPublicaEntrega
                                        .Include(cpe => cpe.ChamadaPublicaResposta)
                                        .AsNoTracking()
                                        .Select(cpe => new PublicCallDeliveryInfo(cpe, true))
                                        .ToListAsync();
        }

        public async Task<List<PublicCallDeliveryInfo>> GetAllByPublicCallId(Guid publicCallId)
        {
            return await this._context.ChamadaPublicaEntrega
                                        .Where(cpe => cpe.ChamadaPublicaResposta.chamada_publica_id == publicCallId)
                                        .AsNoTracking()
                                        .Select(cpe => new PublicCallDeliveryInfo(cpe, true))
                                        .ToListAsync();
        }

        public async Task<List<PublicCallDeliveryInfo>> GetAllByPublicCallAnswerId(Guid publicCallAnswerId)
        {
            return await this._context.ChamadaPublicaEntrega
                                        .Where(cpe => cpe.ChamadaPublicaResposta.id == publicCallAnswerId)
                                        .AsNoTracking()
                                        .Select(cpe => new PublicCallDeliveryInfo(cpe, true))
                                        .ToListAsync();
        }

        public async Task<List<PublicCallDeliveryInfo>> GetAllCooperativesAvailablesForBeChosen(Guid publicCallId)
        {
            return (await this._context.ChamadaPublicaResposta
                                        .Include(cpr => cpr.Alimento)
                                        .Include(cpr => cpr.ChamadaPublica)
                                        .Include(cpr => cpr.Cooperativa.Endereco)
                                        .Where(cpr => cpr.chamada_publica_id == publicCallId)
                                        .AsNoTracking()
                                        .Distinct()
                                        .ToListAsync())
                                        .ConvertAll(cpr => new PublicCallDeliveryInfo(cpr));
        }

        public async Task<List<PublicCallDeliveryInfo>> GetAllCooperativesDeliveryInfo(Guid publicCallId)
        {
            return await this._context.ChamadaPublicaEntrega
                                        .Include(cpe => cpe.ChamadaPublicaResposta.Alimento)
                                        .Include(cpe => cpe.ChamadaPublicaResposta.ChamadaPublica)
                                        .Include(cpe => cpe.ChamadaPublicaResposta.Cooperativa.Endereco)
                                        .Where(cpe => cpe.ChamadaPublicaResposta.chamada_publica_id == publicCallId)
                                        .AsNoTracking()
                                        .Select(cpe => new PublicCallDeliveryInfo(cpe, true))
                                        .ToListAsync();
        }

        public async Task<List<PublicCallDeliveryInfo>> GetAllDeliveryInfoByPublicCallAnswerId(Guid publicCallAnswerId)
        {
            return await this._context.ChamadaPublicaEntrega
                                        .Where(cpe => cpe.chamada_publica_resposta_id == publicCallAnswerId)
                                        .OrderBy(cpe => cpe.data_prevista_entrega)
                                        .AsNoTracking()
                                        .Select(cpe => new PublicCallDeliveryInfo(cpe, true))
                                        .ToListAsync();
        }

        public async Task Save(PublicCallDeliveryInfo delivery)
        {
            var toSave = await this._context.ChamadaPublicaEntrega.FirstOrDefaultAsync(c => c.id == delivery.id);

            if (toSave == null)
            {
                toSave = new ChamadaPublicaEntrega();
                toSave.data_criacao = DateTime.UtcNow.SetKindUtc();

                this._context.ChamadaPublicaEntrega.Add(toSave);
            }

            toSave.id = delivery.id;
            toSave.chamada_publica_resposta_id = delivery.public_call_answer_id;
            toSave.usuario_confirmacao_entrega_id = delivery.delivered_confirmation_user_id;
            toSave.quantidade_prevista_entrega = delivery.delivery_quantity;
            toSave.quantidade_entregue = delivery.delivered_quantity;
            toSave.data_prevista_entrega = delivery.delivery_date.SetKindUtc();
            toSave.data_confirmacao_entrega = delivery.delivered_confirmation_date?.SetKindUtc();
            toSave.foi_entregue = delivery.was_delivered;
        }

        public void Dispose()
        {
            this._context.Dispose();
        }

        #endregion [ FIM - Metodos ]
    }
}
