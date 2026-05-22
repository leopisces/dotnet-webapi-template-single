using System.Reflection;
using Microsoft.AspNetCore.Authorization;
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

    /// <summary>
    /// 是否启用 JWT Bearer 认证输入框（从 ASP.NET Core 认证配置自动推断）
    /// </summary>
    public bool EnableJwtBearer { get; set; } = false;
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

            if (options.EnableJwtBearer)
            {
                sgOptions.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT 授权令牌，请输入 Bearer {token}",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                });
                sgOptions.OperationFilter<AuthorizeCheckOperationFilter>();
            }
        });

        return services;
    }
}

/// <summary>
/// 仅对标有 [Authorize] 的接口添加 JWT 安全要求
/// </summary>
public class AuthorizeCheckOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var hasAuthorize = context.MethodInfo.DeclaringType?.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any() == true
            || context.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any();

        if (!hasAuthorize) return;

        operation.Security =
        [
            new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecuritySchemeReference("Bearer", context.Document),
                    []
                }
            }
        ];
    }
}
