using AuctionService;
using AuctionService.Data;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// adds services to the dependency injection container. 
builder.Services.AddControllers();
// egister a DbContext class (AuctionDbContext) with the DI container
// options is the parameter of the lambda expression. It represents an instance of DbContextOptionsBuilder<AuctionDbContext>, which is used to configure options for the AuctionDbContext.
builder.Services.AddDbContext<AuctionDbContext>(opt =>
{
  opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// AppDomain.CurrentDomain.GetAssemblies() 确保发现并注册所有这些配置文件 (继承自Profile的 class, 在这是MappingProfile)
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// configures MassTransit for message-based applications, to use RabbitMQ as the message broker
builder.Services.AddMassTransit(x => {

  // The outbox pattern ensures that messages are not sent until the associated database transaction is committed successfully.
  // maintaining consistency between the database and the message broker.
  x.AddEntityFrameworkOutbox<AuctionDbContext>(o => 
  {
    // This configuration sets the delay between outbox queries to 10 seconds. The outbox will check for pending messages to be published at this interval.
    o.QueryDelay = TimeSpan.FromSeconds(10);

    o.UsePostgres();

    o.UseBusOutbox();
  });
  
  // adding all consumers by adding just 1
  x.AddConsumersFromNamespaceContaining<AuctionCreatedFaultConsumer>();

  x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("auction", false));

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

// 这行代码添加并配置了身份验证服务。JwtBearerDefaults.AuthenticationScheme 指定了默认的身份验证方案为 JWT Bearer Tokens。
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

builder.Services.AddGrpc();


var app = builder.Build();

app.UseAuthentication();

//  enforces authorization on the incoming requests. I
app.UseAuthorization();

// application's routes are determined by the controllers and their action methods.
app.MapControllers();

app.MapGrpcService<GrpcAuctionService>();

try
{
  Dbinitializer.InitDb(app);
}
catch (Exception e)
{
  
  Console.WriteLine(e);
}

app.Run();


