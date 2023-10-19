using System.ComponentModel;

namespace FIA.SME.Aquisicao.Core.Enums
{
    public enum MeasureUnitEnum
    {
        Undefined = 0,
        [Description("Kg")]
        Quilo = 1,
        [Description("Litro")]
        Litro = 2,
        [Description("Un")]
        Unidade = 3,
        [Description("M")]
        Metro = 4,
        [Description("Peça")]
        Peca = 5
    }
}
