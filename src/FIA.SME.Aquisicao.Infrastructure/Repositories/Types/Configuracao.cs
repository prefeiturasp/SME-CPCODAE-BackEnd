using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FIA.SME.Aquisicao.Infrastructure.Repositories.Types
{
    [Table("configuracao")]
    internal class Configuracao
    {
        [Key]
        public Guid id          { get; set; }
        public string nome      { get; set; }
        public string valor     { get; set; }
        public bool ativa       { get; set; }
    }
}
