using FIA.SME.Aquisicao.Core.Helpers;
using FIA.SME.Aquisicao.Infrastructure.Interfaces;

namespace FIA.SME.Aquisicao.Infrastructure.Models
{
    public class PublicCallCooperativeDeliveryAggregated : IPublicCallCooperativeAggregated
    {
        #region [ Construtores ]

        public PublicCallCooperativeDeliveryAggregated(PublicCallAnswer answer, string cooperativeCityName, string cooperativeStateAcronym)
        {
            if (answer == null || answer.cooperative == null)
                return;

            this.public_call_answer_id = answer.id;
            this.public_call_id = answer.public_call_id;
            this.cooperative_id = answer.cooperative.id;
            this.food_id = answer.food_id;
            this.acronym = answer.cooperative.acronym!;
            this.name = answer.cooperative.name!;
            this.cnpj = answer.cooperative.cnpj.FormatCNPJ();
            this.city_members_total = answer.city_members_total;
            this.daps_fisicas_total = answer.daps_fisicas_total;
            this.indigenous_community_total = answer.indigenous_community_total;
            this.pnra_settlement_total = answer.pnra_settlement_total;
            this.quilombola_community_total = answer.quilombola_community_total;
            this.other_family_agro_total = answer.other_family_agro_total;
            this.total_price = answer.price;
            this.proposal_is_organic = answer.is_organic;
            this.cooperative_is_central = answer.cooperative.pj_type == Core.Enums.CooperativePJTypeEnum.CentralCooperative;
            //this.cooperative_city_id = answer.cooperative.address.city_id;
            this.cooperative_city_id = answer.city_id;
            this.proposal_city_id = answer.public_call.city_id;
            this.members_validated = answer.members_validated;
            this.was_chosen = answer.was_chosen;

            this.city_name = String.IsNullOrEmpty(cooperativeCityName) ? "Fora de São Paulo" : cooperativeCityName;
            this.state_acronym = cooperativeStateAcronym;

            this.deliveries = new List<PublicCallCooperativeDeliveryInfo>();
        }

        #endregion [ FIM - Construtores ]

        #region [ Propriedades ]

        public Guid cooperative_id              { get; init; }
        public Guid public_call_id              { get; init; }
        public Guid public_call_answer_id       { get; init; }
        public Guid food_id                     { get; init; }
        public string acronym                   { get; init; }
        public string name                      { get; init; }
        public string cnpj                      { get; init; }
        public string color_class               { get; private set; }
        public int city_members_total           { get; init; } = 0;
        public int daps_fisicas_total           { get; init; } = 0;
        public int indigenous_community_total   { get; init; } = 0;
        public int pnra_settlement_total        { get; init; } = 0;
        public int quilombola_community_total   { get; init; } = 0;
        public int other_family_agro_total      { get; init; } = 0;
        public decimal total_delivered          { get; private set; }
        public decimal total_proposal           { get; private set; }
        public decimal? total_proposal_edited   { get; private set; }
        public decimal total_price              { get; private set; }
        public int location_score               { get; private set; }
        public bool proposal_is_organic         { get; init; }
        public bool cooperative_is_central      { get; init; }
        public int cooperative_city_id          { get; init; }
        public int proposal_city_id             { get; init; }
        public bool members_validated           { get; init; }
        public bool was_chosen                  { get; init; }

        private string city_name                { get; init; }
        private string state_acronym            { get; init; }

        public string location                  { get { return String.IsNullOrEmpty(this.city_name) ? String.Empty : (String.IsNullOrEmpty(this.state_acronym) ? this.city_name : $"{this.city_name}/{this.state_acronym}"); } }

        public decimal percentage_daps_fisicas
        {
            get
            {
                if (this.total == 0)
                    return 0;

                return (decimal)this.daps_fisicas_total * 100 / (decimal)this.total;
            }
        }

        public decimal percentage_inclusiveness
        {
            get
            {
                var total_inclusiveness = this.indigenous_community_total + this.quilombola_community_total + this.pnra_settlement_total;

                if (this.total == 0)
                    return 0;

                return (decimal)total_inclusiveness * 100 / (decimal)this.total;
            }
        }

