using ExcelDataReader;
using FIA.SME.Aquisicao.Infrastructure.Models;

namespace FIA.SME.Aquisicao.Infrastructure.Components
{
    public interface IExcelParserComponent
    {
        Task<List<DapCafExtract>> ParseDapCafExtract(string fileBase64);
    }

    internal class ExcelParserComponent : IExcelParserComponent
    {
        public async Task<List<DapCafExtract>> ParseDapCafExtract(string fileBase64)
        {
            var result = new List<DapCafExtract>();

            using (var ms = new MemoryStream(Convert.FromBase64String(fileBase64)))
            {
                using (var reader = ExcelReaderFactory.CreateReader(ms))
                {
                    do
                    {
                        while (reader.Read())
                        {
                            // reader.GetDouble(0);
                        }
                    } while (reader.NextResult());
                }
            }

            return result;
        }
    }
}
