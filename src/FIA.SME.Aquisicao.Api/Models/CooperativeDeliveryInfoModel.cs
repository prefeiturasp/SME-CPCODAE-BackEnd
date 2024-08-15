using FIA.SME.Aquisicao.Infrastructure.Interfaces;
using static FIA.SME.Aquisicao.Infrastructure.Models.PublicCallCooperativeDeliveryAggregated;

namespace FIA.SME.Aquisicao.Api.Models
{
    public class PublicCallCooperativeInfoResponse
    {
        #region [ Construtores ]

        public PublicCallCooperativeInfoResponse(IPublicCallCooperativeAggregated cooperative) : this(cooperative, 0, new List<PublicCallCooperativeDeliveryInfo>()) { }

        public PublicCallCooperativeInfoResponse(IPublicCallCooperativeAggregated cooperative, decimal total_delivered_percentage, List<PublicCallCooperativeDeliveryInfo> deliveries)
        {
            if (cooperative == null)
                return;

            this.cooperative_id = cooperative.cooperative_id;
            this.public_call_id = cooperative.public_call_id;
            this.public_call_answer_id = cooperative.public_call_answer_id;
            this.food_id = cooperative.food_id;
            this.acronym = cooperative.acronym;
            this.state_acronym = cooperative.state_acronym;
            this.name = cooperative.name;
            this.cnpj = cooperative.cnpj;
            this.color_class = cooperative.color_class;
            this.city_id = cooperative.cooperative_city_id;
            this.city_members_total = cooperative.city_members_total;
            this.daps_fisicas_total = cooperative.daps_fisicas_total;
            this.indigenous_community_total = cooperative.indigenous_community_total;
            this.pnra_settlement_total = cooperative.pnra_settlement_total;
            this.quilombola_community_total = cooperative.quilombola_community_total;
            this.other_family_agro_total = cooperative.other_family_agro_total;
            this.only_woman = cooperative.only_woman;
            this.location = cooperative.location;
            this.total_delivered = cooperative.total_delivered;
            this.total_delivered_percentage = total_delivered_percentage;
            this.total_proposal = cooperative.total_proposal;
            this.total_proposal_edited = cooperative.total_proposal_edited;
            this.total_price = cooperative.total_price;
            this.members_validated = cooperative.members_validated;
            this.was_chosen = cooperative.was_chosen;
            this.was_confirmed = cooperative.was_confirmed;

            var good_location = cooperative.location_score >= 4;
            var inclusive_cooperative = cooperative.percentage_inclusiveness / 100 >= 50 || cooperative.only_woman;
            var daps_percentage = cooperative.percentage_daps_fisicas >= 50;
            var total = (decimal)(cooperative.pnra_settlement_total + cooperative.indigenous_community_total + cooperative.quilombola_community_total + cooperative.other_family_agro_total);
            var daps_percentage_proportion = total == 0 ? 100 : (decimal)cooperative.daps_fisicas_total * 100 / total;
            this.classification = new CooperativeDeliveryInfoClassificationResponse(good_location, inclusive_cooperative, cooperative.proposal_is_organic, !cooperative.cooperative_is_central, daps_percentage, cooperative.location_score, cooperative.percentage_inclusiveness, daps_percentage_proportion);

            this.delivery_progress = deliveries.ConvertAll(d => new CooperativeDeliveryInfoProgressResponse(d.id, d.delivery_date, d.delivery_percentage, d.delivery_quantity, d.was_delivered, d.delivered_date, d.delivered_quantity));

            var enableToConfirm = true;

            this.delivery_progress.ForEach(dp =>
            {
                if (dp.was_delivered)
                    dp.enable_to_confirm = false;
                else
                {
                    if (enableToConfirm)
                        enableToConfirm = false;
                    else
                        dp.enable_to_confirm = false;
                }
            });
        }

        #endregion [ FIM - Construtores ]

        #region [ Propriedades ]

