using FIA.SME.Aquisicao.Infrastructure.Repositories.Types;

namespace FIA.SME.Aquisicao.Infrastructure.Models
{
    public class PublicCallDocument
    {
        #region [ Construtores ]

        public PublicCallDocument(Guid document_type_id, Guid? food_id)
        {
            this.id = Guid.NewGuid();
            this.document_type_id = document_type_id;
            this.food_id = food_id;
        }

        public PublicCallDocument(Guid? id, Guid document_type_id, Guid? food_id, Guid public_call_id)
        {
            this.id = id ?? Guid.NewGuid();
            this.document_type_id = document_type_id;
            this.food_id = food_id;
            this.public_call_id = public_call_id;
        }

        internal PublicCallDocument(ChamadaPublicaDocumento documento)
        {
            if (documento == null)
                return;

            this.id = documento.id;
            this.food_id = documento.alimento_id;
            this.document_type_id = documento.tipo_documento_id;
            this.public_call_id = documento.chamada_publica_id;

            if (documento.TipoDocumento != null)
            {
                this.document_type_name = documento.TipoDocumento.nome;
                this.application = documento.TipoDocumento.aplicacao;
            }
        }

        #endregion [ FIM - Construtores ]

        #region [ Propriedades ]

        public Guid id                      { get; private set; }
        public Guid? food_id                { get; private set; }
        public Guid document_type_id        { get; private set; }
        public Guid public_call_id          { get; private set; }
        public string document_type_name    { get; private set; }
        public int application              { get; private set; }
        public bool is_reviewed             { get; private set; } = false;

        #endregion [ FIM - Propriedades ]

        #region [ Metodos ]

        public void SetPublicCallId(Guid publicCallId)
        {
            this.public_call_id = publicCallId;
        }

        #endregion [ FIM - Metodos ]
    }
}
