using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RoomBookingCore.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Role { get; set; }
        
        public int DepartmentId { get; set; }
        public Department Department { get; set; }
        
        public ICollection<Booking> Bookings { get; set; }
    }
}