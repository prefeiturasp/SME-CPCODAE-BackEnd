using FIA.SME.Aquisicao.Core.Enums;
using FIA.SME.Aquisicao.Infrastructure.Repositories.Types;

namespace FIA.SME.Aquisicao.Infrastructure.Models
{
    public class CooperativeLegalRepresentative
    {
        #region [ Construtores ]

        internal CooperativeLegalRepresentative() { }

        public CooperativeLegalRepresentative(Guid id, string cpf, string name, string phone, MaritalStatusEnum marital_status, string position, DateTime? position_expiration_date)
        {
            this.id = id;
            this.cpf = cpf;
            this.name = name;
            this.phone = phone;
            this.marital_status = marital_status;
            this.position = position;
            this.position_expiration_date = position_expiration_date;
        }

        internal CooperativeLegalRepresentative(RepresentanteLegal representanteLegal)
        {
            if (representanteLegal == null)
                return;

            this.id = representanteLegal.id;
            this.cpf = representanteLegal.cpf;
            this.name = representanteLegal.nome;
            this.phone = representanteLegal.telefone;
            this.position = representanteLegal.cargo;
            this.position_expiration_date = representanteLegal.data_vigencia_mandato;
            this.marital_status = (MaritalStatusEnum)representanteLegal.estado_civil;
        }

        #endregion [ FIM - Construtores ]

        #region [ Propriedades ]

        public Guid id          { get; private set; }
        public Guid address_id  { get; private set; }
        public string cpf       { get; private set; }
        public string name      { get; private set; }
        public string phone     { get; private set; }
        public string position { get; set; } = String.Empty;
        public DateTime? position_expiration_date { get; set; }
        public MaritalStatusEnum marital_status { get; set; }

        public Address address  { get; private set; }

        #endregion [ FIM - Propriedades ]

        #region [ Metodos ]

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
