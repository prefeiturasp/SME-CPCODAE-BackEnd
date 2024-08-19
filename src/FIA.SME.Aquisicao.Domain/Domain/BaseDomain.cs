using FluentValidation.Results;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FIA.SME.Aquisicao.Core.Domain
{
    public abstract class BaseDomain
    {
        public abstract bool EhValido();

        [JsonIgnore]
        [NotMapped]
        public ValidationResult ValidationResult { get; set; } = new ValidationResult();

        [JsonIgnore]
        [NotMapped]
        public List<string> Erros
        {
            get
            {
                return this.ValidationResult.Errors.ConvertAll(e => e.ErrorMessage);
            }
        }

        [JsonIgnore]
        [NotMapped]
        public bool IsValid { get { return this.Erros.Count == 0; } }
    }
}
