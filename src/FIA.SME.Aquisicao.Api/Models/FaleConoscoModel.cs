using FIA.SME.Aquisicao.Api.Validations;
using FIA.SME.Aquisicao.Core.Domain;

namespace FIA.SME.Aquisicao.Api.Models
{
    public class FaleConoscoModel : BaseDomain
    {
        public Guid cooperative_id      { get; set; }
        public Guid user_id             { get; set; }
        public Guid public_call_id      { get; set; }
        public string message           { get; set; } = null!;
        public string title             { get; set; } = null!;

        public override bool EhValido()
        {
            ValidationResult = new FaleConoscoValidation().Validate(this);
            return ValidationResult.IsValid;
        }
    }
    
    public class FaleConoscoGetAllRequestModel
    {
        public Guid? cooperative_id { get; set; }
        public Guid? public_call_id { get; set; }
        public DateTime? start_date { get; set; }
        public DateTime? end_date { get; set; }
    }

    public class FaleConoscoResponseModel
    {
        public FaleConoscoResponseModel(Guid id, string cooperative_name, string public_call_name, string message, string title, DateTime creation_date)
        {
            this.id = id;
            this.cooperative_name = cooperative_name;
            this.public_call_name = public_call_name;
            this.message = message;
            this.title = title;
            this.creation_date = creation_date;
        }

        public Guid id { get; set; }
        public string cooperative_name { get; set; } = String.Empty;
        public string public_call_name { get; set; } = String.Empty;
        public string message { get; set; } = null!;
        public string title { get; set; } = null!;
        public DateTime creation_date { get; set; }
    }

    public class FaleConoscoListResponseModel
    {
        public FaleConoscoListResponseModel(Guid id, string cooperative_name, string public_call_name, string title, DateTime creation_date)
        {
            this.id = id;
            this.cooperative_name = cooperative_name;
            this.public_call_name = public_call_name;
            this.title = title;
            this.creation_date = creation_date;
        }

        public Guid id { get; set; }
        public string cooperative_name { get; set; } = String.Empty;
        public string public_call_name { get; set; } = String.Empty;
        public string title { get; set; } = null!;
        public DateTime creation_date { get; set; }
    }
}
