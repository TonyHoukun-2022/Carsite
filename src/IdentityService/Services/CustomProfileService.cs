using System.Security.Claims;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using IdentityModel;
using IdentityService.Models;
using Microsoft.AspNetCore.Identity;

namespace IdentityService;

public class CustomProfileService : IProfileService
{
  private readonly UserManager<ApplicationUser> _userManager;
  public CustomProfileService(UserManager<ApplicationUser> userManager)
  {
    _userManager = userManager;
  }

  public async Task GetProfileDataAsync(ProfileDataRequestContext context)
  {
    // context.Subject：表示当前用户的身份信息。context.Subject 是一个 ClaimsPrincipal 对象，它包含了多个 ClaimsIdentity，每个 ClaimsIdentity 包含多个声明（Claim）。
    var user = await _userManager.GetUserAsync(context.Subject);
    //  从数据库中获取用户的现有声明。
    var existingClaims = await _userManager.GetClaimsAsync(user);

    // 创建一个新的声明列表并添加一个 username 声明。
    var claims = new List<Claim>
    {
      new Claim("username", user.UserName)
    };

    //将新的声明和现有声明添加到 context.IssuedClaims，这些声明将作为用户的身份信息传递出去。这确保了用户的名称声明被包含在发出的身份验证令牌中，以便在身份验证过程中传递给应用程序。
    context.IssuedClaims.AddRange(claims);
    // 将当前用户现有声明中类型为 JwtClaimTypes.Name 的声明添加到即将发出的声明集合中
    context.IssuedClaims.Add(
      existingClaims.FirstOrDefault(x => x.Type == JwtClaimTypes.Name)
    );
  }

  // 这段代码实现了 IsActiveAsync 方法，返回一个已完成的任务。该方法被用来检查用户是否活跃，但当前实现中没有任何检查逻辑，默认所有用户都是活跃的。
  public Task IsActiveAsync(IsActiveContext context)
  {
    return Task.CompletedTask;
  }
}
