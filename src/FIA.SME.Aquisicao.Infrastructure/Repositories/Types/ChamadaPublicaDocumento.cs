using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FIA.SME.Aquisicao.Infrastructure.Repositories.Types
{
    [Table("chamada_publica_documento")]
    internal class ChamadaPublicaDocumento
    {
        [Key]
        public Guid id                  { get; set; }
        [ForeignKey("Alimento")]
        public Guid? alimento_id        { get; set; }
        [ForeignKey("ChamadaPublica")]
        public Guid chamada_publica_id { get; set; }
        [ForeignKey("TipoDocumento")]
        public Guid tipo_documento_id   { get; set; }

        public virtual Alimento Alimento { get; set; }
        public virtual ChamadaPublica ChamadaPublica    { get; set; }
        public virtual TipoDocumento TipoDocumento      { get; set; }
    }
}
