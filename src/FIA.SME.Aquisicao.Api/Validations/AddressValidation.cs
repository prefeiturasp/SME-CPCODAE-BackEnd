using FIA.SME.Aquisicao.Api.Models;
using FluentValidation;
using FluentValidationBR.Extensions;

namespace FIA.SME.Aquisicao.Api.Validations
{
    public class AddressUpdateValidation : AbstractValidator<AddressUpdate>
    {
        public AddressUpdateValidation(string prefixo = "")
        {
            RuleFor(x => x.street)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage($"{prefixo}O Logradouro é obrigatório")
                .Length(1, 200).WithMessage($"{prefixo}O Logradouro deve ter até 200 caracteres");

            RuleFor(x => x.cep)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage($"{prefixo}O CEP é obrigatório")
                .Cep().WithMessage($"{prefixo}O CEP está inválido");

            RuleFor(x => x.complement)
                .Length(0, 200).WithMessage($"{prefixo}O Complemento deve ter até 200 caracteres");

            RuleFor(x => x.district)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage($"{prefixo}O Bairro é obrigatório")
                .Length(1, 200).WithMessage($"{prefixo}O Bairro deve ter até 200 caracteres");

            RuleFor(x => x.number)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage($"{prefixo}O Número do Logradouro é obrigatório")
                .Length(1, 10).WithMessage($"{prefixo}O Número do Logradouro deve ter até 10 caracteres");
        }
    }
}
