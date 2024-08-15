using FIA.SME.Aquisicao.Infrastructure.Models;
using Flurl.Http;

namespace FIA.SME.Aquisicao.Infrastructure.Integrations
{
    public interface IIBGEIntegration
    {
        Task<List<IBGEDistrict.IBGEDistrictCity>> GetAllCities();
        Task<List<IBGEDistrict>> GetAllCitiesByStateId(int stateId);
    }

    internal class IBGEIntegration : IIBGEIntegration
    {
        public async Task<List<IBGEDistrict.IBGEDistrictCity>> GetAllCities()
        {
            try
            {
                var url = $"https://servicodados.ibge.gov.br/api/v1/localidades/municipios";
                var response = await url.GetJsonAsync<List<IBGEDistrict.IBGEDistrictCity>>();

                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<IBGEDistrict>> GetAllCitiesByStateId(int stateId)
        {
            try
            {
                var url = $"https://servicodados.ibge.gov.br/api/v1/localidades/estados/{stateId}/distritos";
                var response = await url.GetJsonAsync<List<IBGEDistrict>>();

                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
