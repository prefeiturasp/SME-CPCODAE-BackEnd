using FIA.SME.Aquisicao.Api.Models;
using FIA.SME.Aquisicao.Core.Enums;
using FluentValidation;

namespace FIA.SME.Aquisicao.Api.Validations
{
    public class DocumentTypeRegisterValidation : AbstractValidator<DocumentTypeRegister>
    {
        public DocumentTypeRegisterValidation()
        {
            RuleFor(x => x.name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("O Nome do Documento é obrigatório")
                .Length(1, 200).WithMessage("O Nome do Documento deve ter até 200 caracteres");

            RuleFor(x => (DocumentTypeEnum)x.application)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage(x => $"A Aplicação do Documento <{x.application}> está inválida")
                    .IsInEnum().WithMessage(x => $"A Aplicação do Documento <{x.application}> está inválida");
        }
    }

    public class DocumentTypeUpdateValidation : AbstractValidator<DocumentTypeUpdate>
    {
        public DocumentTypeUpdateValidation()
        {
            RuleFor(x => x.name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("O Nome do Documento é obrigatório")
                .Length(1, 200).WithMessage("O Nome do Documento deve ter até 200 caracteres");

            RuleFor(x => (DocumentTypeEnum)x.application)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage(x => $"A Aplicação do Documento <{x.application}> está inválido")
                    .IsInEnum().WithMessage(x => $"A Aplicação do Documento <{x.application}> está inválido");
        }
    }
}
