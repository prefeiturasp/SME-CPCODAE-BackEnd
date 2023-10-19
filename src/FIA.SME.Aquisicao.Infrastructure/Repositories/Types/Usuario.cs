using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FIA.SME.Aquisicao.Infrastructure.Repositories.Types
{
    [Table("usuario")]
    internal class Usuario
    {
        [Key]
        [Column("id")]
        public Guid id          { get; set; }
        [Column("nome")]
        public string nome      { get; set; } = String.Empty;
        [Column("email")]
        public string email     { get; set; } = String.Empty;
        [Column("senha")]
        public string senha     { get; set; } = String.Empty;
        [Column("perfil")]
        public string perfil    { get; set; } = String.Empty;
        [Column("ativo")]
        public bool ativo       { get; set; }
    }
}
