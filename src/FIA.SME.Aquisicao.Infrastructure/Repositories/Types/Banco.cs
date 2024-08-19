using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FIA.SME.Aquisicao.Infrastructure.Repositories.Types
{
    [Table("banco")]
    internal class Banco
    {
        [Key]
        public Guid id              { get; set; }
        public string agencia       { get; set; }
        public string codigo        { get; set; }
        public string nome          { get; set; }
        public string numero_conta  { get; set; }
    }
}
