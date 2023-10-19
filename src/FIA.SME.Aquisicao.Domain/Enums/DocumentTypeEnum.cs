using System.ComponentModel;

namespace FIA.SME.Aquisicao.Core.Enums
{
    public enum DocumentTypeEnum
    {
        [Description("Proposta")]
        Proposta = 1,
        [Description("Cadastro Cooperativa")]
        CadastroCooperativa = 2,
        [Description("Proposta / Cadastro Cooperativa")]
        PropostaCadastroCooperativa = 3
    }
}
