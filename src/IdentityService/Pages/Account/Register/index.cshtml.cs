using System.Security.Claims;
using IdentityModel;
using IdentityService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityService.Pages.Register

{
  [SecurityHeaders]
  [AllowAnonymous]
  public class Index : PageModel
  {   
    private readonly UserManager<ApplicationUser> _userManager;

    // using UserManager<ApplicationUser> is to define a user manager for the ApplicationUser type. This allows you to use the UserManager class to perform operations specifically for ApplicationUser instances, such as creating users, finding users, updating user information, and more.
    public Index(UserManager<ApplicationUser> userManager)
    {
      _userManager = userManager;
    }

    // This means that data from HTTP requests (such as form submissions) will be automatically mapped to the Input property.
    [BindProperty]
    // property named Input of type RegisterViewModel.
    public RegisterViewModel Input { get; set; }

    [BindProperty]
    public bool RegisterSuccess { get; set; }
    
    // accept returnUrl as query string
    public IActionResult OnGet(string returnUrl)
    {
      // create a new instance of RegisterViewModel, with the binded props, and set another prop ReturnUrl
      Input = new RegisterViewModel
      {
        ReturnUrl = returnUrl,
      };

      //return current page
      return Page();
    }

    public async Task<IActionResult> OnPost()
    {
      // press cancel btn, redirect to home page of Identityserver
      if (Input.Button != "register") return Redirect("~/");

      if (ModelState.IsValid)
      {
        var user = new ApplicationUser
        {
          UserName = Input.Username,
          Email = Input.Email,
          EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, Input.Password);

        if (result.Succeeded)
        {
          // adds claims to the user. Claims are key-value pairs associated with a user, provide additional information about the user’s identity.

          //The primary use case is user registration, where a new user is created and additional identity information is stored as claims.
          // Claims can be used for various purposes, such as personalization, authorization, and storing user-specific data that might be needed across the application.
          await _userManager.AddClaimsAsync(user, new Claim[]
          {
            new Claim(JwtClaimTypes.Name, Input.FullName)
          });

          RegisterSuccess = true;
        }
      }

      return Page();
    }
  }
}
