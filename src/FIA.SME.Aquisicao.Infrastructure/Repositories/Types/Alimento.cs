using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FIA.SME.Aquisicao.Infrastructure.Repositories.Types
{
    [Table("alimento")]
    internal class Alimento
    {
        [Key]
        public Guid id              { get; set; }
        [ForeignKey("Categoria")]
        public Guid categoria_id    { get; set; }
        public int unidade_medida   { get; set; }
        public string nome          { get; set; }
        public bool ativo           { get; set; }

        public virtual Categoria Categoria { get; set; }
    }
}
