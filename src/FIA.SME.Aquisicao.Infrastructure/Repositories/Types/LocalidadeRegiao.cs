using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FIA.SME.Aquisicao.Infrastructure.Repositories.Types
{
    [Table("localidade_regiao")]
    internal class LocalidadeRegiao
    {
        [Key]
        [Column("id")]
        public int id                       { get; set; }
        [Column("municipio")]
        public string municipio             { get; set; } = String.Empty;
        [Column("uf")]
        public string uf                    { get; set; } = String.Empty;
        [Column("regiao_imediata_id")]
        public int regiao_imediata_id       { get; set; }
        [Column("regiao_intermediaria_id")]
        public int regiao_intermediaria_id  { get; set; }
    }
}
