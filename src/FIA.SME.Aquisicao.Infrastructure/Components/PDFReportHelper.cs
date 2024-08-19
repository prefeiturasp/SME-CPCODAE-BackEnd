using Azure;
using FIA.SME.Aquisicao.Core.Enums;
using FIA.SME.Aquisicao.Core.Helpers;
using FIA.SME.Aquisicao.Infrastructure.Models;
using FIA.SME.Aquisicao.Infrastructure.Repositories.Types;
using Org.BouncyCastle.Asn1.Pkcs;
using PdfSharpCore;
using PdfSharpCore.Drawing;
using PdfSharpCore.Drawing.Layout;
using PdfSharpCore.Pdf;
using System.Diagnostics.Metrics;

namespace FIA.SME.Aquisicao.Infrastructure.Components
{
    public static class PDFReportHelperComponent
    {
        private static double _marginLeft = 40;
        private static XFont _fontRegular = new XFont("Verdana", 10, XFontStyle.Regular);
        private static XFont _fontRegularTrTable = new XFont("Verdana", 6, XFontStyle.Regular);
        private static XFont _fontBold = new XFont("Verdana", 8, XFontStyle.Bold);

        public static double AddAssinatura(this PdfDocument document, ref XGraphics gfx, ref double positionHeight, PublicCallAnswerReport report)
        {
            document.AddLine(ref gfx, ref positionHeight);
            document.AddLine(ref gfx, ref positionHeight);

            XTextFormatter tf = new XTextFormatter(gfx);
            var page = gfx.PdfPage;

            tf.DrawString("São Paulo, " + DateTime.Now.ToString("dd 'de' MMMM 'de' yyyy"), _fontRegular, XBrushes.Black, new XRect(_marginLeft + 5, positionHeight, page.Width, page.Height), XStringFormats.TopLeft); // Id
            document.AddLine(ref gfx, ref positionHeight);
            document.AddLine(ref gfx, ref positionHeight);
            document.AddLine(ref gfx, ref positionHeight);

            gfx.DrawLine(XPens.Black, _marginLeft, positionHeight, 200, positionHeight);
            gfx.DrawString($"Nome: {report.cooperative.legal_representative.name ?? "-"}", _fontRegular, XBrushes.Black, new XRect(_marginLeft, positionHeight + 10, page.Width, page.Height), XStringFormats.TopLeft);
            document.AddLine(ref gfx, ref positionHeight);
            gfx.DrawString($"CPF: {report.cooperative.legal_representative.cpf.FormatCPF() ?? "-"}", _fontRegular, XBrushes.Black, new XRect(_marginLeft, positionHeight + 10, page.Width, page.Height), XStringFormats.TopLeft);
            document.AddLine(ref gfx, ref positionHeight);
            gfx.DrawString($"Cargo: {report.cooperative.legal_representative?.position ?? "-"}", _fontRegular, XBrushes.Black, new XRect(_marginLeft, positionHeight + 10, page.Width, page.Height), XStringFormats.TopLeft);
            document.AddLine(ref gfx, ref positionHeight);

            return positionHeight;
        }

        public static double AddDeclaracao(this PdfDocument document, ref XGraphics gfx, ref double positionHeight)
        {
            document.AddLine(ref gfx, ref positionHeight);

            XTextFormatter tf = new XTextFormatter(gfx);
            var page = gfx.PdfPage;

            tf.DrawString("Percentual para o qual a proposta é valida e outras informações", _fontBold, XBrushes.Black, new XRect(_marginLeft + 5, positionHeight, page.Width, page.Height), XStringFormats.TopLeft); // Id
            document.AddLine(ref gfx, ref positionHeight);
             
            tf.DrawString("Esta proposta de fornecimento de alimentos é válida, com os mesmos preços acima indicados, independente do quantitativo a ser contratado.\n\nOs preços ofertados incluem todos os custos diretos e indiretos, incluídas as despesas de frete até os locais de entrega, os encargos sociais e trabalhistas, fiscais e demais despesas necessárias ao cumprimento integral das obrigações decorrentes da licitação.\n\nO proponente se submete a todas as cláusulas e condições do Edital, bem como às disposições da Lei Federal nº 14.133/21, da Lei Municipal nº 13.278/02 e demais normas complementares.", _fontRegular, XBrushes.Black, new XRect(_marginLeft, positionHeight, page.Width - 100, page.Height), XStringFormats.TopLeft);
            document.AddLine(ref gfx, ref positionHeight);
            document.AddLine(ref gfx, ref positionHeight);
            document.AddLine(ref gfx, ref positionHeight);
            document.AddLine(ref gfx, ref positionHeight);

            return positionHeight;
        }

