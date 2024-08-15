using FIA.SME.Aquisicao.Core.Helpers;
using FIA.SME.Aquisicao.Infrastructure.Interfaces;

namespace FIA.SME.Aquisicao.Infrastructure.Models
{
    public class PublicCallCooperativeToBeChosenAggregated : IPublicCallCooperativeAggregated
    {
        #region [ Construtores ]

        public PublicCallCooperativeToBeChosenAggregated(PublicCallAnswer answer, string cooperativeCityName, string cooperativeStateAcronym, decimal total_proposal, Guid food_id)
        {
            if (answer == null || answer.cooperative == null)
                return;

            this.public_call_answer_id = answer.id;
            this.public_call_id = answer.public_call_id;
            this.cooperative_id = answer.cooperative.id;
            this.food_id = food_id;
            this.name = answer.cooperative.name!;
            this.acronym = answer.cooperative.acronym!;
            this.cnpj = answer.cooperative.cnpj.FormatCNPJ();
            this.city_members_total = answer.city_members_total;
            this.daps_fisicas_total = answer.daps_fisicas_total;
            this.indigenous_community_total = answer.indigenous_community_total;
            this.pnra_settlement_total = answer.pnra_settlement_total;
            this.quilombola_community_total = answer.quilombola_community_total;
            this.other_family_agro_total = answer.other_family_agro_total;
            this.only_woman = answer.only_woman;
            this.total_price = answer.price;
            this.proposal_is_organic = answer.is_organic;
            this.cooperative_is_central = answer.cooperative.pj_type == Core.Enums.CooperativePJTypeEnum.CentralCooperative;
            this.cooperative_city_id = answer.city_id;
            this.proposal_city_id = answer.public_call.city_id;
            this.total_proposal = total_proposal;
            this.total_proposal_edited = null;
            this.total_delivered = 0;
            this.members_validated = answer.members_validated;
            this.was_chosen = answer.was_chosen;
            this.was_confirmed = answer.was_confirmed;

            this.cooperative_city_name = cooperativeCityName;
            this.state_acronym = cooperativeStateAcronym;
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
        public int city_members_total           { get; init; }
        public int daps_fisicas_total           { get; init; } = 0;
        public int indigenous_community_total   { get; init; } = 0;
        public int pnra_settlement_total        { get; init; } = 0;
        public int quilombola_community_total   { get; init; } = 0;
        public int other_family_agro_total      { get; init; } = 0;
        public bool only_woman { get; init; }
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
        public bool was_confirmed               { get; init; }

        private string cooperative_city_name                { get; init; }
        public string state_acronym             { get; init; }

        public string location                  { get { return String.IsNullOrEmpty(this.cooperative_city_name) ? String.Empty : (String.IsNullOrEmpty(this.state_acronym) ? this.cooperative_city_name : $"{this.cooperative_city_name}/{this.state_acronym}"); } }

        public decimal percentage_daps_fisicas
        {
            get
            {
                var total = this.indigenous_community_total + this.quilombola_community_total + this.pnra_settlement_total + this.other_family_agro_total;

                if (total == 0)
                    return 0;

                return (decimal)this.daps_fisicas_total * 100 / (decimal)total;
            }
        }

        public decimal percentage_inclusiveness
        {
            get
            {
                var total_inclusiveness = this.indigenous_community_total + this.quilombola_community_total + this.pnra_settlement_total;
                var total = total_inclusiveness + this.other_family_agro_total;

                if (total == 0)
                    return 0;

                //return (decimal)total_inclusiveness * 100 / (decimal)total;
                return (decimal)total_inclusiveness * 100;
            }
        }

        #endregion [ FIM - Propriedades ]

        #region [ Metodos ]

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
    }
}
