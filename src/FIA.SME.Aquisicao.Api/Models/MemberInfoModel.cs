namespace FIA.SME.Aquisicao.Api.Models
{
    public class MemberInfoRequestModel
    {
        public int city_id { get; set; } = 0;
        public int daps_fisicas_total { get; set; } = 0;
        public int indigenous_community_total { get; set; } = 0;
        public int pnra_settlement_total { get; set; } = 0;
        public int quilombola_community_total { get; set; } = 0;
        public int other_family_agro_total { get; set; } = 0;
        public bool only_woman { get; set; } = false;
    }
}
