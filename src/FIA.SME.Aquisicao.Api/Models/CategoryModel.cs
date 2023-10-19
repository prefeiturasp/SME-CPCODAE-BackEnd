using FIA.SME.Aquisicao.Api.Validations;
using FIA.SME.Aquisicao.Core.Domain;
using FIA.SME.Aquisicao.Infrastructure.Models;
using FluentValidation.Results;

namespace FIA.SME.Aquisicao.Api.Models
{
    public class CategoryResponse
    {
        public CategoryResponse(Category? category)
        {
            if (category == null)
                return;

            this.id = category.id;
            this.name = category.name;
            this.is_active = category.is_active;
        }

        public Guid id          { get; set; }
        public string name      { get; set; }
        public bool is_active   { get; set; }
    }

    public class CategoryRegister : BaseDomain
    {
        public string name      { get; set; }

        public override bool EhValido()
        {
            ValidationResult = new CategoryRegisterValidation().Validate(this);
            return ValidationResult.IsValid;
        }
    }

    public class CategoryUpdate : BaseDomain
    {
        public Guid id          { get; set; }
        public string name      { get; set; }
        public bool is_active   { get; set; }

        public override bool EhValido()
        {
            ValidationResult = new CategoryUpdateValidation().Validate(this);
            return ValidationResult.IsValid;
        }
    }
}
