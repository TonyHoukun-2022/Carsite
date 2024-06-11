using AuctionService.Data;
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


