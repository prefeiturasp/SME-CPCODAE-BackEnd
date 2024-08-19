using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FIA.SME.Aquisicao.Infrastructure.Repositories.Types
{
    [Table("chamada_publica_resposta_cooperado")]
    internal class ChamadaPublicaRespostaCooperado
    {
        [Key]
        public Guid id { get; set; }
        [ForeignKey("ChamadaPublicaResposta")]
        public Guid chamada_publica_resposta_id { get; set; }
        [ForeignKey("Cooperado")]
        public Guid cooperado_id { get; set; }
        public decimal preco { get; set; }
        public decimal quantidade { get; set; }
        public bool validado { get; set; }

        public virtual ChamadaPublicaResposta ChamadaPublicaResposta { get; set; }
        public virtual Cooperado Cooperado { get; set; }
    }
}
