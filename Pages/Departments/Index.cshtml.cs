using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RoomBookingCore.Data;
using RoomBookingCore.Models;

namespace RoomBookingCore.Pages.Departments
{
    [Authorize(Roles = "SuperUser,Admin")]
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;
        
        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public IList<Department> Departments { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Departments = await _context.Departments.ToListAsync();
        }
    }
}