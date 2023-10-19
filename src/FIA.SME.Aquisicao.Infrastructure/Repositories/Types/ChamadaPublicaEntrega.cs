using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FIA.SME.Aquisicao.Infrastructure.Repositories.Types
{
    [Table("chamada_publica_entrega")]
    internal class ChamadaPublicaEntrega
    {
        [Key]
        public Guid id                              { get; set; }
        [ForeignKey("ChamadaPublicaResposta")]
        public Guid chamada_publica_resposta_id     { get; set; }
        [ForeignKey("UsuarioConfirmacaoEntrega")]
        public Guid? usuario_confirmacao_entrega_id { get; set; }
        public decimal quantidade_prevista_entrega  { get; set; }
        public decimal? quantidade_entregue         { get; set; }
        public DateTime data_criacao                { get; set; }
        public DateTime data_prevista_entrega       { get; set; }
        public DateTime? data_confirmacao_entrega   { get; set; }
        public bool foi_entregue                    { get; set; }

        public virtual ChamadaPublicaResposta ChamadaPublicaResposta    { get; set; }
        public virtual Usuario UsuarioConfirmacaoEntrega                { get; set; }
    }
}
