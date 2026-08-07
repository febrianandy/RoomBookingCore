using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RoomBookingCore.Data;
using RoomBookingCore.Models;

namespace RoomBookingCore.Pages.Bookings
{
    [Authorize(Roles = "SuperUser,Admin")]
    public class ApprovalModel : PageModel
    {
        private readonly AppDbContext _context;

        public ApprovalModel(AppDbContext context)
        {
            _context = context;
        }

        public IList<Booking> PendingBookings { get; set; } = default!;

        [TempData]
        public string? StatusMessage { get; set; }

        public async Task OnGetAsync()
        {
            PendingBookings = await _context.Bookings
                .Include(b => b.Room)
                .Include(b => b.User)
                .Include(b => b.Department)
                .Where(b => b.Status == "Pending")
                .OrderBy(b => b.StartTime)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostApproveAsync(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                StatusMessage = "Error: Data peminjaman tidak ditemukan.";
                return RedirectToPage();
            }

            booking.Status = "Approved";
            await _context.SaveChangesAsync();
            StatusMessage = "Peminjaman ruangan berhasil Disetujui.";

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRejectAsync(int id, string reason)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            booking.Status = "Rejected";
            booking.RejectionReason = string.IsNullOrWhiteSpace(reason) ? "Tidak ada alasan yang diberikan." : reason;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Peminjaman berhasil ditolak beserta alasannya.";
            return RedirectToPage();
        }
    }
}