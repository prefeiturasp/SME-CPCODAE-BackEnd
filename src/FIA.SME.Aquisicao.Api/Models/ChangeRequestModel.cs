using FIA.SME.Aquisicao.Api.Validations;
using FIA.SME.Aquisicao.Core.Domain;
using FIA.SME.Aquisicao.Infrastructure.Models;

namespace FIA.SME.Aquisicao.Api.Models
{
    public class ChangeRequestResponse
    {
        public ChangeRequestResponse(ChangeRequest? changeRequest)
        {
            if (changeRequest == null)
                return;

            this.id = changeRequest.id;
            this.cooperative_id = changeRequest.cooperative_id;
            this.public_call_id = changeRequest.public_call_id;
            this.food_id = changeRequest.food_id;
            this.message = changeRequest.message;
            this.title = changeRequest.title;
            this.creation_date = changeRequest.creation_date.ToLocalTime();
            this.response_date = changeRequest.response_date?.ToLocalTime();
            this.not_visible = changeRequest.not_visible;
            this.is_response = changeRequest.is_response;
        }

        public Guid id                  { get; set; }
        public Guid cooperative_id      { get; set; }
        public Guid public_call_id      { get; set; }
        public Guid food_id             { get; set; }
        public string message           { get; set; }
        public string title             { get; set; }
        public DateTime creation_date   { get; set; }
        public DateTime? response_date  { get; set; }
        public bool not_visible         { get; set; }
        public bool is_response         { get; set; }
    }

    public class ChangeRequestGroupedResponse
    {
        public ChangeRequestGroupedResponse(List<ChangeRequest> changeRequestList)
        {
            this.public_calls = new List<ChangeRequestPublicCallResponse>();

            foreach (var groupByPublicCall in changeRequestList.GroupBy(cr => cr.public_call_id))
            {
                var publicCall = new ChangeRequestPublicCallResponse()
                {
                    public_call_id = groupByPublicCall.Key,
                    public_call_name = groupByPublicCall.First().public_call.name
                };

                foreach (var group in groupByPublicCall.GroupBy(cr => cr.cooperative_id))
                {
                    var first = group.OrderByDescending(g => g.creation_date).First();

                    if (!first.is_response)
                    {
                        publicCall.has_problems = true;

                        var message = first.response_date >= DateTime.Now ? "Aguardando resposta" : "Prazo de resposta expirado";
                        publicCall.problems_list.Add($"{first.cooperative.name} - {message}");
                    }
                }

                this.public_calls.Add(publicCall);
            }
        }

        public List<ChangeRequestPublicCallResponse> public_calls   { get; set; }
    }

    public class ChangeRequestPublicCallResponse
    {
        public Guid public_call_id          { get; set; }
        public string public_call_name      { get; set; }
        public bool has_problems            { get; set; } = false;
        public List<string> problems_list   { get; set; } = new List<string>();
    }

    public class ChangeRequestRegister : BaseDomain
    {
        public Guid cooperative_id          { get; set; }
        public Guid public_call_id          { get; set; }
        public Guid food_id                 { get; set; }
        public string message               { get; set; }
        public string title                 { get; set; }
        public DateTime response_date       { get; set; }
        public bool requires_new_upload     { get; set; }
        public List<Guid> refused_documents { get; set; } = new List<Guid>();

        public override bool EhValido()
        {
            ValidationResult = new ChangeRequestRegisterValidation().Validate(this);
            return ValidationResult.IsValid;
        }
    }
}
