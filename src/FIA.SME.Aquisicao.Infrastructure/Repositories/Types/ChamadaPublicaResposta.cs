using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FIA.SME.Aquisicao.Infrastructure.Repositories.Types
{
    [Table("chamada_publica_resposta")]
    internal class ChamadaPublicaResposta
    {
        [Key]
        public Guid id                                  { get; set; }
        [ForeignKey("Alimento")]
        public Guid alimento_id                         { get; set; }
        [ForeignKey("ChamadaPublica")]
        public Guid chamada_publica_id                  { get; set; }
        [ForeignKey("Cooperativa")]
        public Guid cooperativa_id                      { get; set; }
        public int codigo_cidade_ibge                   { get; set; }
        public int total_associados_cidade              { get; set; }
        public int total_daps_fisicas                   { get; set; }
        public int total_comunidade_indigena            { get; set; }
        public int total_assentamento_pnra              { get; set; }
        public int total_comunidade_quilombola          { get; set; }
        public int total_outros_agricultores_familiares { get; set; }
        public decimal preco                            { get; set; }
        public decimal quantidade                       { get; set; }
        public decimal? quantidade_editada              { get; set; }
        public bool foi_confirmada                      { get; set; }
        public bool foi_escolhida                       { get; set; }
        public bool organico                            { get; set; }
        public bool somente_mulheres                    { get; set; }
        public bool validada                            { get; set; }
        public bool ativa                               { get; set; }

        public virtual Alimento Alimento                { get; set; }
        public virtual ChamadaPublica ChamadaPublica    { get; set; }
        public virtual Cooperativa Cooperativa          { get; set; }

        public virtual ICollection<ChamadaPublicaEntrega> ChamadaPublicaEntregas { get; set; }
    }
}
