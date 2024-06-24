using AuthDemo.Dtos.Options;
using AuthDemo.Dtos.Repo;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static AuthDemo.Dtos.Response.TokenEndpoint;

namespace AuthDemo.Auth
{
    public static partial class TokenFactory
    {
        public static async Task<TokenReponse> Connect(HttpContext ctx, JwtOptions jwtOptions)
        {
            // validates the content type of the request
            var userName = await ValidateRequestCtx(ctx, jwtOptions);

            // get user roles & permissions
            UserRepo repo = new(); // this is a mock repo for the sake of the example 

            repo.ValidateUser(userName);
            var permissions = repo.GetUserPermissions(userName);

            //creates the access token (jwt token)
            var tokenExpiration = TimeSpan.FromSeconds(jwtOptions.ExpirationSeconds);
            var accessToken = CreateAccessToken(jwtOptions, userName, tokenExpiration, permissions);

            //returns a json response with the access token
            return new TokenReponse
            {
                access_token = accessToken,
                expiration = (int)tokenExpiration.TotalSeconds,
                type = "bearer"
            };
        }

        private static async Task<StringValues> ValidateRequestCtx(HttpContext ctx, JwtOptions jwtOptions)
        {
            if (ctx.Request.ContentType != "application/x-www-form-urlencoded")
            {
                ThrowInvalidRequestException();
            }

            var formCollection = await ctx.Request.ReadFormAsync();

            // pulls information from the form
            StringValues
                requestprivateKey = default,
                grantType = default,
                userName = default;

            if (formCollection.TryGetValue("grant_type", out grantType) == false ||
                formCollection.TryGetValue("private_key", out requestprivateKey) == false ||
                formCollection.TryGetValue("username", out userName) == false)
            {
                ThrowInvalidRequestException();
            }

            if (!string.Equals(requestprivateKey, jwtOptions.PrivateKey, StringComparison.Ordinal) ||
                !string.Equals(grantType, "private_key", StringComparison.Ordinal) ||
                string.IsNullOrWhiteSpace(userName))
            {
                ThrowUnauthorizeRequestException();
            }
            return userName;
        }

        public static string CreateAccessToken(JwtOptions jwtOptions, string username, TimeSpan expiration, string[] permissions)
        {
            var keyBytes = Encoding.UTF8.GetBytes(jwtOptions.SigningKey);
            var symmetricKey = new SymmetricSecurityKey(keyBytes);

            var signingCredentials = new SigningCredentials(symmetricKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>()
            {
                new("sub", username),
                new("name", username),
                new("aud", jwtOptions.Audience)
            };

            var roleClaims = permissions.Select(x => new Claim("role", x));
            claims.AddRange(roleClaims);

            var token = new JwtSecurityToken(
                issuer: jwtOptions.Issuer,
                audience: jwtOptions.Audience,
                claims: claims,
                expires: DateTime.Now.Add(expiration),
                signingCredentials: signingCredentials);

            var rawToken = new JwtSecurityTokenHandler().WriteToken(token);
            return rawToken;
        }
        private static void ThrowUnauthorizeRequestException() => throw new UnauthorizedAccessException("Invalid credentials");
        private static void ThrowInvalidRequestException() => throw new Exception("Invalid Request");
    }
}
