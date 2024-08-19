using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FIA.SME.Aquisicao.Infrastructure.Repositories.Types
{
    [Table("cooperado")]
    internal class Cooperado
    {
        [Key]
        public Guid id                          { get; set; }
        [ForeignKey("Cooperativa")]
        public Guid cooperativa_id              { get; set; }
        [ForeignKey("Endereco")]
        public Guid? endereco_id                { get; set; }
        public string codigo_dap_caf            { get; set; }
        public string? cpf                      { get; set; }
        public string? nome                     { get; set; }
        public int? tipo_agricultor_familiar    { get; set; }
        public int? tipo_producao               { get; set; }
        public DateTime? data_emissao_dap_caf   { get; set; }
        public DateTime? data_validade_dap_caf  { get; set; }
        public bool is_dap                      { get; set; }
        public bool is_masculino                { get; set; }
        public bool ativo                       { get; set; }

        public virtual Cooperativa Cooperativa  { get; set; }
        public virtual Endereco Endereco        { get; set; }
    }
}
