using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RoomBookingCore.Data;
using RoomBookingCore.Models;

namespace RoomBookingCore.Pages.Departments
{
    public class EditModel : PageModel
    {
        private readonly AppDbContext _context;

        public EditModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Department Department { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            var department = await _context.Departments.FirstOrDefaultAsync(m => m.DepartmentId == id);
            if (department == null) return NotFound();

            Department = department;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            _context.Attach(Department).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Departments.Any(e => e.DepartmentId == Department.DepartmentId)) return NotFound();
                else throw;
            }

            return RedirectToPage("./Index");
        }
    }
}