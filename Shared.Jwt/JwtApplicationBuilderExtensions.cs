using Microsoft.AspNetCore.Builder;

namespace Shared.Jwt;

/// <summary>
/// IApplicationBuilder 扩展方法，启用 JWT 认证中间件
/// </summary>
public static class JwtApplicationBuilderExtensions
{
    /// <summary>
    /// 启用 Authentication 和 Authorization 中间件
    /// </summary>
    public static IApplicationBuilder UseJwtAuthentication(this IApplicationBuilder app)
    {
        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }
}
