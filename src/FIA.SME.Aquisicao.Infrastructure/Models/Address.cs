using FIA.SME.Aquisicao.Infrastructure.Repositories.Types;

namespace FIA.SME.Aquisicao.Infrastructure.Models
{
    public class Address
    {
        public Address() { }

        public Address(Guid id, string street, int city_id, string cep, string? complement, string district, string number)
        {
            this.id = id;
            this.street = street;
            this.city_id = city_id;
            this.cep = cep;
            this.complement = complement;
            this.district = district;
            this.number = number;
        }

        internal Address(Endereco endereco)
        {
            if (endereco == null)
                return;

            this.id = endereco.id;
            this.street = endereco.logradouro;
            this.city_id = endereco.codigo_cidade_ibge;
            this.cep = endereco.cep;
            this.complement = endereco.complemento;
            this.district = endereco.bairro;
            this.number = endereco.numero;
        }

        public Guid id              { get; private set; }
        public string street        { get; private set; }
        public int city_id          { get; private set; }
        public string cep           { get; private set; }
        public string? complement   { get; private set; }
        public string district      { get; private set; }
        public string number        { get; private set; }

        public string ToString(IBGEDistrict.IBGEDistrictCity city)
        {
            //var comp = String.IsNullOrEmpty(complement) ? String.Empty : $" - {complement}";

            return $"{street}, {number} - {city.nome}/{city.microrregiao.mesorregiao.UF.sigla.ToUpper()}";
        }
    }
}
