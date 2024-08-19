using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FIA.SME.Aquisicao.Infrastructure.Repositories.Types
{
    [Table("categoria")]
    internal class Categoria
    {
        [Key]
        public Guid id          { get; set; }
        public string nome      { get; set; }
        public bool ativa       { get; set; }
    }
}
