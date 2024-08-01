using MassTransit;
using NotificationService.Consumers;
using NotificationService.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMassTransit(x => {

  x.AddConsumersFromNamespaceContaining<AuctionCreatedConsumer>();

  x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("notification", false));
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

builder.Services.AddSignalR();

var app = builder.Build();

app.MapHub<NotificationHub>("/notifications");


app.Run();
