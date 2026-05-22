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
        public ActionResult<TokenResult> Login([FromBody] LoginRequest request)
        {
            // TODO: 替换为实际的用户验证逻辑
            if (string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new { message = "用户名和密码不能为空" });
            }

            // 示例验证：任意用户名密码均为 demo/demo 时返回令牌
            if (request.UserName != "demo" || request.Password != "demo")
            {
                return Unauthorized(new { message = "用户名或密码错误" });
            }

            var token = _tokenService.GenerateToken(request.UserName);
            return Ok(token);
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
