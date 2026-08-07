using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoomBookingCore.Data;
using RoomBookingCore.Models;

namespace RoomBookingCore.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class BookingsApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BookingsApiController(AppDbContext context)
        {
            _context = context;
        }

        // 1. GET /api/v1/dashboard/summary
        [HttpGet("dashboard/summary")]
        public async Task<IActionResult> GetDashboardSummary()
        {
            var totalRooms = await _context.Rooms.CountAsync();
            var totalBookings = await _context.Bookings.CountAsync();
            var pendingBookings = await _context.Bookings.CountAsync(b => b.Status == "Pending");
            var approvedBookings = await _context.Bookings.CountAsync(b => b.Status == "Approved");
            var rejectedBookings = await _context.Bookings.CountAsync(b => b.Status == "Rejected");

            return Ok(new
            {
                TotalRooms = totalRooms,
                TotalBookings = totalBookings,
                Pending = pendingBookings,
                Approved = approvedBookings,
                Rejected = rejectedBookings
            });
        }

        // 2. GET /api/v1/bookings/calendar
        [HttpGet("bookings/calendar")]
        public async Task<IActionResult> GetCalendarBookings()
        {
            var bookings = await _context.Bookings
                .Include(b => b.Room)
                .Include(b => b.User)
                .Where(b => b.Status == "Approved")
                .Select(b => new
                {
                    Id = b.BookingId,
                    Title = b.Title,
                    RoomName = b.Room != null ? b.Room.RoomName : "-",
                    Start = b.StartTime,
                    End = b.EndTime,
                    UserEmail = b.User != null ? b.User.Email : "-"
                })
                .ToListAsync();

            return Ok(bookings);
        }

        // 3. POST /api/v1/bookings 
        [HttpPost("bookings")]
        public async Task<IActionResult> CreateBooking([FromBody] BookingCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validasi 
            var isConflict = await _context.Bookings.AnyAsync(b => 
                b.RoomId == dto.RoomId && 
                b.Status == "Approved" && 
                dto.StartTime < b.EndTime && 
                dto.EndTime > b.StartTime
            );

            if (isConflict)
            {
                return BadRequest(new { message = "Jadwal pada ruangan dan jam tersebut sudah dibooking orang lain." });
            }

            var booking = new Booking
            {
                RoomId = dto.RoomId,
                UserId = dto.UserId,
                DepartmentId = dto.DepartmentId,
                Title = dto.Title,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                Status = "Pending"
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return StatusCode(201, new { message = "Peminjaman berhasil diajukan.", data = booking });
        }

        // 4. PUT /api/v1/bookings/{id}/status
        [HttpPut("bookings/{id}/status")]
        public async Task<IActionResult> UpdateBookingStatus(int id, [FromBody] StatusUpdateDto model)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound(new { message = "Data peminjaman tidak ditemukan." });
            }

            booking.Status = model.Status; // "Approved" atau "Rejected"
            if (model.Status == "Rejected")
            {
                booking.RejectionReason = model.RejectionReason ?? "Tidak ada alasan.";
            }
            else
            {
                booking.RejectionReason = null;
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = $"Status peminjaman berhasil diubah menjadi {model.Status}." });
        }

        // 5. GET /api/v1/reports/export/excel
        [HttpGet("reports/export/excel")]
        public async Task<IActionResult> ExportExcelReport()
        {
            var bookings = await _context.Bookings
                .Include(b => b.Room)
                .Include(b => b.User)
                .Select(b => new { b.BookingId, b.Title, b.Status, Room = b.Room != null ? b.Room.RoomName : "" })
                .ToListAsync();

            return Ok(new { message = "Endpoint export excel aktif", count = bookings.Count });
        }

        // 6. GET /api/v1/rooms
        [HttpGet("rooms")]
        public async Task<IActionResult> GetRooms()
        {
            var rooms = await _context.Rooms.ToListAsync();
            return Ok(rooms);
        }
    }

    // DTO Models Payload API
    public class BookingCreateDto
    {
        public int RoomId { get; set; }
        public int UserId { get; set; }
        public int DepartmentId { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }

    public class StatusUpdateDto
    {
        public string Status { get; set; } = string.Empty;
        public string? RejectionReason { get; set; }
    }
}