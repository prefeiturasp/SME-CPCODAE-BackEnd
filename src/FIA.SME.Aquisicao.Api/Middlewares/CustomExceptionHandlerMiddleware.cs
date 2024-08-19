using FIA.SME.Aquisicao.Core.Domain;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net;

namespace FIA.SME.Aquisicao.Api.Middlewares
{
    /// <summary>
    /// Middleware para fazer o tratamento de erros HTTP ou de exceptions e padronizar o retorno.
    /// </summary>
    public class CustomExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly ILogger _logger;

        public CustomExceptionHandlerMiddleware(RequestDelegate next, ILogger<CustomExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var jsonSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };

            try
            {
                await _next.Invoke(context);

                return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.GetBaseException().Message);

                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                await context.Response.WriteAsync(JsonConvert.SerializeObject(new InternalServerErrorApiResponse(ex), jsonSettings));
            }
        }
    }

    public static class CustomExceptionHandlerMiddlewareExtension
    {
        public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder builder) => builder.UseMiddleware(typeof(CustomExceptionHandlerMiddleware));
    }
}
