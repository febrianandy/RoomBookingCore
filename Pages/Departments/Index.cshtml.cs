using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RoomBookingCore.Data;
using RoomBookingCore.Models;

namespace RoomBookingCore.Pages.Departments
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;
        
        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public IList<Department> Departments { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            if (!User.IsInRole("SuperUser") && !User.IsInRole("Admin"))
            {
                return RedirectToPage("/Index");
            }

            Departments = await _context.Departments.ToListAsync();
            return Page();
        }
    }
}