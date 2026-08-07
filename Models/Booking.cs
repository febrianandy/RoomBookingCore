using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoomBookingCore.Models
{
    public class Booking
    {
        [Key]
        public int BookingId { get; set; }

        [Required(ErrorMessage = "Judul / Keperluan wajib diisi.")]
        [StringLength(150)]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ruangan wajib dipilih.")]
        public int RoomId { get; set; }

        [ForeignKey("RoomId")]
        public virtual Room? Room { get; set; }

        [Required(ErrorMessage = "User ID wajib diisi.")]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        [Required(ErrorMessage = "Department ID wajib diisi.")]
        public int DepartmentId { get; set; }

        [ForeignKey("DepartmentId")]
        public virtual Department? Department { get; set; }

        [Required(ErrorMessage = "Waktu mulai wajib diisi.")]
        public DateTime StartTime { get; set; }

        [Required(ErrorMessage = "Waktu selesai wajib diisi.")]
        public DateTime EndTime { get; set; }

        [Required]
        public string Status { get; set; } = "Pending";
        [StringLength(250)]
        public string? RejectionReason { get; set; }
    }
}