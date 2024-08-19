using FIA.SME.Aquisicao.Api.Models;
using FluentValidation;
using FluentValidationBR.Extensions;

namespace FIA.SME.Aquisicao.Api.Validations
{
    public class CooperativeLegalRepresentativeUpdateValidation : AbstractValidator<CooperativeLegalRepresentativeUpdate>
    {
        public CooperativeLegalRepresentativeUpdateValidation()
        {
            RuleFor(x => x.name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("O Nome do Representante Legal é obrigatório")
                .Length(1, 200).WithMessage("O Nome do Representante Legal deve ter até 200 caracteres");

            RuleFor(x => x.cpf)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("O CPF do Representante Legal é obrigatório")
                .Cpf().WithMessage("O CPF do Representante Legal está inválido");

            RuleFor(x => x.phone)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("O Telefone do Representante Legal é obrigatório")
                .Phone().WithMessage("O Telefone do Representante Legal está inválido");

            When(x => x.address != null, () =>
            {
                RuleFor(x => x.address!).SetValidator(new AddressUpdateValidation("Endereço: "));
            });
        }
    }
}
