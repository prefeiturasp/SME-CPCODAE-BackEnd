using FIA.SME.Aquisicao.Api.Validations;
using FIA.SME.Aquisicao.Core.Domain;

namespace FIA.SME.Aquisicao.Api.Models
{
    public class PublicCallAnswerProposalRequest
    {
        public Guid public_call_id              { get; set; }
        public string? change_request_title     { get; set; }
        public string? change_request_message   { get; set; }
        
        public List<PublicCallFoodAnswer> foods { get; set; }

        public class PublicCallAnswerProposalMemberRequest
        {
            public Guid? id             { get; set; }
            public string dap_caf_code  { get; set; }
            public string cpf           { get; set; }
            public decimal quantity     { get; set; }
            public decimal price        { get; set; }
        }

        public class CooperativeDocumentRegistration
        {
            public Guid document_type_id        { get; set; }
            public string document_type_name    { get; set; } = String.Empty;
            public string file_base_64          { get; set; } = String.Empty;
            public Int64 file_size              { get; set; }
            public int application              { get; set; }
        }

        public class PublicCallFoodAnswer 
        { 
            public Guid food_id                     { get; set; }
            public string food_name                 { get; set;} = String.Empty;
            public bool is_organic                  { get; set; }
            public int city_id                      { get; set; } = 0;
            public int city_members_total           { get; set; } = 0;
            public int daps_fisicas_total           { get; set; } = 0;
            public int indigenous_community_total   { get; set; } = 0;
            public int pnra_settlement_total        { get; set; } = 0;
            public int quilombola_community_total   { get; set; } = 0;
            public int other_family_agro_total      { get; set; } = 0;
            public List<CooperativeDocumentRegistration> documents      { get; set; }    
            public List<PublicCallAnswerProposalMemberRequest> members  { get; set; }
        }
    }

    public class PublicCallAnswerChooseRequest : BaseDomain
    {
        public List<PublicCallAnswerSelectedCooperative> selectedCooperatives { get; set; }

        public List<PublicCallDeliveryInfoModel> deliveries { get; set; } = new List<PublicCallDeliveryInfoModel>();

        public class PublicCallDeliveryInfoModel
        {
            public DateTime delivery_date { get; set; }
            public decimal delivery_quantity { get; set; }
        }

        public override bool EhValido()
        {
            ValidationResult = new PublicCallAnswerChooseValidation().Validate(this);
            return ValidationResult.IsValid;
        }
    }

    public class PublicCallAnswerSelectedCooperative : BaseDomain
    {
        public Guid public_call_answer_id   { get; set; }
        public decimal? new_quantity        { get; set; }

        public override bool EhValido()
        {
            ValidationResult = new PublicCallAnswerSelectedCooperativeValidation().Validate(this);
            return ValidationResult.IsValid;
        }
    }
}
