using FIA.SME.Aquisicao.Core.Helpers;
using FIA.SME.Aquisicao.Infrastructure.Repositories.Types;

namespace FIA.SME.Aquisicao.Infrastructure.Models
{
    public class PublicCallAnswerMember
    {
        #region [ Construtores ]

        internal PublicCallAnswerMember() { }

        public PublicCallAnswerMember(Guid cooperado_id, decimal price, decimal quantity)
        {
            this.member_id = cooperado_id;
            this.price = price;
            this.quantity = quantity;
        }

        internal PublicCallAnswerMember(ChamadaPublicaRespostaCooperado chamadaPublicaRespostaCooperado)
        {
            this.id = chamadaPublicaRespostaCooperado.id;
            this.member_id = chamadaPublicaRespostaCooperado.cooperado_id;
            this.public_call_answer_id = chamadaPublicaRespostaCooperado.chamada_publica_resposta_id;
            this.price = chamadaPublicaRespostaCooperado.preco;
            this.quantity = chamadaPublicaRespostaCooperado.quantidade;
            this.validated = chamadaPublicaRespostaCooperado.validado;

            if (chamadaPublicaRespostaCooperado.ChamadaPublicaResposta != null)
            {
                this.public_call_answer = new PublicCallAnswer(chamadaPublicaRespostaCooperado.ChamadaPublicaResposta);
            }

            if (chamadaPublicaRespostaCooperado.Cooperado != null)
            {
                this.member = new CooperativeMember(chamadaPublicaRespostaCooperado.Cooperado);
            }
        }

        #endregion [ FIM - Construtores ]

        #region [ Propriedades ]

        public Guid id                              { get; private set; }
        public Guid member_id                       { get; private set; }
        public Guid public_call_answer_id           { get; private set; }
        public decimal price                        { get; private set; }
        public decimal quantity                     { get; private set; }
        public bool validated                       { get; private set; }

        public CooperativeMember member             { get; private set; }
        public PublicCallAnswer public_call_answer  { get; private set; }

        #endregion [ FIM - Propriedades ]

        #region [ Metodos ]

        public void SetIds(Guid id, Guid publicCallAnswerId)
        {
            this.id = id;
            this.public_call_answer_id = publicCallAnswerId;
        }

        public void SetIsValidated()
        {
            this.validated = true;
        }

        #endregion [ FIM - Metodos ]
    }

    public class PublicCallAnswerMemberSimplified
    {
        public PublicCallAnswerMemberSimplified(Guid? id, string cpf, string dap_caf_code, decimal price, decimal quantity)
        {
            this.id = id;
            this.cpf = cpf?.ToOnlyNumbers();
            this.dap_caf_code = dap_caf_code;
            this.price = price;
            this.quantity = quantity;
        }

        public Guid? id             { get; private set; }
        public string? cpf          { get; private set; }
        public string dap_caf_code  { get; private set; }
        public decimal price        { get; private set; }
        public decimal quantity     { get; private set; }

        public void SetId(Guid id)
        {
            this.id = id;
        }
    }
}
