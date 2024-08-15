using FIA.SME.Aquisicao.Core.Enums;
using FIA.SME.Aquisicao.Core.Helpers;
using FIA.SME.Aquisicao.Infrastructure.Models;
using System.Linq;
using System.Text.Json.Serialization;

namespace FIA.SME.Aquisicao.Api.Models
{
    public class PublicCallReportModel
    {
        public PublicCallReportModel(PublicCallReport report)
        {
            if (report is null)
                return;

            this.numero = report.numero;
            this.item = report.item;
            this.quantidade = report.quantidade;
            this.preco_unitario = report.preco_unitario;
            this.preco_total = report.preco_total;
            this.cnpj_proponente = report.cnpj_proponente?.FormatCNPJ();
            this.nome_proponente = report.nome_proponente;
            this.sigla_proponente = report.sigla_proponente;
            this.item_projeto_venda = report.item_projeto_venda;
            this.quantidade_item_projeto_venda = report.quantidade_item_projeto_venda;
            this.unidade_item_projeto_venda = report.unidade_medida;
            this.preco_unitario_item_projeto_venda = report.preco_unitario_item_projeto_venda;
            this.preco_total_item_projeto_venda = report.preco_total_item_projeto_venda;
            this.sistema_producao = report.sistema_producao;
            this.somente_mulheres = report.somente_mulheres;
            this.total_associados = report.total_associados;
            this.porcentagem_pnra = (decimal)(report.porcentagem_pnra ?? 0) / 100;
            this.porcentagem_quilombola = (decimal)(report.porcentagem_quilombola ?? 0) / 100;
            this.porcentagem_indigena = (decimal)(report.porcentagem_indigena ?? 0) / 100;
            this.porcentagem_demais_associados = (decimal)(report.porcentagem_demais_associados ?? 0) / 100;
            this.porcentagem_daps_cafs = (decimal)((report.porcentagem_pnra ?? 0) + (report.porcentagem_quilombola ?? 0) + (report.porcentagem_indigena ?? 0)) / 100;
            this.municipio_maior_numero_associados = report.municipio_maior_numero_associados;
            this.localizacao = report.localizacao;
            this.data_abertura = report.data_abertura.ToString("dd/MM/yyyy");
            this.data_habilitacao = report.data_habilitacao?.ToString("dd/MM/yyyy") ?? "-";
            this.data_homologacao = report.data_homologacao?.ToString("dd/MM/yyyy") ?? "-";
            this.data_contratacao = report.data_contratacao?.ToString("dd/MM/yyyy") ?? "-";

            this.quantidade_homologada = report.quantidade_homologada;
            this.quantidade_adicional = (report.quantidade_comprada ?? 0) > report.quantidade ? report.quantidade_comprada - report.quantidade : (decimal?)null;
        }

        #region [ Propriedades ]

        [JsonPropertyName("Numero")]
        public string numero { get; set; }
        [JsonPropertyName("Item")]
        public string item { get; set; }
        [JsonPropertyName("Quantidade")]
        public decimal quantidade { get; set; }
        [JsonPropertyName("Preco Un")]
        public decimal preco_unitario { get; set; }
        [JsonPropertyName("Preco Total")]
        public decimal preco_total { get; set; }
        [JsonPropertyName("CNPJ Proponente")]
        public string? cnpj_proponente { get; set; }
        [JsonPropertyName("Nome Proponente")]
        public string? nome_proponente { get; set; }
        [JsonPropertyName("Sigla")]
        public string? sigla_proponente { get; set; }
        [JsonPropertyName("Item PV")]
        public string? item_projeto_venda { get; set; }
        [JsonPropertyName("Quantidade PV")]
        public decimal? quantidade_item_projeto_venda { get; set; }
        [JsonPropertyName("Unidade PV")]
        public string? unidade_item_projeto_venda { get; set; }
        [JsonPropertyName("Preco Un PV")]
        public decimal? preco_unitario_item_projeto_venda { get; set; }
        [JsonPropertyName("Preco Total PV")]
        public decimal? preco_total_item_projeto_venda { get; set; }
        [JsonPropertyName("Organico")]
        public string? sistema_producao { get; set; }
        [JsonPropertyName("Somente Mulheres")]
        public string? somente_mulheres { get; set; }
        [JsonPropertyName("Numero Associados no PV")]
        public int? total_associados { get; set; }
        [JsonPropertyName("% PRNA")]
        public decimal? porcentagem_pnra { get; set; }
        [JsonPropertyName("% Quilombolas")]
        public decimal? porcentagem_quilombola { get; set; }
        [JsonPropertyName("% Indigena")]
        public decimal? porcentagem_indigena { get; set; }
        [JsonPropertyName("% Demais Associados")]
        public decimal? porcentagem_demais_associados { get; set; }
        [JsonPropertyName("% Associados Daps/Cafs")]
        public decimal? porcentagem_daps_cafs { get; set; }
        [JsonPropertyName("Municipio Maior Numero Associados")]
        public string? municipio_maior_numero_associados { get; set; }
        [JsonPropertyName("Localizacao")]
        public string? localizacao { get; set; }
        [JsonPropertyName("Data Abertura CP")]
        public string data_abertura { get; set; }
        [JsonPropertyName("Data Documentos Analisados CP")]
        public string? data_habilitacao { get; set; }
        [JsonPropertyName("Data Homologacao CP")]
        public string? data_homologacao { get; set; }
        [JsonPropertyName("Data Contratacao CP")]
        public string? data_contratacao { get; set; }
        [JsonPropertyName("Quantidade Homologada")]
        public decimal? quantidade_homologada { get; set; }
        [JsonPropertyName("Quantidade Adicional")]
        public decimal? quantidade_adicional { get; set; }

