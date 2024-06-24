using AuthDemo.Auth;
using AuthDemo.Dtos.Options;
using AuthDemo.Dtos.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AuthDemo.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [AllowAnonymous]
    [Consumes("application/x-www-form-urlencoded")]
    public class AuthController : ControllerBase
    {
        private readonly JwtOptions jwtOptions;
        private readonly HttpContext _httpContext;

        public AuthController(IOptions<JwtOptions> jwt, IHttpContextAccessor httpContextAccessor)
        {
            jwtOptions = jwt.Value;
            _httpContext = httpContextAccessor.HttpContext ?? throw new Exception("http context not accessible.");
        }

        [HttpPost("Token")]
        public async Task<IActionResult> Token([FromForm] TokenRequest request)
        {
            var response = await TokenFactory.Connect(_httpContext, jwtOptions);
            return Ok(response);
        }
    }
}