        public Guid cooperative_id                  { get; set; }
        public Guid public_call_id                  { get; set; }
        public Guid public_call_answer_id           { get; set; }
        public Guid food_id                         { get; set; }
        public string acronym                       { get; set; } = String.Empty;
        public string name                          { get; set; } = String.Empty;
        public string cnpj                          { get; set; } = String.Empty;
        public string color_class                   { get; set; } = String.Empty;
        public string state_acronym                 { get; set; } = String.Empty;
        public int city_id                          { get; set; } = 0;
        public int city_members_total               { get; set; } = 0;
        public int daps_fisicas_total               { get; set; } = 0;
        public int indigenous_community_total       { get; set; } = 0;
        public int pnra_settlement_total            { get; set; } = 0;
        public int quilombola_community_total       { get; set; } = 0;
        public int other_family_agro_total          { get; set; } = 0;
        public bool only_woman { get; set; } = false;
        public decimal total_delivered              { get; set; } = 0;
        public decimal total_delivered_percentage   { get; set; } = 0;
        public decimal total_proposal               { get; set; } = 0;
        public decimal? total_proposal_edited       { get; set; }
        public decimal total_price                  { get; set; } = 0;
        public string location                      { get; set; } = String.Empty;

        public bool members_validated               { get; set; } = false;
        public bool was_chosen                      { get; set; } = false;
        public bool was_confirmed                   { get; set; } = false;

        public CooperativeDeliveryInfoClassificationResponse classification     { get; set; }
        public List<CooperativeDeliveryInfoProgressResponse> delivery_progress  { get; set; } = new List<CooperativeDeliveryInfoProgressResponse>();

        #endregion [ FIM - Propriedades ]

        #region [ SubClasses ]

        public class CooperativeDeliveryInfoClassificationResponse
        {
            public CooperativeDeliveryInfoClassificationResponse(bool good_location, bool inclusive_cooperative, bool is_organic, bool is_singular, bool daps_percentage, int location_score, decimal inclusive_cooperative_percentage, 
                decimal daps_percentage_proportion)
            {
                this.good_location = good_location;
                this.inclusive_cooperative = inclusive_cooperative;
                this.is_organic = is_organic;
                this.is_singular = is_singular;
                this.daps_percentage = daps_percentage;

                this.location_score = location_score;
                this.inclusive_cooperative_percentage = inclusive_cooperative_percentage;
                this.daps_percentage_proportion = daps_percentage_proportion;
            }

            public bool good_location                       { get; set; }
            public bool inclusive_cooperative               { get; set; }
            public bool is_organic                          { get; set; }
            public bool is_singular { get; set; }
            public bool daps_percentage                     { get; set; }

            public decimal inclusive_cooperative_percentage { get; set; }
            public decimal daps_percentage_proportion       { get; set; }

            private int location_score                      { get; set; } = 0;
            public string good_location_type
            {
                get
                {
                    switch (this.location_score)
                    {
                        case 1:
                            return "Outro Estado";
                        case 2:
                            return "Mesmo Estado";
                        case 3:
                            return "Região Intermediária";
                        case 4:
                            return "Região Imediata";
                        case 5:
                            return "Mesma Cidade";

                        case 0:
                        default:
                            return "Fora de São Paulo";
                    }
                }
            }
        }

        public class CooperativeDeliveryInfoProgressResponse
        {
            public CooperativeDeliveryInfoProgressResponse(Guid id, DateTime deliveryDate, decimal deliveryPercentage, decimal deliveryQuantity, bool wasDelivered, DateTime? deliveredDate, decimal? deliveredQuantity)
            {
                this.id = id;
                this.delivery_date = deliveryDate.ToLocalTime();
                this.delivered_date = deliveredDate?.ToLocalTime();
                this.delivery_percentage = deliveryPercentage;
                this.delivery_quantity = deliveryQuantity;
                this.delivered_quantity = deliveredQuantity;
                this.was_delivered = wasDelivered;
            }

            public Guid id                      { get; set; }
            public DateTime delivery_date       { get; set; }
            public DateTime? delivered_date     { get; set; }
            public decimal delivery_percentage  { get; set; }
            public decimal delivery_quantity    { get; set; }
            public decimal? delivered_quantity  { get; set; }
            public bool was_delivered           { get; set; }

            public bool enable_to_confirm       { get; set; } = true;
        }

        #endregion [ FIM - SubClasses ]
    }
}
