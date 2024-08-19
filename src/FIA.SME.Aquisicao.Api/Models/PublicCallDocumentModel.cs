using FIA.SME.Aquisicao.Infrastructure.Models;

namespace FIA.SME.Aquisicao.Api.Models
{
    public class PublicCallDocumentResponse
    {
        public PublicCallDocumentResponse(PublicCallDocument? publicaCallDocument)
        {
            if (publicaCallDocument == null)
                return;

            this.id = publicaCallDocument.id;
            this.document_type_id = publicaCallDocument.document_type_id;
            this.food_id = publicaCallDocument.food_id;
            this.public_call_id = publicaCallDocument.public_call_id;
            this.document_type_name = publicaCallDocument.document_type_name;
            this.application = publicaCallDocument.application;
            this.is_reviewed = publicaCallDocument.is_reviewed;
        }

        public Guid id                      { get; set; }
        public Guid document_type_id        { get; set; }
        public Guid? food_id                { get; set; }
        public Guid public_call_id          { get; set; }
        public string document_type_name    { get; set; }
        public int application              { get; set; }
        public bool is_reviewed             { get; set; }
    }
}
