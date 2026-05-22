namespace Shared.Entities;

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