        public static double AddHeader(this PdfDocument document, ref XGraphics gfx, double positionHeight, PublicCallAnswerReport report)
        {
            document.AddLine(ref gfx, ref positionHeight);
            document.AddLine(ref gfx, ref positionHeight);

            var page = gfx.PdfPage;

            gfx.DrawString($"Nome da Organização: {report.cooperative.name ?? "-"}", _fontRegular, XBrushes.Black, new XRect(0, positionHeight, page.Width, page.Height), XStringFormats.TopCenter);
            document.AddLine(ref gfx, ref positionHeight);
            gfx.DrawString($"Endereço: {report.cooperative.address.ToString(report.cooperative_city)}", _fontRegular, XBrushes.Black, new XRect(0, positionHeight, page.Width, page.Height), XStringFormats.TopCenter);
            document.AddLine(ref gfx, ref positionHeight);
            gfx.DrawString($"CNPJ: {report.cooperative.cnpj?.FormatCNPJ() ?? "-"}", _fontRegular, XBrushes.Black, new XRect(0, positionHeight, page.Width, page.Height), XStringFormats.TopCenter);
            document.AddLine(ref gfx, ref positionHeight);
            document.AddLine(ref gfx, ref positionHeight);

            gfx.DrawString($"PROJETO DE VENDA DE GÊNEROS ALIMENTÍCIOS DA AGRICULTURA FAMILIAR", _fontRegular, XBrushes.Black, new XRect(0, positionHeight, page.Width, page.Height), XStringFormats.TopCenter);
            document.AddLine(ref gfx, ref positionHeight);
            gfx.DrawString($"Chamada Pública nº {report.public_call_number}", _fontBold, XBrushes.Black, new XRect(0, positionHeight, page.Width, page.Height), XStringFormats.TopCenter);

            return positionHeight;
        }

        public static double AddHeaderText(this PdfDocument document, ref XGraphics gfx, ref double positionHeight, PublicCallAnswerReport report)
        {
            document.AddLine(ref gfx, ref positionHeight);
            document.AddLine(ref gfx, ref positionHeight);

            XTextFormatter tf = new XTextFormatter(gfx);
            var page = gfx.PdfPage;

            tf.DrawString($"{ report.cooperative.name ?? "-"}, com domicílio à { report.cooperative.address.ToString(report.cooperative_city)}, CNPJ nº { report.cooperative.cnpj?.FormatCNPJ() ?? "-"}, telefone { report.cooperative.formatted_phone}, e-mail { report.cooperative.email }, neste ato representado por { report.cooperative.legal_representative.name }, { report.cooperative.legal_representative.marital_status }, pelo presente, propõe o fornecimento do produto { report.food.food.name }, conforme as características descritas no Anexo I do Edital, inclusive no que diz respeito a embalagem, rotulagem e entrega, conforme as seguintes condições:", _fontRegular, XBrushes.Black, new XRect(_marginLeft, positionHeight, page.Width - 100, page.Height), XStringFormats.TopLeft);

            document.AddLine(ref gfx, ref positionHeight);
            document.AddLine(ref gfx, ref positionHeight);
            document.AddLine(ref gfx, ref positionHeight);
            document.AddLine(ref gfx, ref positionHeight);

            return positionHeight;
        }

        public static double AddHrLine(this PdfDocument document, ref XGraphics gfx, ref double positionHeight)
        {
            var page = gfx.PdfPage;
            var lineWidth = page.Width - 100;
            var center = (page.Width / 2) - (lineWidth / 2);
            var pen = new XPen(XColors.Black, 0.8);
            gfx.DrawLine(pen, center, positionHeight, lineWidth, positionHeight);

            return positionHeight;
        }

        public static double AddHrLineTable(this PdfDocument document, ref XGraphics gfx, ref double positionHeight)
        {
            var page = gfx.PdfPage;
            var lineWidth = page.Width - 50;
            var center = (page.Width / 2) - (lineWidth / 2);
            var pen = new XPen(XColors.Black, 0.8);
            gfx.DrawLine(pen, center, positionHeight, lineWidth, positionHeight);

            return positionHeight;
        }

