using Shared.Jwt;
using Shared.Swagger;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Swagger（从 appsettings.json "Swagger" 节加载配置）
builder.Services.AddSwaggerServices(builder.Configuration);

// JWT 认证（从 appsettings.json "Jwt" 节加载配置）
builder.Services.AddJwtAuthentication(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwaggerDocumentation();

app.UseHttpsRedirection();

// JWT 认证中间件
app.UseJwtAuthentication();

app.MapControllers();

app.Run();
