using FIA.SME.Aquisicao.Api.Validations;
using FIA.SME.Aquisicao.Core.Domain;
using FIA.SME.Aquisicao.Infrastructure.Models;
using FluentValidation.Results;

namespace FIA.SME.Aquisicao.Api.Models
{
    public class AddressResponse
    {
        public AddressResponse(Address? address)
        {
            if (address == null)
                return;

            this.id = address.id;
            this.street = address.street;
            this.city_id = address.city_id;
            this.cep = address.cep;
            this.complement = address.complement;
            this.district = address.district;
            this.number = address.number;
        }

        public Guid id              { get; set; }
        public string street        { get; set; } = String.Empty;
        public int city_id          { get; set; }
        public string cep           { get; set; } = String.Empty;
        public string? complement   { get; set; }
        public string district      { get; set; } = String.Empty;
        public string number        { get; set; } = String.Empty;
    }

    public class AddressUpdate : BaseDomain
    {
        public Guid id              { get; set; }
        public string street        { get; set; } = String.Empty;
        public int city_id          { get; set; }
        public string cep           { get; set; } = String.Empty;
        public string? complement   { get; set; }
        public string district      { get; set; } = String.Empty;
        public string number        { get; set; } = String.Empty;

        public override bool EhValido()
        {
            ValidationResult = new AddressUpdateValidation().Validate(this);
            return ValidationResult.IsValid;
        }
    }
}
