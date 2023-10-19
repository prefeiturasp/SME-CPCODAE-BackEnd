using FIA.SME.Aquisicao.Api.Validations;
using FIA.SME.Aquisicao.Core.Domain;
using FIA.SME.Aquisicao.Infrastructure.Models;
using FluentValidation.Results;

namespace FIA.SME.Aquisicao.Api.Models
{
    public class CooperativeLegalRepresentativeResponse
    {
        public CooperativeLegalRepresentativeResponse(CooperativeLegalRepresentative? legalRepresentative)
        {
            if (legalRepresentative == null)
                return;

            this.id = legalRepresentative.id;
            this.cpf = legalRepresentative.cpf;
            this.name = legalRepresentative.name;
            this.phone = legalRepresentative.phone;
            this.address = new AddressResponse(legalRepresentative.address);
        }

        public Guid id                  { get; set; }
        public string cpf               { get; set; } = String.Empty;
        public string name              { get; set; } = String.Empty;
        public string phone             { get; set; } = String.Empty;
        public AddressResponse address  { get; set; }
    }

    public class CooperativeLegalRepresentativeUpdate : BaseDomain
    {
        public Guid id                  { get; set; }
        public string cpf               { get; set; } = String.Empty;
        public string email             { get; set; } = String.Empty;
        public string name              { get; set; } = String.Empty;
        public string phone             { get; set; } = String.Empty;
        public AddressUpdate address    { get; set; }

        public override bool EhValido()
        {
            ValidationResult = new CooperativeLegalRepresentativeUpdateValidation().Validate(this);
            return ValidationResult.IsValid;
        }
    }
}
