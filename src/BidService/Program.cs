using BidService;
using BidService.Services;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using MongoDB.Driver;
using MongoDB.Entities;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddMassTransit(x => {

  x.AddConsumersFromNamespaceContaining<AuctionCreatedConsumer>();

  x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("bids", false));
  // The parameter x is an instance of IBusRegistrationConfigurator, which is used to configure the MassTransit bus.
  //  specifies that RabbitMQ should be used as the transport for MassTransit
  x.UsingRabbitMq((context, config) => 
  {
    //  configuring the connection to a RabbitMQ message broker from appsettings.json
    config.Host(builder.Configuration["RabbitMq:Host"], "/", host => 
    {
      host.Username(builder.Configuration.GetValue("RabbitMq:Username","guest"));
      host.Password(builder.Configuration.GetValue("RabbitMq:Password", "guest"));    
    });

    //  configure all endpoints (consumers, sagas, activities, etc.) that are registered in the MassTransit configuration. 
    config.ConfigureEndpoints(context);
  });
});

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

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddHostedService<CheckAuctionFinished>();

builder.Services.AddScoped<GrpcAuctionClient>();

var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

await DB.InitAsync("BidDb", MongoClientSettings
  .FromConnectionString(builder.Configuration.GetConnectionString("BidDbConnection")));

app.Run();

