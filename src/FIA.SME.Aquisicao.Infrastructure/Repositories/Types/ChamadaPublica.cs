using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FIA.SME.Aquisicao.Infrastructure.Repositories.Types
{
    [Table("chamada_publica")]
    internal class ChamadaPublica
    {
        [Key]
        public Guid id                          { get; set; }
        public int codigo_cidade_ibge           { get; set; }
        public string edital_url                { get; set; }
        public string estimativa                { get; set; }
        public string? informacoes_adicionais   { get; set; }
        public string nome                      { get; set; }
        public string numero                    { get; set; }
        public string objeto                    { get; set; }
        public string orgao                     { get; set; }
        public string processo                  { get; set; }
        public string sessao_publica_local      { get; set; }
        public string sessao_publica_url        { get; set; }
        public string tipo                      { get; set; }
        public int status_id                    { get; set; }
        public DateTime data_criacao            { get; set; }
        public DateTime data_inscricao_inicio   { get; set; }
        public DateTime data_inscricao_termino  { get; set; }
        public DateTime data_sessao_publica     { get; set; }
        public DateTime? data_habilitacao       { get; set; }
        public DateTime? data_homologacao       { get; set; }
        public DateTime? data_contratacao       { get; set; }
        public DateTime? data_contrato_executado { get; set; }
        public DateTime? data_suspensao         { get; set; }
        public DateTime? data_cancelamento      { get; set; }
        public DateTime? data_deserta           { get; set; }
        public bool ativa                       { get; set; }

        public virtual ICollection<ChamadaPublicaAlimento> ChamadaPublicaAlimentos { get; set; }
        public virtual ICollection<ChamadaPublicaDocumento> ChamadaPublicaDocumentos { get; set; }
    }
}
