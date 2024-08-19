using FIA.SME.Aquisicao.Infrastructure.Models;

namespace FIA.SME.Aquisicao.Api.Models
{
    public class ConfigurationMaximumYearSuppliedValueResponseModel
    {
        public ConfigurationMaximumYearSuppliedValueResponseModel(decimal maximumYearSuppliedValue)
        {
            this.maximumYearSuppliedValue = maximumYearSuppliedValue;
        }

        public decimal maximumYearSuppliedValue { get; set; }
    }

    public class ConfigurationResponse
    {
        public ConfigurationResponse(Configuration configuration)
        {
            if (configuration is null)
                return;

            this.id = configuration.id;
            this.name = configuration.name;
            this.value = configuration.value;
            this.is_active = configuration.is_active;
        }

        public Guid id { get; set; }
        public string name { get; set; }
        public string value { get; set; }
        public bool is_active { get; set; }
    }

    public class ConfigurationUpdate
    {
        public Guid id { get; set; }
        public string name { get; set; }
        public string value { get; set; }
    }
}
