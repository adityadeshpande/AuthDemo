using AuthDemo.Dtos.Options;
using AuthDemo.Middleware;

namespace AuthDemo.Helpers
{
    public static class ApplicationMiddlewareHelper
    {

        public static void UseExceptionHandlerMiddleware(this IApplicationBuilder builder)
        {
            ExceptionHandler.ConfigureAPIExceptionHandling(builder);
        }

        public static void ConfigureTokenValidationMiddleware(this IApplicationBuilder builder, JwtOptions jwtOptions)
        {
            TokenValidationHandler.CofigureTokenvalidationMiddleware(builder, jwtOptions);
        }
    }
}
