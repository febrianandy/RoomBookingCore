using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RoomBookingCore.Data;
using RoomBookingCore.Models;
using System.ComponentModel.DataAnnotations;

namespace RoomBookingCore.Pages
{
    public class BookingFormModel : PageModel
    {
        private readonly AppDbContext _context;

        public BookingFormModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public SelectList RoomList { get; set; } = default!;

        public class InputModel
        {
            [Required(ErrorMessage = "Pilih ruangan terlebih dahulu.")]
            [Display(Name = "Ruangan")]
            public int RoomId { get; set; }

            [Required(ErrorMessage = "Waktu mulai wajib diisi.")]
            [Display(Name = "Waktu Mulai")]
            public DateTime StartTime { get; set; } = 
                new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0);

            [Required(ErrorMessage = "Waktu selesai wajib diisi.")]
            [Display(Name = "Waktu Selesai")]
            public DateTime EndTime { get; set; } = 
                new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0).AddHours(1);

            [Required(ErrorMessage = "Keperluan rapat wajib diisi.")]
            [Display(Name = "Keperluan / Acara")]
            public string Title { get; set; } = string.Empty;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadRoomsDropdownAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Mohon periksa kembali form yang Anda isi.";
                await LoadRoomsDropdownAsync();
                return Page();
            }

            var userEmail = User.Identity?.Name;
            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToPage("/Account/Login");
            }

            var currentUser = await _context.Users
                .Include(u => u.Department)
                .FirstOrDefaultAsync(u => u.Email == userEmail);

            if (currentUser == null)
            {
                TempData["ErrorMessage"] = "Data user yang sedang login tidak ditemukan di database.";
                await LoadRoomsDropdownAsync();
                return Page();
            }

            try
            {
                var booking = new Booking
                {
                    RoomId = Input.RoomId,
                    StartTime = Input.StartTime,
                    EndTime = Input.EndTime,
                    Title = Input.Title,
                    UserId = currentUser.UserId,
                    DepartmentId = currentUser.DepartmentId,
                    Status = "Pending"
                };

                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();

                // Pesan Sukses
                TempData["SuccessMessage"] = "Peminjaman ruangan berhasil diajukan dan sedang menunggu persetujuan!";
                return RedirectToPage("./BookingForm");
            }
            catch (Exception)
            {
                // Pesan Gagal jika terjadi error database
                TempData["ErrorMessage"] = "Terjadi kesalahan sistem saat menyimpan data peminjaman.";
                await LoadRoomsDropdownAsync();
                return Page();
            }
        }

        private async Task LoadRoomsDropdownAsync()
        {
            var rooms = await _context.Rooms.ToListAsync();
            RoomList = new SelectList(rooms, "RoomId", "RoomName");
        }
    }
}