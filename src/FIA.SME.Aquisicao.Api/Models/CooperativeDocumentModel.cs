using FIA.SME.Aquisicao.Core.Helpers;
using FIA.SME.Aquisicao.Infrastructure.Models;

namespace FIA.SME.Aquisicao.Api.Models
{
    public class CooperativeDocumentResponse
    {
        public CooperativeDocumentResponse(CooperativeDocument document)
        {
            if (document == null)
                return;

            this.id = document.id;
            this.cooperative_id = document.cooperative_id;
            this.document_type_id = document.document_type_id;
            this.food_id = document.food_id;
            this.public_call_id = document.public_call_id;
            this.document_type_name = document.document_type_name;
            this.document_path = document.document_path;
            this._file_size = document.file_size;
            this.creation_date = document.creation_date.ToLocalTime();
            this.application = document.application;
            this.is_current = document.is_current;
            this.is_reviewed = document.is_reviewed;
        }

        private Int64 _file_size;

        public Guid id                      { get; set; }
        public Guid cooperative_id          { get; set; }
        public Guid document_type_id        { get; set; }
        public Guid? food_id                { get; set; }
        public Guid? public_call_id         { get; set; }
        public string document_type_name    { get; set; } = String.Empty;
        public string document_path         { get; set; } = String.Empty;
        public string file_name             { get { return String.IsNullOrEmpty(this.document_path) ? String.Empty : System.IO.Path.GetFileName(this.document_path); } }
        public string file_size             { get { return SMEHelper.GetSizeWithSuffix(this._file_size); } }
        public DateTime creation_date       { get; set; }
        public int application              { get; set; }
        public bool is_current              { get; set; }
        public bool is_reviewed              { get; set; }
    }

    public class CooperativeDocumentRegister
    {
        public Guid cooperative_id      { get; set; }
        public Guid document_type_id    { get; set; }
        public Guid? public_call_id     { get; set; }
        public string file_base_64      { get; set; } = String.Empty;
        public Int64 file_size          { get; set; }
    }

    public class CooperativeDocumentRemove
    {
        public Guid id  { get; set; }
    }

    public class CooperativeDocumentSetAsReviewed
    {
        public bool is_reviewed { get; set; }
    }
}
