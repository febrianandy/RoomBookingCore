using System.ComponentModel.DataAnnotations;

namespace RoomBookingCore.Models
{
    public class Department
    {
        [Key]
        public int DepartmentId { get; set; }

        [Required(ErrorMessage = "Nama departemen wajib diisi.")]
        [Display(Name = "Nama Departemen")]
        [StringLength(100)]
        public string DepartmentName { get; set; } = string.Empty;
    }
}