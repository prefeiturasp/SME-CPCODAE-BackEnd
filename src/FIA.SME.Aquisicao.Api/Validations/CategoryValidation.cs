using FIA.SME.Aquisicao.Api.Models;
using FluentValidation;

namespace FIA.SME.Aquisicao.Api.Validations
{
    public class CategoryRegisterValidation : AbstractValidator<CategoryRegister>
    {
        public CategoryRegisterValidation()
        {
            RuleFor(x => x.name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("O Nome da Categoria é obrigatório")
                .Length(1, 200).WithMessage("O Nome da Categoria deve ter até 200 caracteres");
        }
    }

    public class CategoryUpdateValidation : AbstractValidator<CategoryUpdate>
    {
        public CategoryUpdateValidation()
        {
            RuleFor(x => x.name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("O Nome da Categoria é obrigatório")
                .Length(1, 200).WithMessage("O Nome da Categoria deve ter até 200 caracteres");
        }
    }
}
