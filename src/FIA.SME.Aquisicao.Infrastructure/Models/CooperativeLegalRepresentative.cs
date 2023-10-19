using FIA.SME.Aquisicao.Infrastructure.Repositories.Types;

namespace FIA.SME.Aquisicao.Infrastructure.Models
{
    public class CooperativeLegalRepresentative
    {
        #region [ Construtores ]

        internal CooperativeLegalRepresentative() { }

        public CooperativeLegalRepresentative(Guid id, string cpf, string name, string phone)
        {
            this.id = id;
            this.cpf = cpf;
            this.name = name;
            this.phone = phone;
        }

        internal CooperativeLegalRepresentative(RepresentanteLegal representanteLegal)
        {
            if (representanteLegal == null)
                return;

            this.id = representanteLegal.id;
            this.cpf = representanteLegal.cpf;
            this.name = representanteLegal.nome;
            this.phone = representanteLegal.telefone;
        }

        #endregion [ FIM - Construtores ]

        #region [ Propriedades ]

        public Guid id          { get; private set; }
        public Guid address_id  { get; private set; }
        public string cpf       { get; private set; }
        public string name      { get; private set; }
        public string phone     { get; private set; }

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
