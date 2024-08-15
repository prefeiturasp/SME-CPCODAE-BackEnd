using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FIA.SME.Aquisicao.Infrastructure.Repositories.Types
{
    [Table("documento_cooperativa")]
    internal class DocumentoCooperativa
    {
        [Key]
        public Guid id                  { get; set; }
        [ForeignKey("Alimento")]
        public Guid? alimento_id        { get; set; }
        [ForeignKey("Cooperativa")]
        public Guid cooperativa_id      { get; set; }
        [ForeignKey("ChamadaPublica")]
        public Guid? chamada_publica_id { get; set; }
        [ForeignKey("Usuario")]
        public Guid? usuario_id { get; set; }
        [ForeignKey("TipoDocumento")]
        public Guid tipo_documento_id   { get; set; }
        public string documento_path    { get; set; }
        public Int64 documento_tamanho  { get; set; }
        public DateTime data_criacao    { get; set; }
        public DateTime? data_revisao { get; set; }
        public bool is_atual            { get; set; }
        public bool is_revisado         { get; set; }

        public virtual Alimento Alimento                { get; set; }
        public virtual Cooperativa Cooperativa          { get; set; }
        public virtual ChamadaPublica ChamadaPublica    { get; set; }
        public virtual TipoDocumento TipoDocumento      { get; set; }
        public virtual Usuario? Usuario { get; set; }
    }
}
