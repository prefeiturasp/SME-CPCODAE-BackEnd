using FIA.SME.Aquisicao.Core.Enums;
using FIA.SME.Aquisicao.Core.Helpers;

namespace FIA.SME.Aquisicao.Infrastructure.Models
{
    public class PublicCallReport
    {
        #region [ Propriedades ]

        public Guid chamada_publica_id { get; set; }
        public Guid alimento_id { get; set; }
        public string numero { get; set; }
        public string nome { get; set; }
        public string processo { get; set; }
        public string item { get; set; }
        public decimal quantidade { get; set; }
        public decimal preco_unitario { get; set; }
        public decimal preco_total { get; set; }
        public string? cnpj_proponente { get; set; }
        public string? nome_proponente { get; set; }
        public string? sigla_proponente { get; set; }
        public string? item_projeto_venda { get; set; }
        public decimal? quantidade_comprada { get; set; }
        public decimal? quantidade_item_projeto_venda { get; set; }
        public decimal? preco_unitario_item_projeto_venda
        {
            get
            {
                return (decimal)(preco_total_item_projeto_venda ?? 0) / (quantidade_item_projeto_venda ?? 1);
            }
        }
        public decimal? preco_total_item_projeto_venda { get; set; }
        public string? sistema_producao { get; set; }
        public string somente_mulheres { get; set; } = String.Empty;
        public int? total_associados { get; set; }
        public decimal? porcentagem_pnra { get; set; }
        public decimal? porcentagem_quilombola { get; set; }
        public decimal? porcentagem_indigena { get; set; }
        public decimal? porcentagem_demais_associados { get; set; }
        public decimal? porcentagem_daps_cafs { get; set; }
        public int? codigo_cidade_ibge { get; set; }
        public int? proposal_city_id { get; set; }
        public string? municipio_maior_numero_associados { get; set; }
        public string? localizacao { get; set; }
        public DateTime data_sessao_publica { get; set; }
        public DateTime data_abertura { get; set; }
        public DateTime? data_habilitacao { get; set; }
        public DateTime? data_homologacao { get; set; }
        public DateTime? data_contratacao { get; set; }
        public DateTime? data_contrato_executado { get; set; }
        public int status_id { get; set; }
        public int? unidade_medida_enum { get; set; }
        public decimal? quantidade_homologada { get; set; }

        public string unidade_medida
        {
            get
            {
                var measureUnit = (MeasureUnitEnum)(unidade_medida_enum ?? 0);

                return measureUnit.DescriptionAttr();
            }
        }

        #endregion [ FIM - Propriedades ]
    }

    public class PublicCallReportCooperative
    {
        public Guid chamada_publica_id { get; set; }
        public string numero { get; set; } = String.Empty;
        public string nome { get; set; } = String.Empty;
        public string processo { get; set; } = String.Empty;
        public int cooperativas_inscritas { get; set; }
        public int cooperativas_habilitadas { get; set; }
        public int cooperativas_inabilitadas { get; set; }
        public DateTime data_sessao_publica { get; set; }
        public int status_id { get; set; }
    }
}