        public decimal total_delivered_percentage { get { return this.total_proposal <= 0 ? 0 : Math.Round((decimal)this.total_delivered * 100 / this.total_proposal, 2); } }

        public List<PublicCallCooperativeDeliveryInfo> deliveries { get; private set; }

        private int total => this.indigenous_community_total + this.quilombola_community_total + this.pnra_settlement_total + this.other_family_agro_total;

        #endregion [ FIM - Propriedades ]

        #region [ Metodos ]

        public void AddDeliveryInfo(Guid deliveryId, DateTime deliveryDate, decimal deliveryQuantity, bool wasDelivered, DateTime? deliveredDate, decimal? deliveredQuantity)
        {
            if (this.deliveries == null)
                this.deliveries = new List<PublicCallCooperativeDeliveryInfo>();

            this.deliveries.Add(new PublicCallCooperativeDeliveryInfo(deliveryId, deliveryDate, deliveryQuantity, wasDelivered, deliveredDate, deliveredQuantity));
        }

        public void SetTotalDelivered(decimal totalDelivered)
        {
            this.total_delivered = totalDelivered;
            this.color_class = PublicCall.GetColorClass(this.total_delivered_percentage);

            decimal currentQuantity = 0;

            foreach (var item in this.deliveries)
            {
                currentQuantity += (item.delivered_quantity ?? item.delivery_quantity);

                var percentage = this.total_proposal <= 0 ? 0 : Math.Round((decimal)currentQuantity * 100 / this.total_proposal, 2);

                if (percentage > 100)
                    percentage = 99.99m;

                item.SetPercentage(percentage);
            }
        }

        public void SetTotalProposal(decimal totalProposal)
        {
            this.total_proposal = totalProposal;
        }

        public void SetLocationScore(List<LocationRegion> locationRegions)
        {
            if (this.cooperative_city_id == this.proposal_city_id)
            {
                this.location_score = 5;
                return;
            }

            var locationRegionCooperativeCity = locationRegions.FirstOrDefault(lr => lr.id == this.cooperative_city_id);
            var locationRegionProposalCity = locationRegions.FirstOrDefault(lr => lr.id == this.proposal_city_id);

            if (locationRegionCooperativeCity == null || locationRegionProposalCity == null)
            {
                this.location_score = 0;
                return;
            }

            if (locationRegionCooperativeCity.imediate_region_id == locationRegionProposalCity.imediate_region_id)
            {
                this.location_score = 4;
                return;
            }

            if (locationRegionCooperativeCity.intermediate_region_id == locationRegionProposalCity.intermediate_region_id)
            {
                this.location_score = 3;
                return;
            }

            if (locationRegionCooperativeCity.state_acronym == locationRegionProposalCity.state_acronym)
            {
                this.location_score = 2;
                return;
            }

            this.location_score = 1;
        }

        #endregion [ FIM - Metodos ]

        #region [ SubClasses ]

        public class PublicCallCooperativeDeliveryInfo
        {
            public PublicCallCooperativeDeliveryInfo(Guid id, DateTime deliveryDate, decimal deliveryQuantity, bool wasDelivered, DateTime? deliveredDate, decimal? deliveredQuantity)
            {
                this.id = id;
                this.delivery_date = deliveryDate;
                this.delivered_date = deliveredDate;
                this.delivery_quantity = deliveryQuantity;
                this.delivered_quantity = deliveredQuantity;
                this.was_delivered = wasDelivered;
            }

            public Guid id                      { get; init; }
            public DateTime delivery_date       { get; init; }
            public DateTime? delivered_date     { get; init; }
            public decimal delivery_quantity    { get; init; }
            public decimal? delivered_quantity  { get; init; }
            public bool was_delivered           { get; init; }

            public decimal delivery_percentage  { get; private set; }

            public void SetPercentage(decimal percentage)
            {
                this.delivery_percentage = percentage;
            }
        }

        #endregion [ FIM - SubClasses ]
    }
}
