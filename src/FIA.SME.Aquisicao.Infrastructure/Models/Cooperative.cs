using FIA.SME.Aquisicao.Core.Enums;
using FIA.SME.Aquisicao.Core.Helpers;
using FIA.SME.Aquisicao.Infrastructure.Repositories.Types;

namespace FIA.SME.Aquisicao.Infrastructure.Models
{
    public class Cooperative
    {
        #region [ Construtores ]

        public Cooperative(string cnpj, string dap, bool is_dap, string terms_use_acceptance_ip)
        {
            this.cnpj = cnpj.ToOnlyNumbers();
            this.dap_caf_code = dap.ToUpper();

            this.status = CooperativeStatusEnum.AwaitingEmailConfirmation;
            this.is_dap = is_dap;
            this.is_active = true;

            this.terms_use_acceptance_ip = terms_use_acceptance_ip;
            this.terms_use_acceptance_date = DateTime.UtcNow;

            this._members = new List<CooperativeMember>();
        }

        public Cooperative(Guid id, Guid user_id, string name, string acronym, string email, string logo, string phone, string cnpj, string? cnpj_central, bool is_dap, string dap_caf_code, DateTime dap_caf_registration_date,
            DateTime dap_caf_expiration_date, CooperativePJTypeEnum pj_type, CooperativeProductionTypeEnum production_type, CooperativeStatusEnum status, string terms_use_acceptance_ip, 
            DateTime terms_use_acceptance_date, bool is_active, List<CooperativeDocument> documents, List<CooperativeMember> members)
        {
            this.id = id;
            this.user_id = user_id;
            this.acronym = acronym;
            this.cnpj = cnpj.ToOnlyNumbers();
            this.cnpj_central = String.IsNullOrEmpty(cnpj_central) ? null : cnpj_central.ToOnlyNumbers();
            this.dap_caf_code = dap_caf_code.ToUpper();
            this.email = email;
            this.logo = logo;
            this.name = name.ToTitleCase();
            this.phone = phone;
            this.dap_caf_registration_date = dap_caf_registration_date;
            this.dap_caf_expiration_date = dap_caf_expiration_date;
            this.terms_use_acceptance_ip = terms_use_acceptance_ip;
            this.terms_use_acceptance_date = terms_use_acceptance_date;
            this.pj_type = pj_type;
            this.production_type = production_type;
            this.is_dap = is_dap;
            this.is_active = is_active;

            this.status = status;
            this._documents = documents;
            this._members = members;
        }

        internal Cooperative(Cooperativa? cooperativa)
        {
            if (cooperativa == null)
                return;

            this.id = cooperativa.id;
            this.address_id = cooperativa.endereco_id;
            this.bank_id = cooperativa.banco_id;
            this.legal_representative_id = cooperativa.representante_legal_id;
            this.user_id = cooperativa.usuario_id;
            this.cnpj = cooperativa.cnpj;
            this.cnpj_central = cooperativa.cnpj_central;
            this.dap_caf_code = cooperativa.codigo_dap_caf;
            this.acronym = cooperativa.sigla;
            this.email = cooperativa.email;
            this.logo = cooperativa.logo;
            this.name = cooperativa.razao_social;
            this.phone = cooperativa.telefone;
            this.pj_type = (CooperativePJTypeEnum)cooperativa.tipo_pessoa_juridica;
            this.production_type = (CooperativeProductionTypeEnum)cooperativa.tipo_producao;
            this.status = (CooperativeStatusEnum)cooperativa.situacao;
            this.creation_date = cooperativa.data_criacao;
            this.dap_caf_registration_date = cooperativa.data_emissao_dap_caf;
            this.dap_caf_expiration_date = cooperativa.data_validade_dap_caf;
            this.terms_use_acceptance_date = cooperativa.data_aceite_termos_uso;
            this.terms_use_acceptance_ip = cooperativa.ip_aceite_termos_uso;
            this.is_dap = cooperativa.is_dap;
            this.is_active = cooperativa.ativa;

            this._documents = new List<CooperativeDocument>();
            this._members = new List<CooperativeMember>();

            if (cooperativa.Cooperados != null && cooperativa.Cooperados.Any())
                this._members = cooperativa.Cooperados.Select(c => new CooperativeMember(c)).ToList();

            if (cooperativa.Documentos != null && cooperativa.Documentos.Any())
                this._documents = cooperativa.Documentos.Select(d => new CooperativeDocument(d)).ToList();

            if (cooperativa.Banco != null)
                this.UpdateBank(cooperativa.Banco.id, cooperativa.Banco.codigo, cooperativa.Banco.nome, cooperativa.Banco.agencia, cooperativa.Banco.numero_conta);

            if (cooperativa.Endereco != null)
                this.UpdateAddress(cooperativa.Endereco.id, cooperativa.Endereco.logradouro, cooperativa.Endereco.numero, cooperativa.Endereco.complemento, cooperativa.Endereco.bairro, cooperativa.Endereco.cep, cooperativa.Endereco.codigo_cidade_ibge);

            if (cooperativa.RepresentanteLegal != null)
                this.UpdateLegalRepresentative(cooperativa.RepresentanteLegal.id, cooperativa.RepresentanteLegal.cpf, cooperativa.RepresentanteLegal.nome, cooperativa.RepresentanteLegal.telefone, (MaritalStatusEnum)cooperativa.RepresentanteLegal.estado_civil, 
                    cooperativa.RepresentanteLegal.cargo, cooperativa.RepresentanteLegal.data_vigencia_mandato, cooperativa.RepresentanteLegal.Endereco.id,
                    cooperativa.RepresentanteLegal.Endereco.logradouro, cooperativa.RepresentanteLegal.Endereco.numero, cooperativa.RepresentanteLegal.Endereco.complemento, cooperativa.RepresentanteLegal.Endereco.bairro,
                    cooperativa.RepresentanteLegal.Endereco.cep, cooperativa.RepresentanteLegal.Endereco.codigo_cidade_ibge);
        }

