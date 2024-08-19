using FIA.SME.Aquisicao.Api.Models;
using FIA.SME.Aquisicao.Core.Validations;
using FluentValidation;

namespace FIA.SME.Aquisicao.Api.Validations
{
    public class PublicCallBaseValidation : AbstractValidator<IPublicCallRequestModel>
    {
        public PublicCallBaseValidation()
        {
            RuleFor(x => x.name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("O Nome da Chamada Pública é obrigatório")
                .Length(1, 200).WithMessage("O Nome da Chamada Pública deve ter até 200 caracteres");

            RuleFor(x => x.number)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("O Número da Chamada Pública é obrigatório")
                .Length(1, 200).WithMessage("O Número da Chamada Pública deve ter até 200 caracteres");

            RuleFor(x => x.process)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("O Número de Processo da Chamada Pública é obrigatório")
                .Length(1, 200).WithMessage("O Número de Processo da Chamada Pública deve ter até 200 caracteres");

            RuleFor(x => x.public_session_url)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("A URL da Sessão Pública é obrigatória")
                .Length(1, 200).WithMessage("A URL da Sessão Pública deve ter até 200 caracteres");

            RuleFor(x => x.public_session_place)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("O Local da Sessão Pública é obrigatório")
                .Length(1, 200).WithMessage("O Local da Sessão Pública deve ter até 200 caracteres");

            RuleFor(x => x.notice_url)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("A URL do Edital é obrigatória")
                .Length(1, 200).WithMessage("A URL do Edital deve ter até 200 caracteres");

            RuleFor(x => x.notice_object)
                .NotEmpty().WithMessage("O Objeto da Chamada Pública é obrigatório");

            RuleFor(x => x.delivery_information)
                .NotEmpty().WithMessage("A Informação de Estimativa de Entrega é obrigatória");

            RuleFor(x => x.registration_start_date)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("A Data de Início das Inscrições para a Chamada Pública é obrigatória")
                .IsValidDateTime(true).WithMessage("A Data de Início das Inscrições para a Chamada Pública está inválida");

            RuleFor(x => x.registration_end_date)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("A Data de Término das Inscrições para a Chamada Pública é obrigatória")
                .IsValidDateTime(true).WithMessage("A Data de Término das Inscrições para a Chamada Pública está inválida");

            RuleFor(x => x.public_session_date)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("A Data da Sessão Pública é obrigatória")
                .IsValidDateTime(true).WithMessage("A Data da Sessão Pública está inválida");
        }
    }

    public class PublicCallRegistrationValidation : AbstractValidator<PublicCallRegistrationRequest>
    {
        public PublicCallRegistrationValidation()
        {
            RuleFor(x => x).SetValidator(new PublicCallBaseValidation());

            RuleFor(x => x.foods)
                .NotEmpty().WithMessage("O Produto da Chamada Pública é obrigatório");

            RuleForEach(x => x.foods).SetValidator(new PubliCallFoodRegistrationValidation());
        }
    }

    public class PublicCallUpdateValidation : AbstractValidator<PublicCallUpdateRequest>
    {
        public PublicCallUpdateValidation()
        {
            RuleFor(x => x).SetValidator(new PublicCallBaseValidation());

            RuleFor(x => x.id)
                .NotEmpty().WithMessage("O Id da Chamada Pública é obrigatório");

            RuleFor(x => x.foods)
                .NotEmpty().WithMessage("O Produto da Chamada Pública é obrigatório");

            RuleForEach(x => x.foods).SetValidator(new PubliCallFoodUpdateValidation());
        }
    }
}