        public static double AddHrLineTableVertical(this PdfDocument document, ref XGraphics gfx, ref double positionHeight, double x)
        {
            var pen = new XPen(XColors.Black, 0.8);
            gfx.DrawLine(pen, x, positionHeight - 10, x, positionHeight + 20);

            return positionHeight;
        }

        public static double AddIdentificacaoEntidadeExecutora(this PdfDocument document, ref XGraphics gfx, ref double positionHeight, PublicCallAnswerReport report)
        {
            var tf = new XTextFormatter(gfx);
            var page = gfx.PdfPage;

            tf.DrawString("Identificação da entidade executora do PNAE/FNDE/MEC", _fontBold, XBrushes.Black, new XRect(_marginLeft + 5, positionHeight, page.Width, page.Height), XStringFormats.TopLeft); // Id
            document.AddLine(ref gfx, ref positionHeight);
            document.AddHrLineTable(ref gfx, ref positionHeight);

            document.AddLineContentTable(ref gfx, ref positionHeight, "1 - Nome da Entidade:", "Secretaria Municipal de Educação");
            document.AddLineContentTable(ref gfx, ref positionHeight, "2 - CNPJ", "46.392.114/0001-25");
            document.AddLineContentTable(ref gfx, ref positionHeight, "3 - Município", "São Paulo");
            document.AddLineContentTable(ref gfx, ref positionHeight, "4 - Endereço", "Rua Borges Lagoa, 1230 – Vila Clementino");
            document.AddLineContentTable(ref gfx, ref positionHeight, "5 - DDD/Fone", "(11) 3396-0174");

            return positionHeight;
        }

        public static double AddIdentificacaoFornecedores(this PdfDocument document, ref XGraphics gfx, ref double positionHeight, PublicCallAnswerReport report)
        {
            var tf = new XTextFormatter(gfx);
            var page = gfx.PdfPage;

            tf.DrawString("Identificação dos fornecedores", _fontBold, XBrushes.Black, new XRect(_marginLeft + 5, positionHeight, page.Width, page.Height), XStringFormats.TopLeft); // Id
            document.AddLine(ref gfx, ref positionHeight);
            document.AddHrLineTable(ref gfx, ref positionHeight);

            document.AddLineContentTable(ref gfx, ref positionHeight, "Nome do Proponente", report.cooperative.name ?? "-");
            document.AddLineContentTable(ref gfx, ref positionHeight, "CNPJ", report.cooperative.cnpj?.FormatCNPJ() ?? "-");
            document.AddLineContentTable(ref gfx, ref positionHeight, "Endereço", report.cooperative.address.ToString(report.cooperative_city));
            document.AddLineContentTable(ref gfx, ref positionHeight, "Município/UF", $"{report.cooperative_city.nome}/{ report.cooperative_city.microrregiao.mesorregiao.UF.sigla.ToUpper()}");
            document.AddLineContentTable(ref gfx, ref positionHeight, "E-mail", report.cooperative.email ?? "-");
            document.AddLineContentTable(ref gfx, ref positionHeight, "DDD/Fone", report.cooperative.formatted_phone);
            document.AddLineContentTable(ref gfx, ref positionHeight, "CEP", report.cooperative.address.cep);
            document.AddLineContentTable(ref gfx, ref positionHeight, "Nº DAP Jurídica", report.cooperative.dap_caf_code);

            if (report.cooperative.bank is not null)
            {
                document.AddLineContentTable(ref gfx, ref positionHeight, "Banco (conforme Decreto nº 51.197/2010)", report.cooperative.bank.name);
                document.AddLineContentTable(ref gfx, ref positionHeight, "Nº da agência", report.cooperative.bank.agency);
                document.AddLineContentTable(ref gfx, ref positionHeight, "Nº da Conta Corrente", report.cooperative.bank.account_number);
            }
            
            document.AddLineContentTable(ref gfx, ref positionHeight, "Nº de Associados", report.total_members.ToString());
            document.AddLineContentTable(ref gfx, ref positionHeight, "Nº de Associados de acordo com a Lei 11.326/2006", report.total_members_dap_fisica.ToString());
            document.AddLineContentTable(ref gfx, ref positionHeight, "Número de Associados com DAP/CAF", report.total_members_dap_fisica.ToString());
            document.AddLineContentTable(ref gfx, ref positionHeight, "Nome do representante legal", report.cooperative.legal_representative.name);
            document.AddLineContentTable(ref gfx, ref positionHeight, "CPF", report.cooperative.legal_representative.cpf.FormatCPF());
            document.AddLineContentTable(ref gfx, ref positionHeight, "DDD/Fone", report.cooperative.legal_representative.phone.FormatPhoneNumber());
            document.AddLineContentTable(ref gfx, ref positionHeight, "Endereço", report.cooperative.legal_representative.address.ToString(report.legal_representative_city));
            document.AddLineContentTable(ref gfx, ref positionHeight, "Município/UF", $"{report.legal_representative_city.nome}/{report.legal_representative_city.microrregiao.mesorregiao.UF.sigla.ToUpper()}");

            document.AddLine(ref gfx, ref positionHeight);

            return positionHeight;
        }

