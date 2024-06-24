using AuthDemo.Dtos.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace AuthDemo.Helpers
{
    public static class ApplicationBuilderHelper
    {
        public static void CofigureServiceDI(WebApplicationBuilder builder)
        {
            builder.Services.AddHttpContextAccessor();
        }

        public static void ConfigureOptions(WebApplicationBuilder builder)
        {
            ConfigAuthStuff(builder);
        }
        private static void ConfigAuthStuff(WebApplicationBuilder builder)
        {
            builder.Services.Configure<JwtOptions>(option => builder.Configuration.GetSection("JwtOptions").Bind(option));
        }
        internal static void AddAuthentication(WebApplicationBuilder builder)
        {
            var jwtOptions = builder.Configuration.GetSection("JwtOptions").Get<JwtOptions>();
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opts =>
            {
                //convert the string signing key to byte array
                byte[] signingKeyBytes = Encoding.UTF8.GetBytes(jwtOptions.SigningKey);

                opts.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(signingKeyBytes)
                };
            });

        }

        public static void ConfigureSwagger(WebApplicationBuilder builder)
        {
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AuthDemo.API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme."

                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                          new OpenApiSecurityScheme
                          {
                              Reference = new OpenApiReference
                              {
                                  Type = ReferenceType.SecurityScheme,
                                  Id = "Bearer"
                              }
                          },
                         new string[] {}
                    }
                });
            });
        }
    }
}
