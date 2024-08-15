using FIA.SME.Aquisicao.Infrastructure.Components;
using FIA.SME.Aquisicao.Infrastructure.Integrations;
using FIA.SME.Aquisicao.Infrastructure.Interfaces;
using FIA.SME.Aquisicao.Infrastructure.Repositories;
using FIA.SME.Aquisicao.Infrastructure.Repositories.Context;
using FIA.SME.Aquisicao.Infrastructure.Storages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FIA.SME.Aquisicao.Application.Setup
{
    public static class DependencyInjectionRepositories
    {
        public static void AddDependencyInjectionApplicationRepositories(this IServiceCollection services, IConfiguration configuration)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            services.AddDbContext<SMEContext>(options => options.UseNpgsql(configuration.GetConnectionString("FIASMEAquisicao"),
                builder => { builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null); }), ServiceLifetime.Scoped, ServiceLifetime.Transient);

            services.AddScoped<ICsvParserComponent, CsvParserComponent>();
            services.AddScoped<IExcelParserComponent, ExcelParserComponent>();
            services.AddScoped<IMailComponent, MailComponent>();

            services.AddScoped<IAddressRepository, AddressRepository>();
            services.AddScoped<IBankRepository, BankRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IConfigurationRepository, ConfigurationRepository>();
            services.AddScoped<IChangeRequestRepository, ChangeRequestRepository>();
            services.AddScoped<ICooperativeRepository, CooperativeRepository>();
            services.AddScoped<ICooperativeDocumentRepository, CooperativeDocumentRepository>();
            services.AddScoped<ICooperativeLegalRepresentativeRepository, CooperativeLegalRepresentativeRepository>();
            services.AddScoped<ICooperativeMemberRepository, CooperativeMemberRepository>();
            services.AddScoped<IDocumentTypeRepository, DocumentTypeRepository>();
            services.AddScoped<IFaleConoscoRepository, FaleConoscoRepository>();
            services.AddScoped<IFoodRepository, FoodRepository>();
            services.AddScoped<ILocationRegionRepository, LocationRegionRepository>();
            services.AddScoped<IPublicCallRepository, PublicCallRepository>();
            services.AddScoped<IPublicCallAnswerRepository, PublicCallAnswerRepository>();
            services.AddScoped<IPublicCallAnswerMemberRepository, PublicCallAnswerMemberRepository>();
            services.AddScoped<IPublicCallDeliveryRepository, PublicCallDeliveryRepository>();
            services.AddScoped<IPublicCallDocumentRepository, PublicCallDocumentRepository>();
            services.AddScoped<IPublicCallFoodRepository, PublicCallFoodRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            services.AddScoped<IIBGEIntegration, IBGEIntegration>();

            services.AddScoped<ISMEStorage, LocalStorage>();
        }
    }
}
