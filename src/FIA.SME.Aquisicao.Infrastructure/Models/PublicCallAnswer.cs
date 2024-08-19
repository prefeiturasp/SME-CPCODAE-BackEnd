using FIA.SME.Aquisicao.Core.Enums;
using FIA.SME.Aquisicao.Infrastructure.Repositories.Types;

namespace FIA.SME.Aquisicao.Infrastructure.Models
{
    public class PublicCallAnswer
    {
        #region [ Construtores ]

        internal PublicCallAnswer() { }

        public PublicCallAnswer(Guid id, Guid cooperative_id, Guid food_id, Guid public_call_id, int city_id, decimal price, decimal quantity, bool is_organic, int city_members_total,
            int daps_fisicas_total, int indigenous_community_total, int pnra_settlement_total, int quilombola_community_total, int other_family_agro_total, bool only_woman)
        {
            this.id = id;
            this.cooperative_id = cooperative_id;
            this.food_id = food_id;
            this.public_call_id = public_call_id;
            this.city_id = city_id;
            this.price = price;
            this.quantity = quantity;
            this.is_organic = is_organic;
            this.city_members_total = city_members_total;
            this.daps_fisicas_total = daps_fisicas_total;
            this.indigenous_community_total = indigenous_community_total;
            this.pnra_settlement_total = pnra_settlement_total;
            this.quilombola_community_total = quilombola_community_total;
            this.other_family_agro_total = other_family_agro_total;
            this.only_woman = only_woman;
            this.is_active = true;
        }

        internal PublicCallAnswer(ChamadaPublicaResposta chamadaPublicaResposta)
        {
            this.id = chamadaPublicaResposta.id;
            this.cooperative_id = chamadaPublicaResposta.cooperativa_id;
            this.food_id = chamadaPublicaResposta.alimento_id;
            this.public_call_id = chamadaPublicaResposta.chamada_publica_id;
            this.city_id = chamadaPublicaResposta.codigo_cidade_ibge;
            this.city_members_total = chamadaPublicaResposta.total_associados_cidade;
            this.daps_fisicas_total = chamadaPublicaResposta.total_daps_fisicas;
            this.indigenous_community_total = chamadaPublicaResposta.total_comunidade_indigena;
            this.pnra_settlement_total = chamadaPublicaResposta.total_assentamento_pnra;
            this.quilombola_community_total = chamadaPublicaResposta.total_comunidade_quilombola;
            this.other_family_agro_total = chamadaPublicaResposta.total_outros_agricultores_familiares;
            this.only_woman = chamadaPublicaResposta.somente_mulheres;
            this.price = chamadaPublicaResposta.preco;
            this.quantity = chamadaPublicaResposta.quantidade;
            this.quantity_edited = chamadaPublicaResposta.quantidade_editada;
            this.is_organic = chamadaPublicaResposta.organico;
            this.is_active = chamadaPublicaResposta.ativa;
            this.was_chosen = chamadaPublicaResposta.foi_escolhida;
            this.was_confirmed = chamadaPublicaResposta.foi_confirmada;
            this.members_validated = chamadaPublicaResposta.validada;

            if (chamadaPublicaResposta.Alimento != null)
            {
                this.food = new Food(chamadaPublicaResposta.Alimento);
            }

            if (chamadaPublicaResposta.ChamadaPublica != null)
            {
                this.public_call = new PublicCall(chamadaPublicaResposta.ChamadaPublica);
            }

            if (chamadaPublicaResposta.Cooperativa != null)
            {
                this.cooperative = new Cooperative(chamadaPublicaResposta.Cooperativa);
            }

            if (chamadaPublicaResposta.ChamadaPublicaEntregas != null && chamadaPublicaResposta.ChamadaPublicaEntregas.Any())
            {
                this._deliveries = chamadaPublicaResposta.ChamadaPublicaEntregas.Select(cpe => new PublicCallDeliveryInfo(cpe, false)).ToList();
            }
        }

        #endregion [ FIM - Construtores ]

        #region [ Propriedades ]

        public Guid id                          { get; private set; }
        public Guid cooperative_id              { get; private set; }
        public Guid food_id                     { get; private set; }
        public Guid public_call_id              { get; private set; }
        public int city_id                      { get; private set; }
        public int city_members_total           { get; private set; }
        public int daps_fisicas_total           { get; private set; }
        public int indigenous_community_total   { get; private set; }
        public int pnra_settlement_total        { get; private set; }
        public int quilombola_community_total   { get; private set; }
        public int other_family_agro_total      { get; private set; }
        public bool only_woman                  { get; private set; }
        public decimal price                    { get; private set; }
        public decimal quantity                 { get; private set; }
        public decimal? quantity_edited         { get; private set; }
        public MeasureUnitEnum measure_unit     { get; private set; }
        public bool is_organic                  { get; private set; }
        public bool is_active                   { get; private set; }
        public bool was_chosen                  { get; private set; }
        public bool was_confirmed               { get; private set; }
        public bool members_validated           { get; private set; }

        public Cooperative cooperative          { get; private set; }
        public Food food                        { get; private set; }
        public PublicCall public_call           { get; private set; }

        private List<PublicCallDeliveryInfo> _deliveries;
        public IReadOnlyCollection<PublicCallDeliveryInfo> deliveries => this._deliveries;

        #endregion [ FIM - Propriedades ]

        #region [ Metodos ]

        public void SetAsChosen(decimal? quantity_edited)
        {
            this.was_chosen = true;
            this.quantity_edited = quantity_edited;
        }

        public void SetAsConfirmed()
        {
            this.was_confirmed = true;
        }

        public void SetAsInvalid()
        {
            this.members_validated = false;
        }
        public void SetAsUnChosen()
        {
            this.was_chosen = false;
        }

        public void SetId(Guid id)
        {
            this.id = id;
        }

        public void SetWasValidated(bool wasValidated)
        {
            this.members_validated = wasValidated;
        }

        public void UpdateMembersInfo(int city_id, int daps_fisicas_total, int indigenous_community_total, int pnra_settlement_total, int quilombola_community_total, int other_family_agro_total, bool only_woman)
        {
            this.city_id = city_id;
            this.daps_fisicas_total = daps_fisicas_total;
            this.indigenous_community_total = indigenous_community_total;
            this.pnra_settlement_total = pnra_settlement_total;
            this.quilombola_community_total = quilombola_community_total;
            this.other_family_agro_total = other_family_agro_total;
            this.only_woman = only_woman;
        }

        #endregion [ FIM - Metodos ]
    }
}
