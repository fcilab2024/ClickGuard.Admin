#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using ClickGuard.Admin.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace ClickGuard.Admin.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public LoginModel(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ILogger<LoginModel> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

		public async Task OnGetAsync(string returnUrl = null)
		{
		// ✅ Ignore ReturnUrl entirely
		ReturnUrl = Url.Content("~/");

		await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

		ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
		}
		
		public async Task<IActionResult> OnPostAsync(string returnUrl = null)
		{
		// ✅ Always ignore returnUrl for safety
		returnUrl = null;
	
		if (ModelState.IsValid)
		{
        var result = await _signInManager.PasswordSignInAsync(
            Input.Email,
            Input.Password,
            Input.RememberMe,
            lockoutOnFailure: false);

        if (result.Succeeded)
        {
            _logger.LogInformation("User logged in.");

            var user = await _userManager.FindByEmailAsync(Input.Email);

            // ✅ Role-based routing ONLY (NO ReturnUrl)
            if (user != null && await _userManager.IsInRoleAsync(user, "FcsAdmin"))
            {
                return Redirect("/Admin/Tenants");
            }

            return Redirect("/Dashboard");
        }

        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
		}

		return Page();
		}
    }
}