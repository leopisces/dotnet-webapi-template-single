using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.Common;

/// <summary>
/// 统一 API 响应模型
/// </summary>
public class ApiResult
{
    /// <summary>
    /// 状态码
    /// </summary>
    public int Code { get; set; }

    /// <summary>
    /// 消息
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 数据
    /// </summary>
    public object? Data { get; set; }

    /// <summary>
    /// 成功响应
    /// </summary>
    public static ApiResult Success(object? data = null, string message = "success")
        => new() { Code = StatusCodes.Status200OK, Message = message, Data = data };

    /// <summary>
    /// 失败响应
    /// </summary>
    public static ApiResult Fail(int code, string message)
        => new() { Code = code, Message = message, Data = null };
}

/// <summary>
/// 统一响应格式 ActionFilter
/// <para>Controller 方法直接返回业务数据（Task&lt;T&gt;），Filter 自动包装为 ApiResult</para>
/// <para>异常和验证失败也会统一为 ApiResult 格式</para>
/// </summary>
public class UnifiedResultFilter : IActionFilter, IExceptionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        // 模型验证失败时统一格式
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState
                .Where(kvp => kvp.Value?.Errors.Count > 0)
                .Select(kvp => $"{kvp.Key}: {string.Join("; ", kvp.Value!.Errors.Select(e => e.ErrorMessage))}")
                .ToList();

            context.Result = new OkObjectResult(ApiResult.Fail(StatusCodes.Status400BadRequest, string.Join("| ", errors)));
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // 异常由 IExceptionFilter 处理
        if (context.Exception != null)
            return;

        // 已经是 ApiResult 的不再二次包装
        if (context.Result is ObjectResult objectResult)
        {
            if (objectResult.Value is ApiResult)
                return;

            // 空结果（如 Delete 返回 NoContent）
            if (objectResult.Value is null || context.Result is NoContentResult)
            {
                context.Result = new OkObjectResult(ApiResult.Success());
                return;
            }

            // 正常业务数据包装为 ApiResult
            context.Result = new OkObjectResult(ApiResult.Success(objectResult.Value));
        }
        else if (context.Result is OkResult)
        {
            context.Result = new OkObjectResult(ApiResult.Success());
        }
    }

    public void OnException(ExceptionContext context)
    {
        var code = context.Exception switch
        {
            BadHttpRequestException => StatusCodes.Status400BadRequest,
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            _ => StatusCodes.Status500InternalServerError
        };

        context.Result = new OkObjectResult(ApiResult.Fail(code, context.Exception.Message));
        context.ExceptionHandled = true;
    }
}

/// <summary>
/// IServiceCollection 扩展方法，注册统一响应格式
/// </summary>
public static class UnifiedResultExtensions
{
    /// <summary>
    /// 全局注册 UnifiedResultFilter，所有接口自动统一为 ApiResult 格式
    /// </summary>
    public static IServiceCollection AddUnifiedResult(this IServiceCollection services)
    {
        services.AddControllers(options =>
        {
            options.Filters.Add<UnifiedResultFilter>();
        });

        return services;
    }
}
