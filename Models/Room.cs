using System.ComponentModel.DataAnnotations;

namespace RoomBookingCore.Models
{
    public class Room
    {
        [Key]
        public int RoomId { get; set; }

        [Required(ErrorMessage = "Nama ruangan wajib diisi.")]
        public string RoomName { get; set; }

        [Required(ErrorMessage = "Kapasitas wajib diisi.")]
        public int Capacity { get; set; }

        public string? Facilities { get; set; }

        // Tambahkan tanda tanya (?) di sini agar tidak wajib diisi saat binding form
        public virtual ICollection<Booking>? Bookings { get; set; }
    }
}