using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;

namespace FIA.SME.Aquisicao.Infrastructure.Components
{
    public static class PDFHelperComponent
    {
        private static void AddHeader(PdfPage page, string headerText)
        {
            XFont font = new XFont("Helvetica", 8);
            XRect layoutRectangleString = new XRect(0/*X*/, 5/*Y*/, page.Width/*Width*/, font.Height/*Height*/);
            XRect layoutRectangleBackground = new XRect(0/*X*/, 0/*Y*/, page.Width/*Width*/, font.Height + 10/*Height*/);

            using (XGraphics gfx = XGraphics.FromPdfPage(page))
            {
                gfx.DrawRectangle(XBrushes.White, layoutRectangleBackground);

                gfx.DrawString(
                    $" ------ {headerText} -----------",
                    font,
                    XBrushes.Black,
                    layoutRectangleString,
                    XStringFormats.Center);
            }
        }

        private static List<byte[]> AddHeaders(List<(string, byte[])> originalPdfBytes)
        {
            var lstHeaderedPdfBytes = new List<byte[]>();

            foreach (var pdf in originalPdfBytes)
            {
                var msPdf = new MemoryStream(pdf.Item2);
                var pdfDoc = PdfReader.Open(msPdf, PdfDocumentOpenMode.Modify);

                foreach (var page in pdfDoc.Pages)
                    AddHeader(page, pdf.Item1);

                MemoryStream stream = new MemoryStream();
                pdfDoc.Save(stream, false);
                lstHeaderedPdfBytes.Add(stream.ToArray());
            }

            return lstHeaderedPdfBytes;
        }

        public static byte[] MergePdf(List<(string, byte[])> originalPdfBytes)
        {
            var lstHeaderedPdfBytes = AddHeaders(originalPdfBytes);
            var lstDocuments = new List<PdfDocument>();

            foreach (var pdf in lstHeaderedPdfBytes)
            {
                var msPdf = new MemoryStream(pdf);
                var pdfDoc = PdfReader.Open(msPdf, PdfDocumentOpenMode.Import);

                lstDocuments.Add(pdfDoc);
            }

            using (PdfDocument outPdf = new PdfDocument())
            {
                for (int i = 1; i <= lstDocuments.Count; i++)
                {
                    foreach (PdfPage page in lstDocuments[i - 1].Pages)
                    {
                        outPdf.AddPage(page);
                    }
                }

                MemoryStream stream = new MemoryStream();
                outPdf.Save(stream, false);
                byte[] bytes = stream.ToArray();

                return bytes;
            }
        }
    }
}
