using FIA.SME.Aquisicao.Api.Validations;
using FIA.SME.Aquisicao.Core.Domain;
using FIA.SME.Aquisicao.Infrastructure.Models;
using FluentValidation.Results;

namespace FIA.SME.Aquisicao.Api.Models
{
    public class BankResponse
    {
        public BankResponse(Bank? bank)
        {
            if (bank == null)
                return;

            this.id = bank.id;
            this.code = bank.code;
            this.name = bank.name;
            this.agency = bank.agency;
            this.account_number = bank.account_number;
        }

        public Guid id                  { get; set; }
        public string code              { get; set; } = String.Empty;
        public string name              { get; set; } = String.Empty;
        public string agency            { get; set; } = String.Empty;
        public string account_number    { get; set; } = String.Empty;
    }

    public class BankUpdate : BaseDomain
    {
        public Guid id                  { get; set; }
        public string code              { get; set; } = String.Empty;
        public string name              { get; set; } = String.Empty;
        public string agency            { get; set; } = String.Empty;
        public string account_number    { get; set; } = String.Empty;

        public override bool EhValido()
        {
            ValidationResult = new BankUpdateValidation().Validate(this);
            return ValidationResult.IsValid;
        }
    }
}
