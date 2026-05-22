using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Shared.Jwt;

/// <summary>
/// JWT Token 生成结果
/// </summary>
public class TokenResult
{
    /// <summary>
    /// 访问令牌
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// 令牌类型
    /// </summary>
    public string TokenType { get; set; } = "Bearer";

    /// <summary>
    /// 过期时间（秒）
    /// </summary>
    public long ExpiresIn { get; set; }
}

/// <summary>
/// JWT Token 生成服务
/// </summary>
public interface IJwtTokenService
{
    /// <summary>
    /// 根据用户名和自定义声明生成 JWT Token
    /// </summary>
    TokenResult GenerateToken(string userName, IEnumerable<Claim>? additionalClaims = null);
}

/// <summary>
/// JWT Token 生成服务实现
/// </summary>
public class JwtTokenService : IJwtTokenService
{
    private readonly JwtOptions _options;
    private readonly SymmetricSecurityKey _securityKey;

    public JwtTokenService(IConfiguration configuration)
    {
        _options = new JwtOptions();
        configuration.GetSection("Jwt").Bind(_options);
        _securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));
    }

    /// <summary>
    /// 根据用户名和自定义声明生成 JWT Token
    /// </summary>
    public TokenResult GenerateToken(string userName, IEnumerable<Claim>? additionalClaims = null)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, userName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        if (additionalClaims != null)
        {
            claims.AddRange(additionalClaims);
        }

        var credentials = new SigningCredentials(_securityKey, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddHours(_options.ExpireHours);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: credentials
        );

        return new TokenResult
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresIn = (long)(expires - DateTime.UtcNow).TotalSeconds
        };
    }
}
