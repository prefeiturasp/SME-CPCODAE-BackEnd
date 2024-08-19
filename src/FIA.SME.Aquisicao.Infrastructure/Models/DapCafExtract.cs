using CsvHelper.Configuration.Attributes;

namespace FIA.SME.Aquisicao.Infrastructure.Models
{
    public class DapCafExtract
    {
        [Index(0)]
        public string cpf       { get; set; }
        [Index(1)]
        public string nome      { get; set; }
        [Index(2)]
        public string dap       { get; set; }
        [Index(3)]
        public string municipio { get; set; }
        [Index(4)]
        public string uf        { get; set; }
        [Index(5)]
        public string validade  { get; set; }
        [Index(6)]
        public string enquadramento { get; set; }
    }
}
