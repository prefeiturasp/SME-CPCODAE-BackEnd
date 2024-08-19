using FIA.SME.Aquisicao.Api.Models;
using FIA.SME.Aquisicao.Core.Enums;
using FIA.SME.Aquisicao.Core.Helpers;
using FluentValidation;

namespace FIA.SME.Aquisicao.Api.Validations
{
    public class FoodRegisterValidation : AbstractValidator<FoodRegister>
    {
        public FoodRegisterValidation()
        {
            RuleFor(x => x.name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("O Nome da Categoria é obrigatório")
                .Length(1, 200).WithMessage("O Nome da Categoria deve ter até 200 caracteres");

            RuleFor(x => x.category_id)
                    .NotEmpty().WithMessage(x => $"A Categoria é obrigatória");
        }
    }

    public class FoodUpdateValidation : AbstractValidator<FoodUpdate>
    {
        public FoodUpdateValidation()
        {
            RuleFor(x => x.name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("O Nome da Categoria é obrigatório")
                .Length(1, 200).WithMessage("O Nome da Categoria deve ter até 200 caracteres");

            RuleFor(x => x.category_id)
                    .NotEmpty().WithMessage(x => $"A Categoria é obrigatória");

            RuleFor(x => (MeasureUnitEnum)x.measure_unit)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage(x => $"A Unidade de Medida <{((MeasureUnitEnum)x.measure_unit).DescriptionAttr()}> está inválida")
                    .IsInEnum().WithMessage(x => $"A Unidade de Medida <{((MeasureUnitEnum)x.measure_unit).DescriptionAttr()}> está inválida");
        }
    }
}