        #endregion [ FIM - Construtores ]

        #region [ Propriedades ]

        public Guid id                                              { get; private set; }
        public Guid? address_id                                     { get; private set; }
        public Guid? bank_id                                        { get; private set; }
        public Guid? legal_representative_id                        { get; private set; }
        public Guid user_id                                         { get; private set; }
        public string cnpj                                          { get; private set; } = String.Empty;
        public string? cnpj_central                                 { get; private set; }
        public string dap_caf_code                                  { get; private set; } = String.Empty;
        public string logo                                          { get; private set; } = String.Empty;
        public string? acronym                                      { get; private set; }
        public Address address                                      { get; private set; }
        public Bank bank                                            { get; private set; }
        public string? email                                        { get; private set; }
        public string? phone                                        { get; private set; }
        public string? name                                         { get; private set; }
        public CooperativeLegalRepresentative legal_representative  { get; private set; }
        public CooperativePJTypeEnum pj_type                        { get; private set; }
        public CooperativeProductionTypeEnum production_type        { get; private set; }
        public CooperativeStatusEnum status                         { get; private set; }
        public DateTime creation_date                               { get; private set; }
        public DateTime? dap_caf_registration_date                  { get; private set; }
        public DateTime? dap_caf_expiration_date                    { get; private set; }
        public string terms_use_acceptance_ip                       { get; private set; }
        public DateTime terms_use_acceptance_date                   { get; private set; }
        public bool is_dap                                          { get; private set; }
        public bool is_active                                       { get; private set; }

        private List<CooperativeDocument> _documents                { get; set; }
        public IReadOnlyCollection<CooperativeDocument> documents => this._documents;

        private List<CooperativeMember> _members                    { get; set; }
        public IReadOnlyCollection<CooperativeMember> members => this._members;

        public string formatted_phone                           { get { return String.IsNullOrEmpty(this.phone) ? String.Empty : this.phone.FormatPhoneNumber(); } }

        #endregion [ FIM - Propriedades ]

        #region [ Metodos ]

        public void Disable()
        {
            this.is_active = false;
        }

        public void SetId(Guid id)
        {
            this.id = id;
        }

        public void SetToAwaitingEmailConfirmation()
        {
            this.status = CooperativeStatusEnum.AwaitingEmailConfirmation;
        }

        public void SetToAwaitingToCompleteRegistration()
        {
            this.status = CooperativeStatusEnum.AwaitingToCompleteRegistration;
        }

        public void SetToCompletedRegistration()
        {
            this.status = CooperativeStatusEnum.Registered;
        }

        public void SetUserId(Guid id)
        {
            this.user_id = id;
        }

        public void UpdateAddress(Guid? id, string street, string number, string? complement, string district, string cep, int city_id)
        {
            if (id == null || id == Guid.Empty)
                id = Guid.NewGuid();

            this.address = new Address(id.Value, street, city_id, cep, complement, district, number);
            this.address_id = id.Value;
        }

        public void UpdateBank(Guid? id, string code, string name, string agency, string account_number)
        {
            if (id == null || id == Guid.Empty)
                id = Guid.NewGuid();

            this.bank = new Bank(id.Value, code, name, agency, account_number);
            this.bank_id = id.Value;
        }

        public void UpdateLegalRepresentative(Guid? id, string cpf, string name, string phone, MaritalStatusEnum marital_status, string position, DateTime? position_expiration_date, Guid? address_id, string street, string number, string? complement, string district, string cep, int city_id)
        {
            if (id == null || id == Guid.Empty)
                id = Guid.NewGuid();

            if (address_id == null || address_id == Guid.Empty)
                address_id = Guid.NewGuid();

            this.legal_representative = new CooperativeLegalRepresentative(id.Value, cpf, name, phone, marital_status, position, position_expiration_date);
            this.legal_representative.UpdateAddress(address_id.Value, street, number, complement, district, cep, city_id);
            this.legal_representative_id = id.Value;
        }

        #endregion [ FIM - Metodos ]
    }
}
