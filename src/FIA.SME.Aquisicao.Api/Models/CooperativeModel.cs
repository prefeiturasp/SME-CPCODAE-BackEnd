using FIA.SME.Aquisicao.Core.Enums;
using FIA.SME.Aquisicao.Infrastructure.Models;

namespace FIA.SME.Aquisicao.Api.Models
{
    public class CooperativeDetailResponse
    {
        public CooperativeDetailResponse(Cooperative? cooperative, bool withChildren)
        {
            if (cooperative == null)
                return;

            this.id = cooperative.id;
            this.user_id = cooperative.user_id;
            this.acronym = cooperative.acronym;
            this.address = new AddressResponse(cooperative.address);
            //this.bank = new BankResponse(cooperative.bank);
            this.cnpj = cooperative.cnpj;
            this.cnpj_central = cooperative.cnpj_central;
            this.dap_caf_code = cooperative.dap_caf_code;
            this.email = cooperative.email;
            this.name = cooperative.name;
            this.phone = cooperative.phone;
            this.legal_representative = new CooperativeLegalRepresentativeResponse(cooperative.legal_representative);
            this.pj_type = (int)cooperative.pj_type;
            this.production_type = (int)cooperative.production_type;
            this.status = (int)cooperative.status;
            this.dap_caf_registration_date = cooperative.dap_caf_registration_date?.ToLocalTime();
            this.dap_caf_expiration_date = cooperative.dap_caf_expiration_date?.ToLocalTime();

            this.is_dap = cooperative.is_dap;
            this.is_active = cooperative.is_active;

            if (withChildren && cooperative.members != null && cooperative.members.Any())
            {
                this.members = cooperative.members.Select(m => new CooperativeMemberResponse(m)).ToList();
            }

            if (withChildren && cooperative.documents != null && cooperative.documents.Any())
            {
                this.documents = cooperative.documents.Select(m => new CooperativeDocumentResponse(m)).ToList();
            }
        }

        public Guid id                                          { get; set; }
        public Guid user_id                                     { get; set; }
        public string? acronym                                  { get; set; } = String.Empty;
        public AddressResponse address                          { get; set; }
        public BankResponse bank                                { get; set; }
        public string cnpj                                      { get; set; } = String.Empty;
        public string? cnpj_central                             { get; set; } = String.Empty;
        public string dap_caf_code                              { get; set; } = String.Empty;
        public string? email                                    { get; set; } = String.Empty;
        public string? name                                     { get; set; } = String.Empty;
        public string? phone                                    { get; set; } = String.Empty;
        public CooperativeLegalRepresentativeResponse legal_representative  { get; set; }
        public int pj_type                                      { get; set; }
        public int production_type                              { get; set; }
        public int status                                       { get; set; }
        public DateTime? dap_caf_registration_date              { get; set; }
        public DateTime? dap_caf_expiration_date                { get; set; }
        
        public bool is_dap                                      { get; set; }
        public bool is_active                                   { get; set; }

        public List<CooperativeDocumentResponse> documents      { get; set; }
        public List<CooperativeMemberResponse> members          { get; set; }
    }

    public class CooperativeRegister
    {
        public string cnpj                      { get; set; } = String.Empty;
        public string dap_caf_code              { get; set; } = String.Empty;
        public string name                      { get; set; } = String.Empty;
        public string email                     { get; set; } = String.Empty;
        public string password                  { get; set; } = String.Empty;
        public string terms_use_acceptance_ip   { get; set; } = String.Empty;
        public bool is_dap                      { get; set; } = true;

        public Guid id                                          { get; set; }
        public string acronym                                   { get; set; } = String.Empty;
        public AddressUpdate address                            { get; set; } = new AddressUpdate();
        public string? cnpj_central                             { get; set; } = String.Empty;
        public string phone                                     { get; set; } = String.Empty;
        public CooperativeLegalRepresentativeUpdate legal_representative { get; set; } = new CooperativeLegalRepresentativeUpdate();
        public CooperativePJTypeEnum pj_type                    { get; set; }
        public CooperativeProductionTypeEnum production_type    { get; set; }
        public DateTime dap_caf_registration_date               { get; set; }
        public DateTime dap_caf_expiration_date                 { get; set; }        
    }

    public class CooperativeStep1
    {
        public Guid id                                          { get; set; }
        public string acronym                                   { get; set; } = String.Empty;
        public AddressUpdate address                            { get; set; } = new AddressUpdate();
        //public BankUpdate bank                                  { get; set; } = new BankUpdate();
        public string cnpj                                      { get; set; } = String.Empty;
        public string? cnpj_central                             { get; set; } = String.Empty;
        public string dap_caf_code                              { get; set; } = String.Empty;
        public string email                                     { get; set; } = String.Empty;
        public string name                                      { get; set; } = String.Empty;
        public string phone                                     { get; set; } = String.Empty;
        public CooperativeLegalRepresentativeUpdate legal_representative { get; set; } = new CooperativeLegalRepresentativeUpdate();
        public CooperativePJTypeEnum pj_type                    { get; set; }
        public CooperativeProductionTypeEnum production_type    { get; set; }
        public DateTime dap_caf_registration_date               { get; set; }
        public DateTime dap_caf_expiration_date                 { get; set; }
        
        public bool is_dap                                      { get; set; }
    }

    public class CooperativeUpdate
    {
        public Guid id                                          { get; set; }
        public string acronym                                   { get; set; } = String.Empty;
        public AddressUpdate address                            { get; set; } = new AddressUpdate();
        public BankUpdate bank                                  { get; set; } = new BankUpdate();
        public string cnpj                                      { get; set; } = String.Empty;
        public string? cnpj_central                             { get; set; } = String.Empty;
        public string dap_caf_code                              { get; set; } = String.Empty;
        public string email                                     { get; set; } = String.Empty;
        public string name                                      { get; set; } = String.Empty;
        public string phone                                     { get; set; } = String.Empty;
        public CooperativeLegalRepresentativeUpdate legal_representative { get; set; } = new CooperativeLegalRepresentativeUpdate();
        public CooperativePJTypeEnum pj_type                    { get; set; }
        public CooperativeProductionTypeEnum production_type    { get; set; }
        public DateTime dap_caf_registration_date               { get; set; }
        public DateTime dap_caf_expiration_date                 { get; set; }
        
        public bool is_dap                                      { get; set; }
        public bool is_active                                   { get; set; }
    }
}
