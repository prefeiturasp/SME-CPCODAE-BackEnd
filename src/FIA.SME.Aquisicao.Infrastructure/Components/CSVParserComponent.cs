using CsvHelper;
using CsvHelper.Configuration;
using FIA.SME.Aquisicao.Infrastructure.Models;
using System.Globalization;

namespace FIA.SME.Aquisicao.Infrastructure.Components
{
    public interface ICsvParserComponent
    {
        Task<List<DapCafExtract>> ParseDapCafExtract(string csvFileBase64);
    }

    internal class CsvParserComponent : ICsvParserComponent
    {
        public async Task<List<DapCafExtract>> ParseDapCafExtract(string csvFileBase64)
        {
            var splittedValue = csvFileBase64.Split(',');
            var fileBase64 = splittedValue.Length > 1 ? splittedValue[1] : csvFileBase64;

            var config = new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = ";" };
            CsvReader csv;
            using (var ms = new MemoryStream(Convert.FromBase64String(fileBase64)))
            {
                using (var textReader2 = new StreamReader(ms))
                {
                    csv = new CsvReader(textReader2, config);

                    var records = csv.GetRecords<DapCafExtract>().ToList();

                    return records;
                }
            }
        }
    }
}
