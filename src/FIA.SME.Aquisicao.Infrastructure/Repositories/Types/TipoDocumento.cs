using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FIA.SME.Aquisicao.Infrastructure.Repositories.Types
{
    [Table("tipo_documento")]
    internal class TipoDocumento
    {
        [Key]
        public Guid id          { get; set; }
        public string nome      { get; set; }
        public int aplicacao    { get; set; }
        public bool visivel     { get; set; }
        public bool ativo       { get; set; }
    }
}
