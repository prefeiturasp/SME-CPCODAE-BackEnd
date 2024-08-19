using FIA.SME.Aquisicao.Core.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace FIA.SME.Aquisicao.Core.Setup
{
    public static class DependencyInjectionCore
    {
        public static void AddDependencyInjectionCoreServices(this IServiceCollection services)
        {
            services.AddSingleton<IPasswordHasher, PasswordHasher>();
            services.AddSingleton<ITextEncryptor, AesTextEncryptor>();
        }
    }
}
