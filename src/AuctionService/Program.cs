using AuctionService.Data;
using MassTransit;
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

// configures MassTransit, a popular .NET library for message-based applications, to use RabbitMQ as the message broker
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

  // The parameter x is an instance of IBusRegistrationConfigurator, which is used to configure the MassTransit bus.
  //  specifies that RabbitMQ should be used as the transport for MassTransit
  x.UsingRabbitMq((context, config) => 
  {
    //  configure all endpoints (consumers, sagas, activities, etc.) that are registered in the MassTransit configuration. 
    config.ConfigureEndpoints(context);
  });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
// middlewares
if (app.Environment.IsDevelopment())
{

}

//  enforces authorization on the incoming requests. I
app.UseAuthorization();

// application's routes are determined by the controllers and their action methods.
app.MapControllers();

try
{
  Dbinitializer.InitDb(app);
}
catch (Exception e)
{
  
  Console.WriteLine(e);
}

app.Run();


