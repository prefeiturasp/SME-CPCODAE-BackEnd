using FIA.SME.Aquisicao.Core.Helpers;

namespace FIA.SME.Aquisicao.Infrastructure.Models
{
    public class PublicCallAnswerReport
    {
        #region [ Construtores ]

        public PublicCallAnswerReport(PublicCallAnswer publicCallAnswer, List<PublicCallAnswerMember> members, List<IBGEDistrict.IBGEDistrictCity> allCities)
        {
            cooperative = publicCallAnswer.cooperative;
            cooperative_city = allCities.FirstOrDefault(c => c.id == cooperative.address.city_id)!;
            legal_representative_city = allCities.FirstOrDefault(c => c.id == cooperative.legal_representative.address.city_id)!;
            public_call_number = publicCallAnswer.public_call.number;

            food = publicCallAnswer.public_call.foods.Where(f => f.food_id == publicCallAnswer.food_id).First();
            public_call_members = members.OrderBy(m => m.member.name).ThenBy(m => m.member.dap_caf_code).ToList();

            total_members = publicCallAnswer.indigenous_community_total + publicCallAnswer.pnra_settlement_total + publicCallAnswer.quilombola_community_total + publicCallAnswer.other_family_agro_total;
            total_members_dap_fisica = publicCallAnswer.daps_fisicas_total;
        }

        #endregion [ FIM - Construtores ]

        #region [ Propriedades ]

        public Cooperative cooperative { get; private set; }

        public IBGEDistrict.IBGEDistrictCity cooperative_city          { get; private set; }
        public IBGEDistrict.IBGEDistrictCity legal_representative_city { get; private set; }

        public PublicCallFood food { get; private set; }

        public List<PublicCallAnswerMember> public_call_members { get; private set; } = new();

        public int total_members            { get; private set; }
        public int total_members_dap_fisica { get; private set; }
        public string public_call_number    { get; private set; } = String.Empty;


        #endregion [ FIM - Propriedades ]
    }
}
