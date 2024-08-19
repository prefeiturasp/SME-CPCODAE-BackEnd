using FIA.SME.Aquisicao.Api.Models;
using FIA.SME.Aquisicao.Core.Enums;
using FluentValidation;

namespace FIA.SME.Aquisicao.Api.Validations
{
    public class PubliCallFoodBaseValidation : AbstractValidator<IPublicCallRequest>
    {
        public PubliCallFoodBaseValidation()
        {
            RuleFor(x => x.food_id)
                .NotEmpty().WithMessage("O Produto é obrigatório");

            RuleFor(x => (MeasureUnitEnum)x.measure_unit_id)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage(x => $"A Unidade de Medida do Produto <{x.measure_unit_id}> está inválida")
                    .IsInEnum().WithMessage(x => $"A Unidade de Medida do Produto <{x.measure_unit_id}> está inválida");

            RuleFor(x => x.price)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("O Preço do Produto é obrigatório")
                .GreaterThan(0).WithMessage("O Preço do Produto deve ser maior do que zero");

            RuleFor(x => x.quantity)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("A Quantidade do Produto é obrigatória")
                .GreaterThan(0).WithMessage("A Quantidade do Produto deve ser maior do que zero");
        }
    }

    public class PubliCallFoodRegistrationValidation : AbstractValidator<PublicCallFoodRegistrationRequest>
    {
        public PubliCallFoodRegistrationValidation()
        {
            RuleFor(x => x).SetValidator(new PubliCallFoodBaseValidation());
        }
    }

    public class PubliCallFoodUpdateValidation : AbstractValidator<PublicCallFoodUpdateRequest>
    {
        public PubliCallFoodUpdateValidation()
        {
            RuleFor(x => x).SetValidator(new PubliCallFoodBaseValidation());

            RuleFor(x => x.id)
                .NotEmpty().WithMessage("O Id do Produto é obrigatório");

            RuleFor(x => x.public_call_id)
                .NotEmpty().WithMessage("A Chamada Pública do Produto é obrigatória");
        }
    }
}
