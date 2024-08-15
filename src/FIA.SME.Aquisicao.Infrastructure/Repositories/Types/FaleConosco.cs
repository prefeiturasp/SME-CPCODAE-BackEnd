using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FIA.SME.Aquisicao.Infrastructure.Repositories.Types
{
    [Table("fale_conosco")]
    internal class FaleConosco
    {
        [Key]
        public Guid id          { get; set; }
        [ForeignKey("ChamadaPublica")]
        public Guid chamada_publica_id { get; set; }
        [ForeignKey("Cooperativa")]
        public Guid cooperativa_id { get; set; }
        public string assunto { get; set; }
        public string mensagem { get; set; }
        public DateTime data_criacao { get; set; }

        public virtual ChamadaPublica ChamadaPublica { get; set; }
        public virtual Cooperativa Cooperativa { get; set; }
    }
}
