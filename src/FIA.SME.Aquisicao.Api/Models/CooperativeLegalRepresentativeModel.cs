using FIA.SME.Aquisicao.Api.Validations;
using FIA.SME.Aquisicao.Core.Domain;
using FIA.SME.Aquisicao.Core.Enums;
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
            this.position_expiration_date = legalRepresentative.position_expiration_date;
            this.position = legalRepresentative.position;
            this.marital_status = legalRepresentative.marital_status;
            this.address = new AddressResponse(legalRepresentative.address);
        }

        public Guid id                  { get; set; }
        public string cpf               { get; set; } = String.Empty;
        public string name              { get; set; } = String.Empty;
        public string phone             { get; set; } = String.Empty;
        public string position { get; set; } = String.Empty;
        public DateTime? position_expiration_date { get; set; }
        public MaritalStatusEnum marital_status { get; set; }
        public AddressResponse address  { get; set; }
    }

    public class CooperativeLegalRepresentativeUpdate : BaseDomain
    {
        public Guid id                  { get; set; }
        public string cpf               { get; set; } = String.Empty;
        public string email             { get; set; } = String.Empty;
        public string name              { get; set; } = String.Empty;
        public string phone             { get; set; } = String.Empty;
        public string position { get; set; } = String.Empty;
        public DateTime? position_expiration_date { get; set; }
        public MaritalStatusEnum marital_status { get; set; }

        public AddressUpdate address    { get; set; }

        public override bool EhValido()
        {
            ValidationResult = new CooperativeLegalRepresentativeUpdateValidation().Validate(this);
            return ValidationResult.IsValid;
        }
    }
}
