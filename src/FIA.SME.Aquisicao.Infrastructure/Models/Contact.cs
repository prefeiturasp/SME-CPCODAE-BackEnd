using FIA.SME.Aquisicao.Infrastructure.Repositories.Types;

namespace FIA.SME.Aquisicao.Infrastructure.Models
{
    public class Contact
    {
        #region [ Construtores ]

        internal Contact(FaleConosco faleConosco)
        {
            if (faleConosco == null)
                return;

            this.id = faleConosco.id;
            this.cooperative_id = faleConosco.cooperativa_id;
            this.public_call_id = faleConosco.chamada_publica_id;
            this.subject = faleConosco.assunto;
            this.message = faleConosco.mensagem;
            this.creation_date = faleConosco.data_criacao;

            if (faleConosco.Cooperativa != null)
            {
                this.cooperative = new Cooperative(faleConosco.Cooperativa);
            }

            if (faleConosco.ChamadaPublica != null)
            {
                this.publicCall = new PublicCall(faleConosco.ChamadaPublica);
            }
        }

        public Contact(Guid cooperativeId, Guid publicCallId, string subject, string message)
        {
            this.id = Guid.NewGuid();
            this.cooperative_id = cooperativeId;
            this.public_call_id = publicCallId;
            this.subject = subject;
            this.message = message;
            this.creation_date = DateTime.Now;
        }

        #endregion [ FIM - Construtores ]

        #region [ Propriedades ]

        public Guid id { get; private set; }
        public Guid cooperative_id { get; private set; }
        public Guid public_call_id { get; private set; }
        public string subject { get; private set; }
        public string message { get; private set; }
        public DateTime creation_date { get; private set; }

        public Cooperative cooperative { get; private set; }
        public PublicCall publicCall { get; private set; }


        #endregion [ FIM - Propriedades ]
    }
}
