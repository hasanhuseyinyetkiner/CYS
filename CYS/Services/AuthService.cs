using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace CYS.Services
{
    public class JwtTokenService
    {
        private readonly IConfiguration _configuration;

        public JwtTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(string clientId, string clientName, int validityMinutes = 1440)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, clientId),
                new Claim(ClaimTypes.Name, clientName),
                new Claim("client_type", "milk_analysis_client"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(validityMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public ClaimsPrincipal ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]);

            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["JwtSettings:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["JwtSettings:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return principal;
            }
            catch
            {
                return null;
            }
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly JwtTokenService _jwtTokenService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(JwtTokenService jwtTokenService, ILogger<AuthController> logger)
        {
            _jwtTokenService = jwtTokenService;
            _logger = logger;
        }

        [HttpPost("api/auth/token")]
        public IActionResult GetToken([FromBody] AuthRequest request)
        {
            try
            {
                // Basit client authentication - gerçek uygulamada veritabanından kontrol edilmeli
                if (request.ClientId == "MERLAB-SutAnalizi" && request.ClientSecret == "MerlabSutAnalizi2024!")
                {
                    var token = _jwtTokenService.GenerateToken(request.ClientId, "Merlab Süt Analizi Client");
                    
                    _logger.LogInformation($"Token generated for client: {request.ClientId}");
                    
                    return Ok(new AuthResponse
                    {
                        Token = token,
                        TokenType = "Bearer",
                        ExpiresIn = 86400, // 24 hours
                        ClientId = request.ClientId
                    });
                }

                _logger.LogWarning($"Invalid authentication attempt for client: {request.ClientId}");
                return Unauthorized(new { error = "Invalid client credentials" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating token");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        [HttpPost("api/auth/refresh")]
        [Authorize]
        public IActionResult RefreshToken()
        {
            try
            {
                var clientId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var clientName = User.FindFirst(ClaimTypes.Name)?.Value;

                if (string.IsNullOrEmpty(clientId))
                {
                    return Unauthorized(new { error = "Invalid token" });
                }

                var newToken = _jwtTokenService.GenerateToken(clientId, clientName);
                
                return Ok(new AuthResponse
                {
                    Token = newToken,
                    TokenType = "Bearer",
                    ExpiresIn = 86400,
                    ClientId = clientId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing token");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }
    }

    public class AuthRequest
    {
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
    }

    public class AuthResponse
    {
        public string Token { get; set; } = string.Empty;
        public string TokenType { get; set; } = "Bearer";
        public int ExpiresIn { get; set; }
        public string ClientId { get; set; } = string.Empty;
    }
}
