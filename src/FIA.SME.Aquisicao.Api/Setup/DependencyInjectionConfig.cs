using FIA.SME.Aquisicao.Application.Setup;
using FIA.SME.Aquisicao.Core.Setup;

namespace FIA.SME.Aquisicao.Api.Setup
{
    public static class DependencyInjectionConfig
    {
        public static void AddDependencyInjectionConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDependencyInjectionCoreServices();
            services.AddDependencyInjectionApplicationServices(configuration);
        }
    }
}
