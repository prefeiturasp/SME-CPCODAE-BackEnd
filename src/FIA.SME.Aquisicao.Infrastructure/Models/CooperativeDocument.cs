using FIA.SME.Aquisicao.Infrastructure.Repositories.Types;

namespace FIA.SME.Aquisicao.Infrastructure.Models
{
    public class CooperativeDocument
    {
        #region [ Construtores ]

        public CooperativeDocument(Guid cooperative_id, Guid document_type_id, Guid? public_call_id, string document_path, Int64 file_size, Guid? food_id)
        {
            this.cooperative_id = cooperative_id;
            this.document_type_id = document_type_id;
            this.public_call_id = public_call_id;
            this.document_path = document_path;
            this.file_size = file_size;
            this.food_id = food_id;
        }

        internal CooperativeDocument(DocumentoCooperativa documento)
        {
            if (documento == null)
                return;

            this.id = documento.id;
            this.cooperative_id = documento.cooperativa_id;
            this.document_type_id = documento.tipo_documento_id;
            this.food_id = documento.alimento_id;
            this.public_call_id = documento.chamada_publica_id;
            this.document_path = documento.documento_path;
            this.file_size = documento.documento_tamanho;
            this.creation_date = documento.data_criacao;
            this.is_current = documento.is_atual;
            this.is_reviewed = documento.is_revisado;

            if (documento.TipoDocumento != null)
            {
                this.document_type_name = documento.TipoDocumento.nome;
                this.application = documento.TipoDocumento.aplicacao;
            }
        }

        #endregion [ FIM - Construtores ]

        #region [ Propriedades ]

        public Guid id                      { get; private set; }
        public Guid cooperative_id          { get; private set; }
        public Guid document_type_id        { get; private set; }
        public Guid? food_id                { get; private set; }
        public Guid? public_call_id         { get; private set; }
        public string document_type_name    { get; private set; }
        public string document_path         { get; private set; }
        public Int64 file_size              { get; private set; }
        public DateTime creation_date       { get; private set; }
        public int application              { get; private set; }
        public bool is_current              { get; private set; } = false;
        public bool is_reviewed             { get; private set; } = false;

        #endregion [ FIM - Propriedades ]

        #region [ Metodos ]

        public void Disable()
        {
            this.is_current = false;
        }

        public void SetId(Guid id)
        {
            this.id = id;
            this.is_current = true;
        }

        public void SetAsReviewed(bool isReviewed)
        {
            this.is_reviewed = isReviewed;
        }

        #endregion [ FIM - Metodos ]
    }
}