        #endregion [ FIM - Propriedades ]
    }

    public class PublicCallReportTotalizadoModel
    {
        public PublicCallReportTotalizadoModel(List<PublicCallReport> chamadas)
        {
            if (chamadas is null)
                return;

            var chamadasAgrupadasPorId = chamadas.GroupBy(c => c.chamada_publica_id)
            .Select(g => new
            {
                chamada_publica_id = g.Key,
                status_id = g.Min(i => i.status_id),
                quantidade = g.GroupBy(ga => ga.alimento_id).Select(ga => new { alimento_id = ga.Key, quantidade = ga.Min(c => c.quantidade) }).DefaultIfEmpty(new { alimento_id = Guid.NewGuid(), quantidade = 0m }).Sum(ga => ga.quantidade),
                quantidade_comprada = g.GroupBy(ga => ga.alimento_id).Select(ga => new { alimento_id = ga.Key, quantidade_comprada = ga.Min(c => (c.quantidade_comprada ?? 0m)) }).DefaultIfEmpty(new { alimento_id = Guid.NewGuid(), quantidade_comprada = 0m }).Sum(ga => ga.quantidade_comprada)
            })
            .ToList();

            foreach (var item in chamadasAgrupadasPorId)
            {
                this.total++;

                if (item.status_id == (int)PublicCallStatusEnum.EmAndamento)
                    this.em_andamentos++;

                if (item.status_id == (int)PublicCallStatusEnum.Aprovada)
                    this.aprovadas++;

                if (item.status_id == (int)PublicCallStatusEnum.Homologada)
                    this.homologadas++;

                if (item.status_id == (int)PublicCallStatusEnum.Contratada)
                    this.contratadas++;

                if (item.status_id == (int)PublicCallStatusEnum.CronogramaExecutado)
                    this.cronogramas_executados++;

                if (item.status_id == (int)PublicCallStatusEnum.Suspensa)
                    this.suspensas++;

                if (item.status_id == (int)PublicCallStatusEnum.Cancelada)
                    this.canceladas++;

                if (item.status_id == (int)PublicCallStatusEnum.Deserta)
                    this.desertas++;

                if (item.status_id >= (int)PublicCallStatusEnum.Contratada && item.status_id <= (int)PublicCallStatusEnum.CronogramaExecutado)
                {
                    if (item.quantidade <= item.quantidade_comprada)
                        this.exitosas++;
                    else
                        this.parcialmente_exitosas++;
                }
            }
        }

        #region [ Propriedades ]

        [JsonPropertyName("Total")]
        public int total { get; set; }
        [JsonPropertyName("Em Andamento")]
        public int em_andamentos { get; set; }
        [JsonPropertyName("Aprovada")]
        public int aprovadas { get; set; }
        [JsonPropertyName("Homologada")]
        public int homologadas { get; set; }
        [JsonPropertyName("Contratadas")]
        public int contratadas { get; set; }
        [JsonPropertyName("Cronograma Executado")]
        public int cronogramas_executados { get; set; }
        [JsonPropertyName("Suspensa")]
        public int suspensas { get; set; }
        [JsonPropertyName("Canceladas")]
        public int canceladas { get; set; }
        [JsonPropertyName("Desertas")]
        public int desertas { get; set; }
        [JsonPropertyName("Exitosas")]
        public int exitosas { get; private set; }
        [JsonPropertyName("Parcialmente Exitosas")]
        public int parcialmente_exitosas { get; private set; }

        #endregion [ FIM - Propriedades ]
    }

    public class PublicCallReportCooperativeModel
    {
        public PublicCallReportCooperativeModel(PublicCallReportCooperative report)
        {
            if (report is null)
                return;

            this.numero = report.numero;
            this.cooperativas_inscritas = report.cooperativas_inscritas;
            this.cooperativas_habilitadas = report.cooperativas_habilitadas;
            this.cooperativas_inabilitadas = report.cooperativas_inabilitadas;
        }

        #region [ Propriedades ]

        [JsonPropertyName("Chamada")]
        public string numero { get; set; }
        [JsonPropertyName("Cooperativas Inscritas")]
        public int cooperativas_inscritas { get; set; }
        [JsonPropertyName("Cooperativas Habilitadas")]
        public int cooperativas_habilitadas { get; set; }
        [JsonPropertyName("Cooperativas Inabilitadas")]
        public int cooperativas_inabilitadas { get; set; }

        #endregion [ FIM - Propriedades ]
    }
}
