using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RoomBookingCore.Data;
using System.Security.Claims;

namespace RoomBookingCore.Pages
{
    public class LoginModel : PageModel
    {
        private readonly AppDbContext _context;

        public LoginModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string Email { get; set; } = string.Empty;

        [TempData]
        public string? Message { get; set; }

        public void OnGet() 
        { 
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                Response.Redirect("/Index");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(Email))
            {
                Message = "Email wajib diisi!";
                return Page();
            }
            
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == Email);

            if (user == null)
            {
                Message = "Email tidak ditemukan di sistem!";
                return Page();
            }

            var claims = new List<System.Security.Claims.Claim>
            {
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, user.Email ?? "User"),
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, user.Role ?? "Pegawai") 
            };

            var claimsIdentity = new System.Security.Claims.ClaimsIdentity(
                claims, 
                CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme, 
                new System.Security.Claims.ClaimsPrincipal(claimsIdentity),
                authProperties
            );

            return RedirectToPage("/Index");
        }
    }
}