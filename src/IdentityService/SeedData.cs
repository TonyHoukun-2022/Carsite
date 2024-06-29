using System.Security.Claims;
using IdentityModel;
using IdentityService.Data;
using IdentityService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace IdentityService;

public class SeedData
{
    public static void EnsureSeedData(WebApplication app)
    {
    using var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate();

    var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    if (userMgr.Users.Any()) return;

    var tony = userMgr.FindByNameAsync("tony").Result;
    if (tony == null)
    {
      tony = new ApplicationUser
      {
        UserName = "tony",
        Email = "kevinmomochi001@gmail.com",
        EmailConfirmed = true,
      };
      var result = userMgr.CreateAsync(tony, "WS@lp2dxing").Result;
      if (!result.Succeeded)
      {
        throw new Exception(result.Errors.First().Description);
      }

      result = userMgr.AddClaimsAsync(tony, new Claim[]{
                            new Claim(JwtClaimTypes.Name, "Tony Peng")}).Result;
      if (!result.Succeeded)
      {
        throw new Exception(result.Errors.First().Description);
      }
      Log.Debug("tony created");
    }
    else
    {
      Log.Debug("tony already exists");
    }

    var bob = userMgr.FindByNameAsync("bob").Result;
    if (bob == null)
    {
      bob = new ApplicationUser
      {
        UserName = "bob",
        Email = "BobSmith@email.com",
        EmailConfirmed = true
      };
      var result = userMgr.CreateAsync(bob, "WS@lp2dxing").Result;
      if (!result.Succeeded)
      {
        throw new Exception(result.Errors.First().Description);
      }

      result = userMgr.AddClaimsAsync(bob, new Claim[]{
                            new Claim(JwtClaimTypes.Name, "Bob Smith"),
                        }).Result;
      if (!result.Succeeded)
      {
        throw new Exception(result.Errors.First().Description);
      }
      Log.Debug("bob created");
    }
    else
    {
      Log.Debug("bob already exists");
    }

     var alice = userMgr.FindByNameAsync("alice").Result;
    if (alice == null)
    {
      alice = new ApplicationUser
      {
        UserName = "alice",
        Email = "aliceSmith@email.com",
        EmailConfirmed = true
      };
      var result = userMgr.CreateAsync(alice, "WS@lp2dxing").Result;
      if (!result.Succeeded)
      {
        throw new Exception(result.Errors.First().Description);
      }

      result = userMgr.AddClaimsAsync(alice, new Claim[]{
                            new Claim(JwtClaimTypes.Name, "Alice Smith"),
                        }).Result;
      if (!result.Succeeded)
      {
        throw new Exception(result.Errors.First().Description);
      }
      Log.Debug("alice created");
    }
    else
    {
      Log.Debug("alice already exists");
    }

     var tom = userMgr.FindByNameAsync("tom").Result;
    if (tom == null)
    {
      tom = new ApplicationUser
      {
        UserName = "tom",
        Email = "TomJack@email.com",
        EmailConfirmed = true
      };
      var result = userMgr.CreateAsync(tom, "WS@lp2dxing").Result;
      if (!result.Succeeded)
      {
        throw new Exception(result.Errors.First().Description);
      }

      result = userMgr.AddClaimsAsync(tom, new Claim[]{
                            new Claim(JwtClaimTypes.Name, "Tom Jack"),
                        }).Result;
      if (!result.Succeeded)
      {
        throw new Exception(result.Errors.First().Description);
      }
      Log.Debug("tom created");
    }
    else
    {
      Log.Debug("tom already exists");
    }
  }
}
