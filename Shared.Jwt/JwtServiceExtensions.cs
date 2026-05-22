using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Shared.Jwt;

/// <summary>
/// JWT 认证配置选项，可通过 appsettings.json 的 "Jwt" 节绑定
/// </summary>
public class JwtOptions
{
    /// <summary>
    /// 密钥（至少 16 个字符）
    /// </summary>
    public string SecretKey { get; set; } = string.Empty;

    /// <summary>
    /// 签发者
    /// </summary>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    /// 受众
    /// </summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// Token 过期时间（小时），默认 24 小时
    /// </summary>
    public int ExpireHours { get; set; } = 24;
}

/// <summary>
/// IServiceCollection 扩展方法，注册 JWT 认证服务
/// </summary>
public static class JwtServiceExtensions
{
    /// <summary>
    /// 从 IConfiguration 的 "Jwt" 节读取配置并注册 JWT 认证服务
    /// </summary>
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<JwtOptions>? configure = null)
    {
        var options = new JwtOptions();
        configuration.GetSection("Jwt").Bind(options);
        configure?.Invoke(options);

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(jwtOptions =>
            {
                jwtOptions.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = options.Issuer,
                    ValidAudience = options.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        System.Text.Encoding.UTF8.GetBytes(options.SecretKey))
                };
            });

        services.AddAuthorization();

        services.AddSingleton<IJwtTokenService, JwtTokenService>();

        return services;
    }
}
