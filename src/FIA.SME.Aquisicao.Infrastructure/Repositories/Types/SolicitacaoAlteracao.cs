using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FIA.SME.Aquisicao.Infrastructure.Repositories.Types
{
    [Table("solicitacao_alteracao")]
    internal class SolicitacaoAlteracao
    {
        [Key]
        public Guid id                          { get; set; }
        [ForeignKey("ChamadaPublica")]
        public Guid chamada_publica_id          { get; set; }
        [ForeignKey("Cooperativa")]
        public Guid cooperativa_id              { get; set; }
        [ForeignKey("Alimento")]
        public Guid alimento_id                 { get; set; }
        public string mensagem                  { get; set; }
        public string titulo                    { get; set; }
        public DateTime data_criacao            { get; set; }
        public DateTime? data_maxima_resposta   { get; set; }
        public bool exige_upload                { get; set; }
        public bool is_invisivel                { get; set; }
        public bool is_resposta                 { get; set; }

        public virtual Alimento Alimento                { get; set; }
        public virtual ChamadaPublica ChamadaPublica    { get; set; }
        public virtual Cooperativa Cooperativa          { get; set; }        
    }
}
