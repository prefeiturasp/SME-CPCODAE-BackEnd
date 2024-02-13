using FIA.SME.Aquisicao.Api.Validations;
using FIA.SME.Aquisicao.Core.Domain;

namespace FIA.SME.Aquisicao.Api.Models
{
    public class FaleConoscoModel : BaseDomain
    {
        public Guid cooperative_id      { get; set; }
        public Guid user_id             { get; set; }
        public string message           { get; set; } = null!;
        public string title             { get; set; } = null!;

        public override bool EhValido()
        {
            ValidationResult = new FaleConoscoValidation().Validate(this);
            return ValidationResult.IsValid;
        }
    }
}
