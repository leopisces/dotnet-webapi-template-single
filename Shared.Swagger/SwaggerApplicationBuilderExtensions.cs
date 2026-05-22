using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Shared.Swagger;

/// <summary>
/// IApplicationBuilder 扩展方法，启用 Swagger 中间件
/// </summary>
public static class SwaggerApplicationBuilderExtensions
{
    /// <summary>
    /// 启用 Swagger JSON 终端和 Swagger UI（仅在 Development 环境生效）
    /// </summary>
    public static IApplicationBuilder UseSwaggerDocumentation(
        this IApplicationBuilder app,
        string version = "v1")
    {
        if (app.ApplicationServices.GetRequiredService<IHostEnvironment>().IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint($"/swagger/{version}/swagger.json", $"API {version}");
            });
        }

        return app;
    }
}
