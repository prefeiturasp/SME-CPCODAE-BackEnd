using FIA.SME.Aquisicao.Core.Helpers;
using FIA.SME.Aquisicao.Infrastructure.Repositories.Types;

namespace FIA.SME.Aquisicao.Infrastructure.Models
{
    public class PublicCallDeliveryInfo
    {
        #region [ Construtores ]

        public PublicCallDeliveryInfo(Guid id, Guid public_call_answer_id, DateTime delivery_date, decimal delivery_quantity)
        {
            this.id = id;
            this.public_call_answer_id = public_call_answer_id;
            this.delivery_date = delivery_date;
            this.delivery_quantity = delivery_quantity;
        }

        public PublicCallDeliveryInfo(Guid public_call_answer_id, DateTime delivery_date, decimal delivery_quantity)
        {
            this.id = Guid.NewGuid();
            this.public_call_answer_id = public_call_answer_id;
            this.delivery_date = delivery_date.ConvertToSolveUTC();
            this.delivery_quantity = delivery_quantity;
            this.creation_date = DateTime.Today.ConvertToSolveUTC();
            this.delivered_confirmation_user_id = null;
            this.delivered_quantity = null;
            this.delivered_confirmation_date = null;
            this.was_delivered = false;
        }

        internal PublicCallDeliveryInfo(ChamadaPublicaEntrega chamadaPublicaEntrega, bool loadChamadaPublicaResposta = true)
        {
            if (chamadaPublicaEntrega == null)
                return;

            this.id = chamadaPublicaEntrega.id;
            this.public_call_answer_id = chamadaPublicaEntrega.chamada_publica_resposta_id;
            this.delivered_confirmation_user_id = chamadaPublicaEntrega.usuario_confirmacao_entrega_id;
            this.delivery_quantity = chamadaPublicaEntrega.quantidade_prevista_entrega;
            this.delivered_quantity = chamadaPublicaEntrega.quantidade_entregue;
            this.creation_date = chamadaPublicaEntrega.data_criacao;
            this.delivery_date = chamadaPublicaEntrega.data_prevista_entrega;
            this.delivered_confirmation_date = chamadaPublicaEntrega.data_confirmacao_entrega;
            this.was_delivered = chamadaPublicaEntrega.foi_entregue;

            if (loadChamadaPublicaResposta && chamadaPublicaEntrega.ChamadaPublicaResposta != null)
            {
                this.answer = new PublicCallAnswer(chamadaPublicaEntrega.ChamadaPublicaResposta);
                this.food_id = chamadaPublicaEntrega.ChamadaPublicaResposta.alimento_id;
            }
        }

        internal PublicCallDeliveryInfo(ChamadaPublicaResposta chamadaPublicaResposta)
        {
            if (chamadaPublicaResposta == null)
                return;

            this.id = chamadaPublicaResposta.id;
            this.public_call_answer_id = chamadaPublicaResposta.id;
            this.food_id = chamadaPublicaResposta.alimento_id;
            this.delivery_quantity = chamadaPublicaResposta.quantidade;
            this.creation_date = DateTime.Today;
            this.delivery_date = DateTime.Today;
            this.was_chosen = chamadaPublicaResposta.foi_escolhida;
            this.was_delivered = false;

            this.delivered_confirmation_user_id = null;
            this.delivered_quantity = null;
            this.delivered_confirmation_date = null;

            this.answer = new PublicCallAnswer(chamadaPublicaResposta);
        }

        #endregion [ FIM - Construtores ]

        #region [ Propriedades ]

        public Guid id                                  { get; private set; }
        public Guid public_call_answer_id               { get; private set; }
        public Guid? delivered_confirmation_user_id     { get; private set; }
        public Guid food_id                             { get; private set; }
        public decimal delivery_quantity                { get; private set; }
        public decimal? delivered_quantity              { get; private set; }
        public DateTime creation_date                   { get; private set; }
        public DateTime delivery_date                   { get; private set; }
        public DateTime? delivered_confirmation_date    { get; private set; }
        public bool was_chosen                          { get; private set; }
        public bool was_delivered                       { get; private set; }

        public PublicCallAnswer answer                  { get; private set; }

        #endregion [ FIM - Propriedades ]

        #region [ Metodos ]

        public void RemoveDelivery()
        {
            this.delivered_confirmation_user_id = null;
            this.delivered_confirmation_date = null;
            this.delivered_quantity = null;
            this.was_delivered = false;
        }

        public void SetDelivery(Guid loggedUserId, DateTime deliveredDate, decimal deliveredQuantity)
        {
            this.delivered_confirmation_user_id = loggedUserId;
            this.delivered_confirmation_date = deliveredDate;
            this.delivered_quantity = deliveredQuantity;
            this.was_delivered = true;
        }

        public void SetId(Guid id)
        {
            this.id = id;
        }

        #endregion [ FIM - Metodos ]
    }
}
