// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.


using Microsoft.AspNetCore.Identity;

namespace IdentityService.Models;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
  internal async Task CreateAsync(ApplicationUser user, string password)
  {
    throw new NotImplementedException();
  }

  public static implicit operator ApplicationUser(UserManager<ApplicationUser> v)
  {
    throw new NotImplementedException();
  }
}
