using FIA.SME.Aquisicao.Infrastructure.Models;

namespace FIA.SME.Aquisicao.Api.Models
{
    public class DapMemberResponse
    {
        public DapMemberResponse(CooperativeMember? cooperativeMember)
        {
            if (cooperativeMember == null)
                return;

            this.dap_caf_code = cooperativeMember.dap_caf_code;
            this.name = cooperativeMember.name;
            this.dap_caf_registration_date = cooperativeMember.dap_caf_registration_date?.ToLocalTime();
            this.dap_caf_expiration_date = cooperativeMember.dap_caf_expiration_date?.ToLocalTime();
            this.is_dap = cooperativeMember.is_dap;
        }

        public string dap_caf_code                  { get; set; }
        public string? name                         { get; set; }
        public DateTime? dap_caf_registration_date  { get; set; }
        public DateTime? dap_caf_expiration_date    { get; set; }
        public bool is_dap                          { get; set; }
    }

    public class CooperativeMemberResponse
    {
        public CooperativeMemberResponse(CooperativeMember? cooperativeMember)
        {
            if (cooperativeMember == null)
                return;

            this.id = cooperativeMember.id;
            this.cooperative_id = cooperativeMember.cooperative_id;
            this.address = new AddressResponse(cooperativeMember.address);
            this.cpf = cooperativeMember.cpf;
            this.dap_caf_code = cooperativeMember.dap_caf_code;
            this.name = cooperativeMember.name;
            this.pf_type = cooperativeMember.pf_type.HasValue ? (int)cooperativeMember.pf_type : null;
            this.production_type = cooperativeMember.production_type.HasValue ? (int)cooperativeMember.production_type : null;
            this.dap_caf_registration_date = cooperativeMember.dap_caf_registration_date?.ToLocalTime();
            this.dap_caf_expiration_date = cooperativeMember.dap_caf_expiration_date?.ToLocalTime();
            this.is_dap = cooperativeMember.is_dap;
            this.is_male = cooperativeMember.is_male;
            this.is_active = cooperativeMember.is_active;
        }

        public Guid id                              { get; set; }
        public Guid? cooperative_id                 { get; set; }
        public AddressResponse? address             { get; set; }
        public string? cpf                          { get; set; }
        public string dap_caf_code                  { get; set; }
        public string? name                         { get; set; }
        public int? pf_type                         { get; set; }
        public int? production_type                 { get; set; }
        public DateTime? dap_caf_registration_date  { get; set; }
        public DateTime? dap_caf_expiration_date    { get; set; }
        public bool is_dap                          { get; set; }
        public bool is_male                         { get; set; }
        public bool is_active                       { get; set; }
    }

    public class CooperativeMemberRegister
    {
        public Guid? cooperative_id                 { get; set; }
        public AddressUpdate address                { get; set; } = new AddressUpdate();
        public string cpf                           { get; set; } = String.Empty;
        public string dap_caf_code                  { get; set; } = String.Empty;
        public string name                          { get; set; } = String.Empty;
        public int pf_type                          { get; set; }
        public int production_type                  { get; set; }
        public DateTime dap_caf_registration_date   { get; set; }
        public DateTime dap_caf_expiration_date     { get; set; }
        public bool is_dap                          { get; set; }
        public bool is_male                         { get; set; }
    }

    public class CooperativeMemberUpdate
    {
        public Guid id                              { get; set; }
        public Guid? cooperative_id                 { get; set; }
        public AddressUpdate? address               { get; set; }
        public string? cpf                          { get; set; }
        public string dap_caf_code                  { get; set; } = String.Empty;
        public string? name                         { get; set; }
        public int? pf_type                         { get; set; }
        public int? production_type                 { get; set; }
        public DateTime? dap_caf_registration_date  { get; set; }
        public DateTime? dap_caf_expiration_date    { get; set; }
        public bool is_dap                          { get; set; }
        public bool is_male                         { get; set; }
        public bool is_active                       { get; set; }
    }

    public class CooperativeMemberToValidateRequest
    {
        public List<string> dap_caf_code_list       { get; set; } = new List<string>();
    }

    public class CooperativeMemberToValidateResponse
    {
        public CooperativeMemberToValidateResponse(Guid? id, string? cpf, string? dap_caf_code, string? name, int line_number, decimal total_year_supplied_value, string message, bool is_success)
        {
            this.id = id;
            this.cpf = cpf;
            this.dap_caf_code = dap_caf_code;
            this.name = name;
            this.line_number = line_number;
            this.total_year_supplied_value = total_year_supplied_value;
            this.message = message;
            this.is_success = is_success;
        }

        public Guid? id                             { get; set; }
        public string? cpf                          { get; set; } = String.Empty;
        public string? dap_caf_code                 { get; set; } = String.Empty;
        public string? name                         { get; set; }
        public int line_number                      { get; set; }
        public decimal total_year_supplied_value    { get; set; }

        public string message                       { get; set; } = String.Empty;
        public bool is_success                      { get; set; } = true;
    }

    public class CooperativeMemberValidatedResponse
    {
        public CooperativeMemberValidatedResponse(PublicCallAnswerMember answerMember)
        {
            this.dap = answerMember.member.dap_caf_code;
            this.cpf = answerMember.member.cpf ?? "Não encontrado";
            this.nome = answerMember.member.name;
            this.validade = answerMember.member.dap_caf_expiration_date?.ToString("dd/MM/yyyy");
            this.validado = answerMember.validated ? "Sim" : "Não";
        }

        public string? cpf      { get; set; }
        public string dap       { get; set; } = String.Empty;
        public string? nome     { get; set; }
        public string? validade { get; set; }
        public string validado  { get; set; }
    }

    public class CooperativeAnswerMemberResponse
    {
        public CooperativeAnswerMemberResponse(PublicCallAnswerMember answerMember)
        {
            this.dap_caf_code = answerMember.member.dap_caf_code;
            this.cpf = answerMember.member.cpf ?? "Não encontrado";
            this.name = answerMember.member.name;
            this.price = answerMember.price;
            this.quantity = answerMember.quantity;
        }

        public string? cpf { get; set; }
        public string dap_caf_code { get; set; } = String.Empty;
        public string? name { get; set; }
        public decimal price { get; set; }
        public decimal quantity { get; set; }
    }
}
