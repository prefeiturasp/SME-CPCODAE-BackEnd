using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FIA.SME.Aquisicao.Infrastructure.Repositories.Types
{
    [Table("endereco")]
    internal class Endereco
    {
        [Key]
        public Guid id                  { get; set; }
        public int codigo_cidade_ibge   { get; set; }
        public string bairro            { get; set; }
        public string cep               { get; set; }
        public string? complemento      { get; set; }
        public string logradouro        { get; set; }
        public string numero            { get; set; }
    }
}
