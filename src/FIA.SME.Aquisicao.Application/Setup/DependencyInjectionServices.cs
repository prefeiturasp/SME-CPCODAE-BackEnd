using FIA.SME.Aquisicao.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FIA.SME.Aquisicao.Application.Setup
{
    public static class DependencyInjectionServices
    {
        public static void AddDependencyInjectionApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IAuthorizationSMEService, AuthorizationSMEService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IChangeRequestService, ChangeRequestService>();
            services.AddScoped<ICooperativeMemberService, CooperativeMemberService>();
            services.AddScoped<ICooperativeService, CooperativeService>();
            services.AddScoped<ICooperativeDocumentService, CooperativeDocumentService>();
            services.AddScoped<IDocumentTypeService, DocumentTypeService>();
            services.AddScoped<IFaleConoscoService, FaleConoscoService>();
            services.AddScoped<IFoodService, FoodService>();
            services.AddScoped<IMailService, MailService>();
            services.AddScoped<IPublicCallService, PublicCallService>();
            services.AddScoped<IPublicCallAnswerService, PublicCallAnswerService>();
            services.AddScoped<IPublicCallDeliveryService, PublicCallDeliveryService>();
            services.AddScoped<IPublicCallDocumentService, PublicCallDocumentService>();
            services.AddScoped<IStorageService, StorageService>();
            services.AddScoped<IUserService, UserService>();

            services.AddDependencyInjectionApplicationRepositories(configuration);
        }
    }
}
