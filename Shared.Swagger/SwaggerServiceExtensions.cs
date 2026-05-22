using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Shared.Swagger;

/// <summary>
/// Swagger 服务注册配置选项，可通过 appsettings.json 的 "Swagger" 节绑定
/// </summary>
public class SwaggerOptions
{
    /// <summary>
    /// API 文档标题
    /// </summary>
    public string Title { get; set; } = "API Documentation";

    /// <summary>
    /// API 版本号
    /// </summary>
    public string Version { get; set; } = "v1";

    /// <summary>
    /// API 文档描述
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// 是否启用 XML 注释（需要项目开启 GenerateDocumentationFile）
    /// </summary>
    public bool IncludeXmlComments { get; set; } = true;
}

/// <summary>
/// IServiceCollection 扩展方法，注册 Swagger 服务
/// </summary>
public static class SwaggerServiceExtensions
{
    /// <summary>
    /// 从 IConfiguration 的 "Swagger" 节读取配置并注册 Swagger 服务
    /// </summary>
    public static IServiceCollection AddSwaggerServices(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<SwaggerOptions>? configure = null)
    {
        var options = new SwaggerOptions();
        configuration.GetSection("Swagger").Bind(options);
        configure?.Invoke(options);

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(sgOptions =>
        {
            sgOptions.SwaggerDoc(options.Version, new OpenApiInfo
            {
                Title = options.Title,
                Version = options.Version,
                Description = options.Description
            });

            if (options.IncludeXmlComments)
            {
                var xmlFile = $"{Assembly.GetEntryAssembly()?.GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    sgOptions.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
                }
            }
        });

        return services;
    }
}
