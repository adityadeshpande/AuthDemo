using AuthDemo.Dtos.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace AuthDemo.Middleware
{
    public class TokenValidationHandler
    {
        private static JwtOptions _jwtOptions;

        public static void CofigureTokenvalidationMiddleware(IApplicationBuilder app, JwtOptions jwtOptions)
        {
            _jwtOptions = jwtOptions ?? throw new ArgumentNullException(nameof(jwtOptions));
            app.UseMiddleware<TokenValidationMiddleware>();
        }

        public class TokenValidationMiddleware
        {
            private readonly RequestDelegate _next;

            public TokenValidationMiddleware(RequestDelegate next)
            {
                _next = next;
            }

            public async Task Invoke(HttpContext context)
            {
                var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                if (token != null)
                {
                    AttachUserToContext(context, token);
                }

                await _next(context);
            }

            private static void AttachUserToContext(HttpContext context, string token)
            {
                try
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.ASCII.GetBytes(_jwtOptions.SigningKey);
                    tokenHandler.ValidateToken(token, new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ClockSkew = TimeSpan.Zero,
                        ValidIssuer = _jwtOptions.Issuer,
                        ValidAudience = _jwtOptions.Audience,
                        ValidateLifetime = true,
                        ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha256 }

                    }, out SecurityToken validatedToken);

                    var jwtToken = (JwtSecurityToken)validatedToken;
                    var userId = jwtToken.Claims.First(x => x.Type == "name").Value;

                    // attach user to context on successful jwt validation
                    context.Items["User"] = userId;
                    context.Items["Permissions"] = string.Join(",", jwtToken.Claims.Where(x => x.Type == "role").Select(x => x.Value));
                }
                catch
                {
                    throw new UnauthorizedAccessException("Invalid token");
                }
            }
        }
    }
}