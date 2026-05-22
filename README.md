# dotnet-webapi-template-single

基于 .NET 10 的 WebAPI 单项目模板，集成 Swagger、JWT 认证、统一响应格式，共享代码提取为可复用的类库。

## 项目结构

```
dotnet-webapi-template-single/
├── Shared.Common/          # 统一响应格式（ApiResult + ActionFilter）
├── Shared.Entities/        # 实体类（WeatherForecast、TokenResult、LoginRequest）
├── Shared.Jwt/             # JWT 认证（服务注册 + Token 生成）
├── Shared.Swagger/         # Swagger（配置驱动 + Authorize 锁图标）
└── WebApiTemplateSingle/   # 主项目（Controllers + Program.cs）
```

## 功能特性

- **Swagger 文档** — 配置驱动，支持 XML 注释（含控制器级）、JWT Bearer 认证输入框
- **JWT 认证** — 配置驱动，HMAC-SHA256 签名，`[Authorize]` 控制接口权限
- **统一响应格式** — ActionFilter 自动包装 `ApiResult`，控制器直接返回 `Task<T>`
- **路由全小写** — 全局 `LowercaseUrls`，路由模板 `api/[controller]/[action]`
- **共享类库** — Common / Entities / Jwt / Swagger 独立类库，可直接复用到其他项目

## 快速开始

```bash
# 克隆
git clone https://github.com/leopisces/dotnet-webapi-template-single.git
cd dotnet-webapi-template-single

# 运行
dotnet run --project WebApiTemplateSingle
```

启动后自动打开 Swagger 页面：`https://localhost:5001/swagger`

## 配置说明

所有功能通过 `appsettings.json` 配置驱动：

```json
{
  "Swagger": {
    "Title": "WebApiTemplateSingle API",
    "Version": "v1",
    "Description": "WebApiTemplateSingle API Documentation",
    "IncludeXmlComments": true,
    "EnableJwtBearer": true
  },
  "Jwt": {
    "SecretKey": "YourSuperSecretKeyAtLeast32Chars!!",
    "Issuer": "WebApiTemplateSingle",
    "Audience": "WebApiTemplateSingle",
    "ExpireHours": 24
  }
}
```

| 配置项 | 说明 | 默认值 |
|--------|------|--------|
| `Swagger:Title` | Swagger 文档标题 | `API Documentation` |
| `Swagger:Version` | API 版本号 | `v1` |
| `Swagger:IncludeXmlComments` | 是否显示 XML 注释 | `true` |
| `Swagger:EnableJwtBearer` | 是否启用 Authorize 按钮 | `false` |
| `Jwt:SecretKey` | 签名密钥（至少 32 字符） | - |
| `Jwt:Issuer` | 签发者 | - |
| `Jwt:Audience` | 受众 | - |
| `Jwt:ExpireHours` | Token 过期时间（小时） | `24` |

## 统一响应格式

控制器方法直接返回业务数据，`UnifiedResultFilter` 自动包装为统一格式：

```csharp
// 控制器：直接返回 Task<T>
[HttpGet]
public async Task<IEnumerable<WeatherForecast>> GetAsync() { ... }

// 响应格式
{
  "code": 200,
  "message": "success",
  "data": [ ... ]
}
```

### 异常处理

| 异常类型 | HTTP 状态码 | 场景 |
|----------|------------|------|
| `BadHttpRequestException` | 400 | 参数校验失败 |
| `UnauthorizedAccessException` | 401 | 未授权访问 |
| 模型验证失败 | 400 | ModelState 无效 |
| 其他异常 | 500 | 服务器内部错误 |

## JWT 认证使用

### 登录获取 Token

```
POST /api/auth/loginasync
Content-Type: application/json

{
  "userName": "demo",
  "password": "demo"
}
```

### 访问受保护接口

在 Swagger 页面点击 🔒 Authorize 按钮，输入 `Bearer {token}`，或手动添加请求头：

```
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
```

### 标记接口需要认证

```csharp
[ApiController]
[Route("api/[controller]/[action]")]
[Authorize]  // 整个控制器需要认证
public class MyController : ControllerBase { ... }
```

Swagger 中仅标记了 `[Authorize]` 的接口会显示 🔒 锁图标。

## 共享类库说明

### Shared.Common

| 类 | 说明 |
|----|------|
| `ApiResult` | 统一响应模型（Code / Message / Data） |
| `UnifiedResultFilter` | ActionFilter，自动包装响应 + 异常处理 |
| `AddUnifiedResult()` | 替代 `AddControllers()`，注册 Filter |

### Shared.Entities

| 类 | 说明 |
|----|------|
| `TokenResult` | JWT Token 返回模型 |
| `LoginRequest` | 登录请求模型 |
| `WeatherForecast` | 示例实体 |

### Shared.Jwt

| 类 | 说明 |
|----|------|
| `IJwtTokenService` | Token 生成接口 |
| `JwtTokenService` | Token 生成实现 |
| `JwtOptions` | 配置选项 |
| `AddJwtAuthentication()` | 注册认证服务 |
| `UseJwtAuthentication()` | 注册认证中间件 |

### Shared.Swagger

| 类 | 说明 |
|----|------|
| `SwaggerOptions` | 配置选项 |
| `AuthorizeCheckOperationFilter` | 按 `[Authorize]` 显示锁图标 |
| `AddSwaggerServices()` | 注册 Swagger 服务 |
| `UseSwaggerDocumentation()` | 注册 Swagger 中间件 |

## 技术栈

- .NET 10
- Swashbuckle.AspNetCore 10.1.7（Microsoft.OpenApi 2.4.1）
- Microsoft.AspNetCore.Authentication.JwtBearer 10.0.8
