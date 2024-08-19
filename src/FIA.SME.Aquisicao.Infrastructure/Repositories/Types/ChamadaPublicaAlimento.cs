using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FIA.SME.Aquisicao.Infrastructure.Repositories.Types
{
    [Table("chamada_publica_alimento")]
    internal class ChamadaPublicaAlimento
    {
        [Key]
        public Guid id                  { get; set; }
        [ForeignKey("Alimento")]
        public Guid alimento_id         { get; set; }
        [ForeignKey("ChamadaPublica")]
        public Guid chamada_publica_id  { get; set; }
        public decimal preco            { get; set; }
        public decimal quantidade       { get; set; }
        public DateTime data_criacao    { get; set; }
        public bool aceita_organico     { get; set; }
        public bool organico            { get; set; }
        public bool ativo               { get; set; }

        public virtual Alimento Alimento                { get; set; }
        public virtual ChamadaPublica ChamadaPublica    { get; set; }
    }
}
