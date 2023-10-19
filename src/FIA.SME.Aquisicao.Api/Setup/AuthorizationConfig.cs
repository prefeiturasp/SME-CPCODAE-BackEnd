using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace FIA.SME.Aquisicao.Api.Setup
{
    public static class AuthorizationConfig
    {
        public static void AddAuthorizationConfigServices(this IServiceCollection services, IConfiguration configuration)
        {
            var audience = configuration.GetSection("Jwt:Audience").Value;
            var issuer = configuration.GetSection("Jwt:Issuer").Value;
            var secret = configuration.GetSection("Jwt:Secret").Value;
            var key = Encoding.ASCII.GetBytes(secret);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience,
                };
            });

            services.AddAuthorization();
        }

        public static void UseAuthorizationConfigure(this IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseAuthorization();
        }
    }
}
