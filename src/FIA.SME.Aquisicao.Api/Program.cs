using FIA.SME.Aquisicao.Api.Setup;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerConfigServices();

builder.Services.AddGeneralConfigServices(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthorizationConfigServices(builder.Configuration);
builder.Services.AddDependencyInjectionConfig(builder.Configuration);




var app = builder.Build();

app.UseSwaggerConfigure();

app.UseGeneralConfigure(builder.Configuration);

app.UseAuthorizationConfigure();

app.MapControllers();

app.Run();
