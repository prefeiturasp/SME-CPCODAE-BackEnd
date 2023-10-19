using FIA.SME.Aquisicao.Api.Models;
using FluentValidation;

namespace FIA.SME.Aquisicao.Api.Validations
{
    public class BankUpdateValidation : AbstractValidator<BankUpdate>
    {
        public BankUpdateValidation(string prefixo = "")
        {
            RuleFor(x => x.code)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage($"{prefixo}O Código do Banco é obrigatório")
                .Length(1, 10).WithMessage($"{prefixo}O Código do Banco deve ter até 10 caracteres");

            RuleFor(x => x.name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage($"{prefixo}O Nome do Banco é obrigatório")
                .Length(1, 200).WithMessage($"{prefixo}O Nome do Banco deve ter até 200 caracteres");

            RuleFor(x => x.agency)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage($"{prefixo}O Número da Agência é obrigatório")
                .Length(1, 20).WithMessage($"{prefixo}O Número da Agência deve ter até 20 caracteres");

            RuleFor(x => x.account_number)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage($"{prefixo}O Número da Conta é obrigatório")
                .Length(1, 20).WithMessage($"{prefixo}O Número da Conta deve ter até 20 caracteres");
        }
    }
}
