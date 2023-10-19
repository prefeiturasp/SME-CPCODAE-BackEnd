using FIA.SME.Aquisicao.Api.Models;
using FluentValidation;

namespace FIA.SME.Aquisicao.Api.Validations
{
    public class PublicCallAnswerChooseValidation : AbstractValidator<PublicCallAnswerChooseRequest>
    {
        public PublicCallAnswerChooseValidation()
        {
            RuleFor(x => x.selectedCooperatives)
                .NotEmpty().WithMessage("Ao menos uma cooperativa deve ser selecionada");

            RuleForEach(x => x.selectedCooperatives).SetValidator(new PublicCallAnswerSelectedCooperativeValidation());
        }
    }

    public class PublicCallAnswerSelectedCooperativeValidation : AbstractValidator<PublicCallAnswerSelectedCooperative>
    {
        public PublicCallAnswerSelectedCooperativeValidation()
        {
            When(x => x.new_quantity != null, () =>
            {
                RuleFor(x => x.new_quantity!)
                    .GreaterThan(0).WithMessage("A nova quantidade escolhida deve ser maior do que zero");
            });
        }
    }
}
