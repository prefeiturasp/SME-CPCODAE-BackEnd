using System.ComponentModel;

namespace FIA.SME.Aquisicao.Core.Enums
{
    public enum PublicCallStatusEnum
    {
        [Description("Aberta")] // Quando uma chamada é cadastrada e está dentro do prazo para recebimento de projetos de venda
        Aberta = 0,
        [Description("Em Andamento")] // Após o término do prazo de recebimento dos projetos e durante análise de documentos
        EmAndamento = 1,
        [Description("Aprovada")] // Quando os fornecedores da chamada tiverem sido selecionados
        Aprovada = 2,
        [Description("Homologada")] // Quando os fornecedores da chamada tiverem sido selecionados e todos os trâmites jurídicos tiverem sido feitos
        Homologada = 3,
        [Description("Contratada")] // Após a assinatura do contrato o cronograma se inicia
        Contratada = 4,
        [Description("Cronograma Executado")] // 100% entregue
        CronogramaExecutado = 5,
        [Description("Suspensa")] // Terá efeito temporário e será futuramente cancelada e cadastrada uma nova chamada
        Suspensa = 6,
        [Description("Cancelada")]
        Cancelada = 7,
        [Description("Deserta")] // Quando a chamada não tiver nenhuma resposta de cooperativa (status final)
        Deserta = 8
    }
}
