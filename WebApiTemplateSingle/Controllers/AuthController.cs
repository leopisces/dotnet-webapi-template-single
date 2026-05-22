using Microsoft.AspNetCore.Mvc;
using Shared.Jwt;

namespace WebApiTemplateSingle.Controllers
{
    /// <summary>
    /// 认证管理
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IJwtTokenService _tokenService;

        public AuthController(IJwtTokenService tokenService)
        {
            _tokenService = tokenService;
        }

        /// <summary>
        /// 登录获取令牌
        /// </summary>
        /// <param name="request">登录请求</param>
        /// <returns>JWT 令牌</returns>
        [HttpPost("login")]
        public async Task<TokenResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.Password))
            {
                throw new BadHttpRequestException("用户名和密码不能为空");
            }

            // TODO: 替换为实际的用户验证逻辑（如查数据库）
            await Task.CompletedTask;

            if (request.UserName != "demo" || request.Password != "demo")
            {
                throw new UnauthorizedAccessException("用户名或密码错误");
            }

            return _tokenService.GenerateToken(request.UserName);
        }
    }

    /// <summary>
    /// 登录请求模型
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; } = string.Empty;
    }
}
