using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RoomBookingCore.Data;
using RoomBookingCore.Models;

namespace RoomBookingCore.Pages.Rooms
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public IList<Room> RoomList { get; set; } = new List<Room>();

        [BindProperty]
        public Room NewRoom { get; set; } = new Room();

        [TempData]
        public string? Message { get; set; }

        public async Task OnGetAsync()
        {
            RoomList = await _context.Rooms.ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                RoomList = await _context.Rooms.ToListAsync();
                return Page();
            }

            _context.Rooms.Add(NewRoom);
            await _context.SaveChangesAsync();

            Message = "Ruangan baru berhasil ditambahkan!";
            return RedirectToPage();
        }
    }
}