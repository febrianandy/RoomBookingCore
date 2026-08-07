using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RoomBookingCore.Data;
using RoomBookingCore.Models;

namespace RoomBookingCore.Services
{
    public class BookingService
    {
        private readonly AppDbContext _context; 

        public BookingService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(bool Success, string Message)> CreateBookingAsync(Booking newBooking)
        {
            if (newBooking.StartTime >= newBooking.EndTime)
            {
                return (false, "Waktu selesai harus lebih besar daripada waktu mulai.");
            }

            bool isCollision = await _context.Bookings.AnyAsync(b =>
                b.RoomId == newBooking.RoomId &&
                (b.Status == "Approved" || b.Status == "Pending") &&
                newBooking.StartTime < b.EndTime &&
                newBooking.EndTime > b.StartTime
            );

            if (isCollision)
            {
                return (false, "Collision Detected: Ruangan sudah dibooking pada rentang waktu tersebut.");
            }

            newBooking.Status = "Pending";
            _context.Bookings.Add(newBooking);
            await _context.SaveChangesAsync();

            return (true, "Pengajuan berhasil dibuat.");
        }
    }
}