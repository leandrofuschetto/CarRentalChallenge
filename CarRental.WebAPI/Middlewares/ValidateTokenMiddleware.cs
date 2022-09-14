using CarRental.Service.Users;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace CarRental.WebAPI.Middlewares
{
    public class ValidateTokenMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public ValidateTokenMiddleware(RequestDelegate next, IConfiguration config)
        {
            _next = next;
            _configuration = config;
        }

        public async Task Invoke(HttpContext context, IUsersService userService)
        {
            var token = context.Request.Headers["Authorization"]
                .FirstOrDefault()?.Split(" ").Last();

            var userId = ValidateToken(token);
            if (userId > 0)
            {
                context.Items["User"] = await userService.GetUserById(Convert.ToInt32(userId));
            }

            await _next(context);
        }

        public int? ValidateToken(string token)
        {
            if (token == null)
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var keyToken = _configuration.GetSection("ConfigForJwt").Value;
            var key = Encoding.ASCII.GetBytes(keyToken);
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);

                return userId;
            }
            catch
            { 
                return null;
            }
        }
    }
}
