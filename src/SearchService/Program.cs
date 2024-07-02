using System.Net;
using MassTransit;
using Polly;
using Polly.Extensions.Http;
using SearchService;
using SearchService.Data;
using SearchService.Services;

var builder = WebApplication.CreateBuilder(args);

// adds services to the DI containers
builder.Services.AddControllers();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
// leverages the HttpClientFactory to configure and manage HttpClient instances used by the AuctionServiceHttpClient
builder.Services.AddHttpClient<AuctionServiceHttpClient>().AddPolicyHandler(GetPolicy());
builder.Services.AddMassTransit(x => {

  x.AddConsumersFromNamespaceContaining<AuctionCreatedConsumer>();

  //auctionCreated message will be named search-auction-created in rabbitmq console
  x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("search", false));
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
    
    config.ReceiveEndpoint("search-auction-created", e =>
    {
      e.UseMessageRetry(r => r.Interval(5, 5));

      e.ConfigureConsumer<AuctionCreatedConsumer>(context);
    });
    //  configure all endpoints (consumers, sagas, activities, etc.) that are registered in the MassTransit configuration. 
    config.ConfigureEndpoints(context);
  });
});

var app = builder.Build();

//  enforces authorization on the incoming requests. I
app.UseAuthorization();

// application's routes are determined by the controllers and their action methods.
app.MapControllers();

app.Lifetime.ApplicationStarted.Register(async () =>
{
  try
  {
    await DbInitializer.InitDb(app);
  }
  catch (Exception e)
  {
    
    Console.WriteLine(e);
  }
});

app.Run();

// policy - handle response when searchService start before auctionservice, so cannot pre-install data by sync http request to auctionsvc
static IAsyncPolicy<HttpResponseMessage> GetPolicy()
  => HttpPolicyExtensions
      .HandleTransientHttpError()
      .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound)
      .WaitAndRetryForeverAsync(_ => TimeSpan.FromSeconds(3));