        public static double AddListaAssociados(this PdfDocument document, ref XGraphics gfx, ref double positionHeight, PublicCallAnswerReport report)
        {
            var tf = new XTextFormatter(gfx);
            var page = gfx.PdfPage;

            string[] headers = new[] { "ID", "Nome", "CPF", "DAP/CAF", "Produto", "Qt", "Un", "Valor Un", "Valor Total" };

            document.AddLine(ref gfx, ref positionHeight);

            var defaultWidth = 60;
            var defaultMediumWidth = 90;
            var defaultLargeWidth = 150;
            var currentStartX = 0;

            tf.DrawString("Relação de DAP's físicas relacionadas no projeto de venda", _fontBold, XBrushes.Black, new XRect(_marginLeft + 5, positionHeight, page.Width, page.Height), XStringFormats.TopLeft); // Id
            document.AddLine(ref gfx, ref positionHeight);

            // Desenha cabeçalhos
            tf.DrawString(headers[0], _fontBold, XBrushes.Black, new XRect(_marginLeft, positionHeight, page.Width, page.Height), XStringFormats.TopLeft); // Id
            currentStartX += (defaultWidth / 4);
            tf.DrawString(headers[1], _fontBold, XBrushes.Black, new XRect(_marginLeft + currentStartX, positionHeight, page.Width, page.Height), XStringFormats.TopLeft); // Nome
            currentStartX += defaultLargeWidth;
            tf.DrawString(headers[2], _fontBold, XBrushes.Black, new XRect(_marginLeft + currentStartX, positionHeight, page.Width, page.Height), XStringFormats.TopLeft); // CPF
            currentStartX += defaultMediumWidth;
            tf.DrawString(headers[3], _fontBold, XBrushes.Black, new XRect(_marginLeft + currentStartX, positionHeight, page.Width, page.Height), XStringFormats.TopLeft); // DAP/CAF
            currentStartX += defaultLargeWidth;
            tf.DrawString(headers[4], _fontBold, XBrushes.Black, new XRect(_marginLeft + currentStartX, positionHeight, page.Width, page.Height), XStringFormats.TopLeft); // Produto
            currentStartX += defaultLargeWidth;
            tf.DrawString(headers[5], _fontBold, XBrushes.Black, new XRect(_marginLeft + currentStartX, positionHeight, page.Width, page.Height), XStringFormats.TopLeft); // Qt
            currentStartX += defaultWidth;
            tf.DrawString(headers[6], _fontBold, XBrushes.Black, new XRect(_marginLeft + currentStartX, positionHeight, page.Width, page.Height), XStringFormats.TopLeft); // Un
            currentStartX += defaultWidth;
            tf.DrawString(headers[7], _fontBold, XBrushes.Black, new XRect(_marginLeft + currentStartX, positionHeight, page.Width, page.Height), XStringFormats.TopLeft); // Valor Un
            currentStartX += defaultWidth;
            tf.DrawString(headers[8], _fontBold, XBrushes.Black, new XRect(_marginLeft + currentStartX, positionHeight, page.Width, page.Height), XStringFormats.TopLeft); // Valor Total

            document.AddLine(ref gfx, ref positionHeight);

            var cooperados = report.public_call_members;
            var produto = report.food;

            for (int i = 0; i < cooperados.Count; i++)
            {
                var cooperado = cooperados[i];
                tf = new XTextFormatter(gfx);
                currentStartX = 0;

                tf.DrawString((i+1).ToString(), _fontRegularTrTable, XBrushes.Black, new XRect(_marginLeft + currentStartX, positionHeight, page.Width, page.Height), XStringFormats.TopLeft); // Id
                currentStartX += (defaultWidth / 4);
                tf.DrawString(cooperado.member.name ?? "-", _fontRegularTrTable, XBrushes.Black, new XRect(_marginLeft + currentStartX, positionHeight, page.Width, page.Height), XStringFormats.TopLeft); // Nome
                currentStartX += defaultLargeWidth;
                tf.DrawString(cooperado.member.cpf?.FormatCPF() ?? "-", _fontRegularTrTable, XBrushes.Black, new XRect(_marginLeft + currentStartX, positionHeight, page.Width, page.Height), XStringFormats.TopLeft); // CPF
                currentStartX += defaultMediumWidth;
                tf.DrawString(cooperado.member.dap_caf_code, _fontRegularTrTable, XBrushes.Black, new XRect(_marginLeft + currentStartX, positionHeight, page.Width, page.Height), XStringFormats.TopLeft); // DAP/CAF
                currentStartX += defaultLargeWidth;
                tf.DrawString(produto.food.name, _fontRegularTrTable, XBrushes.Black, new XRect(_marginLeft + currentStartX, positionHeight, page.Width, page.Height), XStringFormats.TopLeft); // Produto
                currentStartX += defaultLargeWidth;
                tf.DrawString(String.Format("{0:N2}", produto.quantity), _fontRegularTrTable, XBrushes.Black, new XRect(_marginLeft + currentStartX, positionHeight, page.Width, page.Height), XStringFormats.TopLeft); // Qt
                currentStartX += defaultWidth;
                tf.DrawString(((MeasureUnitEnum)produto.food.measure_unit).DescriptionAttr(), _fontRegularTrTable, XBrushes.Black, new XRect(_marginLeft + currentStartX, positionHeight, page.Width, page.Height), XStringFormats.TopLeft); // Un
                currentStartX += defaultWidth;
                tf.DrawString(String.Format("{0:C2}", produto.price), _fontRegularTrTable, XBrushes.Black, new XRect(_marginLeft + currentStartX, positionHeight, page.Width, page.Height), XStringFormats.TopLeft); // Valor Un
                currentStartX += defaultWidth;
                tf.DrawString(String.Format("{0:C2}", produto.quantity * produto.price), _fontRegularTrTable, XBrushes.Black, new XRect(_marginLeft + currentStartX, positionHeight, page.Width, page.Height), XStringFormats.TopLeft); // Valor Total

                document.AddLine(ref gfx, ref positionHeight);
            }

            return positionHeight;
        }

