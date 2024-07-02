using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// 反向代理服务器是一种位于客户端设备和后端服务器之间的服务器。它拦截客户端请求并将其转发到一个或多个后端服务器，然后将后端服务器的响应通过反向代理返回给客户端。反向代理服务器用于多种目的，包括负载均衡、增强安全性和提高性能(cacheing)。
builder.Services.AddReverseProxy()
  .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
  //添加了 JWT Bearer 处理程序，并配置了处理程序的选项。
  .AddJwtBearer( opts => 
  {
    // 指定了 JWT Bearer 令牌的颁发者的 URL，identityServer的 URL
    opts.Authority = builder.Configuration["IdentityServiceUrl"];
    // identityServer runs on http
    opts.RequireHttpsMetadata = false;
    // 意味着在验证 JWT 令牌时，不会检查令牌中的 aud (Audience) 声明。这通常用于在开发或测试环境中，或者当你的应用程序不关心令牌的受众时。
    opts.TokenValidationParameters.ValidateAudience = false;
    // 自定义名称声明类型，使身份验证系统能够正确识别用户名称。
    // so in this service, username of a user can be accessed by User.Identity.Name
    opts.TokenValidationParameters.NameClaimType = "username";
  });


var app = builder.Build();

app.MapReverseProxy();

app.UseAuthentication();

app.UseAuthorization();

app.Run();
