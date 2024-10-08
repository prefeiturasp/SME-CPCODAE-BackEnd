﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FIA.SME.Aquisicao.Infrastructure.Repositories.Types
{
    [Table("representante_legal")]
    internal class RepresentanteLegal
    {
        [Key]
        public Guid id          { get; set; }
        [ForeignKey("Endereco")]
        public Guid endereco_id { get; set; }
        public string cpf       { get; set; }
        public string nome      { get; set; }
        public string telefone  { get; set; }
        public string cargo { get; set; } = String.Empty;
        public DateTime? data_vigencia_mandato { get; set; }
        public int estado_civil { get; set; }

        public virtual Endereco Endereco { get; set; }
    }
}
