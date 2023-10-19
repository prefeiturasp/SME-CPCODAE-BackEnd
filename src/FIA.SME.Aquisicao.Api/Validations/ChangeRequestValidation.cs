using FIA.SME.Aquisicao.Api.Models;
using FIA.SME.Aquisicao.Core.Validations;
using FluentValidation;

namespace FIA.SME.Aquisicao.Api.Validations
{
    public class ChangeRequestRegisterValidation : AbstractValidator<ChangeRequestRegister>
    {
        public ChangeRequestRegisterValidation()
        {
            RuleFor(x => x.title)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("O Título da solicitação é obrigatória")
                .Length(1, 100).WithMessage("O Título da solicitação deve ter até 100 caracteres");

            RuleFor(x => x.message)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("A Mensagem da solicitação é obrigatória")
                .Length(1, 500).WithMessage("A Mensagem da solicitação deve ter até 500 caracteres");

            RuleFor(u => u.response_date)
                .Cascade(CascadeMode.Stop)
                .IsValidDateTime(false).WithMessage("O Prazo para Resposta está inválido")
                .GreaterThan(DateTime.UtcNow.Date).WithMessage("O Prazo para Resposta deve ser posterior a data de hoje");
        }
    }
}
