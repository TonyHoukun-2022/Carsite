using System.Net;
using Polly;
using Polly.Extensions.Http;
using SearchService.Data;
using SearchService.Services;

var builder = WebApplication.CreateBuilder(args);

// adds services to the DI containers
builder.Services.AddControllers();
// leverages the HttpClientFactory to configure and manage HttpClient instances used by the AuctionServiceHttpClient
builder.Services.AddHttpClient<AuctionServiceHttpClient>().AddPolicyHandler(GetPolicy());

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
