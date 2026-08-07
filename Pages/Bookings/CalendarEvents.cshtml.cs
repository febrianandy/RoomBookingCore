using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RoomBookingCore.Data;

namespace RoomBookingCore.Pages.Bookings
{
    public class CalendarModel : PageModel
    {
        private readonly AppDbContext _context;

        public CalendarModel(AppDbContext context)
        {
            _context = context;
        }

        public void OnGet()
        {
            
        }

        public async Task<IActionResult> OnGetCalendarEventsAsync()
        {
            var bookings = await _context.Bookings
                .Include(b => b.Room)
                .Include(b => b.User)
                .Where(b => b.Status == "Approved")
                .ToListAsync();

            var events = bookings.Select(b => new
            {
                id = b.BookingId,
                title = $"{b.Title} ({b.Room?.RoomName ?? "Ruangan"})",
                start = b.StartTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                end = b.EndTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                backgroundColor = GetRoomColor(b.RoomId),
                borderColor = GetRoomColor(b.RoomId),
                extendedProps = new
                {
                    roomName = b.Room?.RoomName,
                    userName = b.User?.Email,
                    purpose = b.Title
                }
            });

            return new JsonResult(events);
        }

        private string GetRoomColor(int roomId)
        {
            string[] colors = { "#0d6efd", "#198754", "#ffc107", "#fc0720", "#6f42c1", "#fd7e14" };
            return colors[roomId % colors.Length];
        }
    }
}