using FIA.SME.Aquisicao.Api.Models;
using FluentValidation;

namespace FIA.SME.Aquisicao.Api.Validations
{
    public class FaleConoscoValidation : AbstractValidator<FaleConoscoModel>
    {
        public FaleConoscoValidation()
        {
            RuleFor(x => x.title)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("O Título da solicitação é obrigatória")
                .Length(1, 100).WithMessage("O Título da solicitação deve ter até 100 caracteres");

            RuleFor(x => x.message)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("A Mensagem da solicitação é obrigatória")
                .Length(1, 500).WithMessage("A Mensagem da solicitação deve ter até 500 caracteres");
        }
    }
}
