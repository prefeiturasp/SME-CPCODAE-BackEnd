using FIA.SME.Aquisicao.Api.Validations;
using FIA.SME.Aquisicao.Core.Domain;
using FIA.SME.Aquisicao.Infrastructure.Models;
using FluentValidation.Results;

namespace FIA.SME.Aquisicao.Api.Models
{
    public class DocumentTypeResponse
    {
        public DocumentTypeResponse(DocumentType? document)
        {
            if (document == null)
                return;

            this.id = document.id;
            this.name = document.name;
            this.application = document.application;
            this.is_active = document.is_active;
        }

        public Guid id          { get; set; }
        public string name      { get; set; }
        public int application  { get; set; }
        public bool is_active   { get; set; }
    }

    public class DocumentTypeRegister : BaseDomain
    {
        public string name      { get; set; }
        public int application  { get; set; }

        public override bool EhValido()
        {
            ValidationResult = new DocumentTypeRegisterValidation().Validate(this);
            return ValidationResult.IsValid;
        }
    }

    public class DocumentTypeUpdate : BaseDomain
    {
        public Guid id          { get; set; }
        public string name      { get; set; }
        public int application  { get; set; }
        public bool is_active   { get; set; }

        public override bool EhValido()
        {
            ValidationResult = new DocumentTypeUpdateValidation().Validate(this);
            return ValidationResult.IsValid;
        }
    }
}
