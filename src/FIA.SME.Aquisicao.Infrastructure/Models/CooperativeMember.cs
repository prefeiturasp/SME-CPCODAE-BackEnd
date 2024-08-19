using FIA.SME.Aquisicao.Core.Enums;
using FIA.SME.Aquisicao.Core.Helpers;
using FIA.SME.Aquisicao.Infrastructure.Repositories.Types;

namespace FIA.SME.Aquisicao.Infrastructure.Models
{
    public class CooperativeMember
    {
        #region [ Construtores ]

        public CooperativeMember(Guid cooperative_id, string dap_caf_code, string cpf) : this(null, cooperative_id, dap_caf_code, cpf)
        { }

        public CooperativeMember(Guid? id, Guid cooperative_id, string dap_caf_code, string cpf)
        {
            this.id = id ?? Guid.NewGuid();
            this.cooperative_id = cooperative_id;
            this.dap_caf_code = dap_caf_code;
            this.cpf = cpf;
            this.is_dap = true;
            this.is_male = true;
            this.is_active = true;
        }

        public CooperativeMember(Guid? cooperative_id, bool is_dap, string dap_caf_code, string name, string cpf, int pf_type, int production_type, DateTime dap_caf_registration_date, 
            DateTime dap_caf_expiration_date, bool is_male, bool is_active)
        {
            this.cooperative_id = cooperative_id;
            this.dap_caf_code = dap_caf_code;
            this.cpf = cpf.ToOnlyNumbers();
            this.name = name.ToTitleCase();
            this.pf_type = (CooperativePFTypeEnum)pf_type;
            this.production_type = (CooperativeProductionTypeEnum)production_type;
            this.dap_caf_registration_date = dap_caf_registration_date;
            this.dap_caf_expiration_date = dap_caf_expiration_date;
            this.is_dap = is_dap;
            this.is_male = is_male;
            this.is_active = is_active;
        }

        internal CooperativeMember(Cooperado cooperado)
        {
            if (cooperado == null)
                return;

            this.id = cooperado.id;
            this.address_id = cooperado.endereco_id;
            this.cooperative_id = cooperado.cooperativa_id;
            this.dap_caf_code = cooperado.codigo_dap_caf;
            this.cpf = cooperado.cpf;
            this.name = cooperado.nome;
            this.pf_type = cooperado.tipo_agricultor_familiar.HasValue ? (CooperativePFTypeEnum)cooperado.tipo_agricultor_familiar : null;
            this.production_type = cooperado.tipo_producao.HasValue ?(CooperativeProductionTypeEnum)cooperado.tipo_producao : null;
            this.dap_caf_registration_date = cooperado.data_emissao_dap_caf;
            this.dap_caf_expiration_date = cooperado.data_validade_dap_caf;
            this.is_dap = cooperado.is_dap;
            this.is_male = cooperado.is_masculino;
            this.is_active = cooperado.ativo;

            if (cooperado.Endereco != null)
                this.address = new Address(cooperado.Endereco);
        }

        #endregion [ FIM - Construtores ]

        #region [ Propriedades ]

        public Guid id                                          { get; private set; }
        public Guid? address_id                                 { get; private set; }
        public Guid? cooperative_id                             { get; private set; }
        public Address? address                                 { get; private set; }
        public string? cpf                                      { get; private set; }
        public string dap_caf_code                              { get; private set; } = String.Empty;
        public string? name                                     { get; private set; }
        public CooperativePFTypeEnum? pf_type                   { get; private set; }
        public CooperativeProductionTypeEnum? production_type   { get; private set; }
        public DateTime? dap_caf_registration_date              { get; private set; }
        public DateTime? dap_caf_expiration_date                { get; private set; }
        public bool is_dap                                      { get; private set; }
        public bool is_male                                     { get; private set; }
        public bool is_active                                   { get; private set; }

        #endregion [ FIM - Propriedades ]

        #region [ Metodos ]

        public void SetData(string cpf, string name, DateTime? dap_caf_expiration_date)
        {
            this.cpf = cpf;
            this.name = name;

            if (dap_caf_expiration_date.HasValue)
            {
                this.dap_caf_registration_date = DateTime.Now.AddYears(-1);
                this.dap_caf_expiration_date = dap_caf_expiration_date;
            }
        }

        public void SetId(Guid id)
        {
            this.id = id;
        }

        public void SetIds(Guid id, Guid cooperative_id)
        {
            this.id = id;
            this.cooperative_id = cooperative_id;
        }

        public void UpdateAddress(Guid? id, string street, string number, string? complement, string district, string cep, int city_id)
        {
            if (id == null || id == Guid.Empty)
                id = Guid.NewGuid();

            this.address = new Address(id.Value, street, city_id, cep, complement, district, number);
            this.address_id = id.Value;
        }

        #endregion [ FIM - Metodos ]
    }
}
