using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FIA.SME.Aquisicao.Infrastructure.Repositories.Types
{
    [Table("cooperativa")]
    internal class Cooperativa
    {
        [Key]
        public Guid id                                  { get; set; }
        [ForeignKey("Banco")]
        public Guid? banco_id                           { get; set; }
        [ForeignKey("Endereco")]
        public Guid? endereco_id                        { get; set; }
        [ForeignKey("RepresentanteLegal")]
        public Guid? representante_legal_id             { get; set; }
        [ForeignKey("Usuario")]
        public Guid usuario_id                          { get; set; }
        public string cnpj                              { get; set; }
        public string? cnpj_central                     { get; set; }
        public string codigo_dap_caf                    { get; set; }
        public string? email                            { get; set; }
        public string ip_aceite_termos_uso              { get; set; }
        public string? razao_social                     { get; set; }
        public string? sigla                            { get; set; }
        public string? telefone                         { get; set; }
        public int situacao                             { get; set; }
        public int tipo_pessoa_juridica                 { get; set; }
        public int tipo_producao                        { get; set; }
        public DateTime data_aceite_termos_uso          { get; set; }
        public DateTime data_criacao                    { get; set; }
        public DateTime? data_emissao_dap_caf           { get; set; }
        public DateTime? data_validade_dap_caf          { get; set; }
        public bool is_dap                              { get; set; }
        public bool ativa                               { get; set; }

        public virtual Banco Banco                                  { get; set; }
        public virtual Endereco Endereco                            { get; set; }
        public virtual RepresentanteLegal RepresentanteLegal        { get; set; }
        public virtual Usuario Usuario                              { get; set; }

        public virtual ICollection<Cooperado> Cooperados            { get; set; }
        public virtual ICollection<DocumentoCooperativa> Documentos { get; set; }
    }
}
