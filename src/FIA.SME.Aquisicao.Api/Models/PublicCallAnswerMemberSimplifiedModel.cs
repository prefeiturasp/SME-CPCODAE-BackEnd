using FIA.SME.Aquisicao.Core.Helpers;

namespace FIA.SME.Aquisicao.Api.Models
{
    public class PublicCallAnswerMemberSimplifiedModel
    {
        public PublicCallAnswerMemberSimplifiedModel(string cpf, string dap_caf_code, decimal price, decimal quantity)
        {
            this.cpf = cpf?.FormatCPF();
            this.dap_caf_code = dap_caf_code;
            this.preco = price.ToString("C");
            this.quantidade = quantity.ToString("N");
        }

        public string? cpf { get; private set; }
        public string dap_caf_code { get; private set; }
        public string preco { get; private set; }
        public string quantidade { get; private set; }
    }
}