        public static double AddListaProdutos(this PdfDocument document, ref XGraphics gfx, ref double positionHeight, PublicCallAnswerReport report)
        {
            var page = gfx.PdfPage;

            string[] headers = new[] { "Produto", "Qt", "Un", "Valor Un", "Valor Total" };
            
            document.AddLine(ref gfx, ref positionHeight);

            var defaultWidth = 60;
            var currentStartX = 200;

            var tf = new XTextFormatter(gfx);

            tf.DrawString("Relação de Produto", _fontBold, XBrushes.Black, new XRect(_marginLeft + 5, positionHeight, page.Width, page.Height), XStringFormats.TopLeft); // Id
            document.AddLine(ref gfx, ref positionHeight);

            // Desenha cabeçalhos
            tf.DrawString(headers[0], _fontBold, XBrushes.Black, new XRect(_marginLeft, positionHeight, page.Width, page.Height), XStringFormats.TopLeft);
            tf.DrawString(headers[1], _fontBold, XBrushes.Black, new XRect(_marginLeft + currentStartX, positionHeight, page.Width, page.Height), XStringFormats.TopLeft);
            currentStartX += defaultWidth;
            tf.DrawString(headers[2], _fontBold, XBrushes.Black, new XRect(_marginLeft + currentStartX, positionHeight, page.Width, page.Height), XStringFormats.TopLeft);
            currentStartX += defaultWidth;
            tf.DrawString(headers[3], _fontBold, XBrushes.Black, new XRect(_marginLeft + currentStartX, positionHeight, page.Width, page.Height), XStringFormats.TopLeft);
            currentStartX += defaultWidth;
            tf.DrawString(headers[4], _fontBold, XBrushes.Black, new XRect(_marginLeft + currentStartX, positionHeight, page.Width, page.Height), XStringFormats.TopLeft);

            document.AddLine(ref gfx, ref positionHeight);

            var produto = report.food;
            tf = new XTextFormatter(gfx);
            currentStartX = 200;

            tf.DrawString(produto.food.name, _fontRegularTrTable, XBrushes.Black, new XRect(_marginLeft, positionHeight, page.Width, page.Height), XStringFormats.TopLeft);
            tf.DrawString(String.Format("{0:N2}", produto.quantity), _fontRegularTrTable, XBrushes.Black, new XRect(_marginLeft + currentStartX, positionHeight, page.Width, page.Height), XStringFormats.TopLeft);
            currentStartX += defaultWidth;
            tf.DrawString(produto.measure_unit.DescriptionAttr(), _fontRegularTrTable, XBrushes.Black, new XRect(_marginLeft + currentStartX, positionHeight, page.Width, page.Height), XStringFormats.TopLeft);
            currentStartX += defaultWidth;
            tf.DrawString(String.Format("{0:C2}", produto.price), _fontRegularTrTable, XBrushes.Black, new XRect(_marginLeft + currentStartX, positionHeight, page.Width, page.Height), XStringFormats.TopLeft);
            currentStartX += defaultWidth;
            tf.DrawString(String.Format("{0:C2}", produto.quantity * produto.price), _fontRegularTrTable, XBrushes.Black, new XRect(_marginLeft + currentStartX, positionHeight, page.Width, page.Height), XStringFormats.TopLeft);

            document.AddLine(ref gfx, ref positionHeight);

            document.AddLine(ref gfx, ref positionHeight);

            return positionHeight;
        }

