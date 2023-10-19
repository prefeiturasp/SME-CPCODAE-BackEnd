namespace FIA.SME.Aquisicao.Infrastructure.Interfaces
{
    public interface IPublicCallCooperativeAggregated
    {
        Guid cooperative_id                 { get; init; }
        Guid public_call_id                 { get; init; }
        Guid public_call_answer_id          { get; init; }
        Guid food_id                        { get; init; }
        string name                         { get; init; }
        string color_class                  { get; }
        int city_members_total              { get; init; }
        int daps_fisicas_total              { get; init; }
        int indigenous_community_total      { get; init; }
        int pnra_settlement_total           { get; init; }
        int quilombola_community_total      { get; init; }
        int other_family_agro_total         { get; init; }
        decimal total_delivered             { get; }
        decimal total_proposal              { get; }
        decimal? total_proposal_edited      { get; }
        decimal total_price                 { get; }
        int location_score                  { get; }
        bool proposal_is_organic            { get; init; }
        bool cooperative_is_central         { get; init; }
        int cooperative_city_id             { get; init; }
        int proposal_city_id                { get; init; }

        decimal percentage_daps_fisicas     { get; }
        decimal percentage_inclusiveness    { get; }

        string location                     { get; }
        bool members_validated              { get; }
        bool was_chosen                     { get; }
    }
}
