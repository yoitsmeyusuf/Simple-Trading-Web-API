using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace Takasbu.Middleware
{
    public class TokenAuthenticationMiddleware : IMiddleware
    {

        private readonly IConfiguration _configuration;

        public TokenAuthenticationMiddleware(IConfiguration configuration)
        {
            _configuration = configuration;

        }
        public bool ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value!);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
                return true;
            }
            catch
            {
                return false;
            }
        }

        async Task IMiddleware.InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (context.Request.Path.StartsWithSegments("/api/Auth/register")||context.Request.Path.StartsWithSegments("/api/Auth/login"))
            {
                await next(context);
                return;
                
            }
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
            {
                var isValidToken = ValidateToken(token);

                if (isValidToken)
                {
                    await next(context);
                }
            }
            else
            {
                // Token bulunamadı veya geçerli değilse, isteği reddet
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            }
        }


    }

}