        public static void AddLine(this PdfDocument document, ref XGraphics gfx, ref double positionHeight, bool? isTable = false)
        {
            var lineHeight = 20;
            XSize size = PageSizeConverter.ToSize(PageSize.A4);
            var hasToAddPage = positionHeight > (size.Width - 100);

            if (!hasToAddPage)
            {
                positionHeight += lineHeight;
                return;
            }

            if (isTable.HasValue && isTable.Value == true)
            {
                positionHeight += lineHeight;
                document.AddHrLineTable(ref gfx, ref positionHeight);
            }

            PdfPage page = document.AddPage();
            page.Orientation = PageOrientation.Landscape;
            gfx = XGraphics.FromPdfPage(page);

            positionHeight = 40;
        }

        public static void AddLineTable(this PdfDocument document, ref XGraphics gfx, ref double positionHeight)
        {
            var lineHeight = 10;
            XSize size = PageSizeConverter.ToSize(PageSize.A4);
            var hasToAddPage = positionHeight > (size.Width - 20);

            if (!hasToAddPage)
            {
                positionHeight += lineHeight;
                return;
            }

            PdfPage page = document.AddPage();
            page.Orientation = PageOrientation.Landscape;
            gfx = XGraphics.FromPdfPage(page);

            positionHeight = 20;
        }

        public static void AddLineContentTable(this PdfDocument document, ref XGraphics gfx, ref double positionHeight, string label, string content)
        {
            var tf = new XTextFormatter(gfx);
            var page = gfx.PdfPage;

            var defaultWidth = 570;
            var currentStartX = 0;

            var tableEnd = page.Width - 50;
            var tableStart = (page.Width / 2) - (tableEnd / 2);

            document.AddLineTable(ref gfx, ref positionHeight);
            document.AddHrLineTableVertical(ref gfx, ref positionHeight, tableStart);
            tf.DrawString(label, _fontBold, XBrushes.Black, new XRect(_marginLeft + currentStartX, positionHeight, page.Width, page.Height), XStringFormats.TopLeft); // Id
            currentStartX += (defaultWidth / 2);
            document.AddHrLineTableVertical(ref gfx, ref positionHeight, currentStartX);
            tf.DrawString(content, _fontRegular, XBrushes.Black, new XRect(5 + currentStartX, positionHeight, page.Width, page.Height), XStringFormats.TopLeft); // CPF
            document.AddHrLineTableVertical(ref gfx, ref positionHeight, tableEnd);
            document.AddLine(ref gfx, ref positionHeight, true);
            document.AddHrLineTable(ref gfx, ref positionHeight);
        }

        public static double AddLogo(this XGraphics gfx, PublicCallAnswerReport report)
        {
            var page = gfx.PdfPage;

            var base64Logo = report.cooperative.logo ?? "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";
            byte[] imageBytes = Convert.FromBase64String(base64Logo);

            XImage logo = XImage.FromStream(() => new MemoryStream(imageBytes));
            var logoHeight = logo.PixelHeight * 0.75;
            var logoWidth = logo.PixelWidth * 0.75;
            var center = (page.Width / 2) - (logoWidth / 2);
            gfx.DrawImage(logo, center, 20, logoWidth, logoHeight);

            return logoHeight;
        }
    }
}
