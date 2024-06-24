using AuthDemo.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;

namespace AuthDemo.Middleware
{
    public class ExceptionHandler
    {
        public static void ConfigureAPIExceptionHandling(IApplicationBuilder app)
        {
            app.UseExceptionHandler(c => c.Run(async context =>
            {
                var ex = context.Features.Get<IExceptionHandlerPathFeature>().Error;
                context.Response.StatusCode = (int)GetStatusCode(ex);

                await context.Response.WriteAsJsonAsync(new APIExceptionResponse
                {
                    Type = ex.GetType().Name,
                    Message = ex.Message,
                    StatusCode = (HttpStatusCode)context.Response.StatusCode
                });
            }));
        }

        private static HttpStatusCode GetStatusCode(Exception ex)
        {
            if (ex is UnauthorizedAccessException) return HttpStatusCode.Unauthorized;
            if (ex is UserNotFoundException) return HttpStatusCode.Unauthorized;
            if (ex is KeyNotFoundException) return HttpStatusCode.NotFound;
            if (ex is ArgumentException) return HttpStatusCode.BadRequest;
            return ex is not null ? HttpStatusCode.InternalServerError : HttpStatusCode.ServiceUnavailable;
        }
    }
    public class APIExceptionResponse
    {
        public string Type { get; set; }
        public string Message { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }
}
