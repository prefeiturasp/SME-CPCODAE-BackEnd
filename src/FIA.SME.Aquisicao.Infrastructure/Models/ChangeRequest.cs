using FIA.SME.Aquisicao.Infrastructure.Repositories.Types;

namespace FIA.SME.Aquisicao.Infrastructure.Models
{
    public class ChangeRequest
    {
        #region [ Construtores ]

        internal ChangeRequest(SolicitacaoAlteracao solicitacaoAlteracao)
        {
            this.id = solicitacaoAlteracao.id;
            this.food_id = solicitacaoAlteracao.alimento_id;
            this.cooperative_id = solicitacaoAlteracao.cooperativa_id;
            this.public_call_id = solicitacaoAlteracao.chamada_publica_id;
            this.message = solicitacaoAlteracao.mensagem;
            this.title = solicitacaoAlteracao.titulo;
            this.creation_date = solicitacaoAlteracao.data_criacao;
            this.response_date = solicitacaoAlteracao.data_maxima_resposta;
            this.requires_new_upload = solicitacaoAlteracao.exige_upload;
            this.not_visible = solicitacaoAlteracao.is_invisivel;
            this.is_response = solicitacaoAlteracao.is_resposta;

            if (solicitacaoAlteracao.ChamadaPublica != null)
            {
                this.public_call = new PublicCall(solicitacaoAlteracao.ChamadaPublica);
            }

            if (solicitacaoAlteracao.Cooperativa != null)
            {
                this.cooperative = new Cooperative(solicitacaoAlteracao.Cooperativa);
            }
        }

        public ChangeRequest(Guid cooperative_id, Guid public_call_id, Guid food_id, string message, string title, DateTime? response_date, bool is_response, bool requires_new_upload, List<Guid> refused_documents)
        {
            this.cooperative_id = cooperative_id;
            this.public_call_id = public_call_id;
            this.food_id = food_id;
            this.message = message;
            this.title = title;
            this.response_date = response_date;
            this.is_response = is_response;
            this.not_visible = false;
            this.requires_new_upload = requires_new_upload;
            this.refused_documents = refused_documents;
        }

        public ChangeRequest(Guid cooperative_id, Guid public_call_id, Guid food_id, string message, string title, DateTime? response_date, bool is_response, bool requires_new_upload, List<Guid> refused_documents, bool not_visible)
            : this (cooperative_id, public_call_id, food_id, message, title, response_date, is_response, requires_new_upload, refused_documents)
        {
            this.not_visible = not_visible;
        }

        #endregion [ FIM - Construtores ]

        #region [ Propriedades ]

        public Guid id                      { get; private set; }
        public Guid cooperative_id          { get; private set; }
        public Guid public_call_id          { get; private set; }
        public Guid food_id                 { get; private set; }
        public string message               { get; private set; }
        public string title                 { get; private set; }
        public DateTime creation_date       { get; private set; }
        public DateTime? response_date      { get; private set; }
        public bool is_response             { get; private set; }
        public bool requires_new_upload     { get; private set; }
        public bool not_visible             { get; private set; }
        public List<Guid> refused_documents { get; private set; }

        public Cooperative cooperative      { get; private set; }
        public PublicCall public_call       { get; private set; }

        #endregion [ FIM - Propriedades ]

        #region [ Metodos ]

        public void SetId(Guid id)
        {
            this.id = id;
        }

        #endregion [ FIM - Metodos ]
    }
}
