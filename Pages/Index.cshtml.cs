using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RoomBookingCore.Data;
using RoomBookingCore.Models;
using Microsoft.AspNetCore.Mvc;

namespace RoomBookingCore.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public int TotalBookingCount { get; set; }
        public int TotalHoursUsed { get; set; }
        public string FavoriteRoom { get; set; } = "-";
        public string ActiveDepartment { get; set; } = "-";

        // Grafik Departemen (untuk Chart)
        public Dictionary<string, int> DeptBookingStats { get; set; } = new();

        public class RoomBlueprintStatus
        {
            public string RoomName { get; set; } = string.Empty;
            public string TimeSlot { get; set; } = string.Empty;
            public bool IsInUse { get; set; }
        }

        public List<RoomBlueprintStatus> InUseRooms { get; set; } = new();
        public List<RoomBlueprintStatus> AvailableRooms { get; set; } = new();

        public async Task OnGetAsync()
        {
            var today = DateTime.Today;
            var now = DateTime.Now;

            TotalBookingCount = await _context.Bookings.CountAsync(b => b.Status == "Approved");

            var allApproved = await _context.Bookings
                .Where(b => b.Status == "Approved")
                .ToListAsync();

            TotalHoursUsed = (int)allApproved.Sum(b => (b.EndTime - b.StartTime).TotalHours);

            var favRoomGroup = allApproved
                .GroupBy(b => b.RoomId)
                .OrderByDescending(g => g.Count())
                .FirstOrDefault();

            if (favRoomGroup != null)
            {
                var roomObj = await _context.Rooms.FindAsync(favRoomGroup.Key);
                FavoriteRoom = roomObj?.RoomName ?? "-";
            }

            var favDeptGroup = allApproved
                .Where(b => b.DepartmentId != null)
                .GroupBy(b => b.DepartmentId)
                .OrderByDescending(g => g.Count())
                .FirstOrDefault();

            if (favDeptGroup != null)
            {
                var deptObj = await _context.Departments.FindAsync(favDeptGroup.Key);
                ActiveDepartment = deptObj?.DepartmentName ?? "-";
            }

            var departments = await _context.Departments.ToListAsync();
            foreach (var dept in departments)
            {
                int count = allApproved.Count(b => b.DepartmentId == dept.DepartmentId);
                DeptBookingStats[dept.DepartmentName] = count;
            }


            var rooms = await _context.Rooms.ToListAsync();
            var todayBookings = allApproved.Where(b => b.StartTime.Date == today).ToList();

            foreach (var room in rooms)
            {
                var activeBooking = todayBookings.FirstOrDefault(b => 
                    b.RoomId == room.RoomId && 
                    now >= b.StartTime && 
                    now <= b.EndTime);

                if (activeBooking != null)
                {
                    InUseRooms.Add(new RoomBlueprintStatus
                    {
                        RoomName = room.RoomName,
                        TimeSlot = $"{activeBooking.StartTime:HH:mm} - {activeBooking.EndTime:HH:mm}",
                        IsInUse = true
                    });
                }
                else
                {
                    AvailableRooms.Add(new RoomBlueprintStatus
                    {
                        RoomName = room.RoomName,
                        TimeSlot = "Tersedia",
                        IsInUse = false
                    });
                }
            }
        }

        public async Task<IActionResult> OnGetRoomStatusAsync()
        {
            var today = DateTime.Today;
            var now = DateTime.Now;

            var rooms = await _context.Rooms.ToListAsync();
            var allApproved = await _context.Bookings
                .Where(b => b.Status == "Approved" && b.StartTime.Date == today)
                .ToListAsync();

            var inUseList = new List<object>();
            var availableList = new List<object>();

            foreach (var room in rooms)
            {
                var activeBooking = allApproved.FirstOrDefault(b => 
                    b.RoomId == room.RoomId && 
                    now >= b.StartTime && 
                    now <= b.EndTime);

                if (activeBooking != null)
                {
                    inUseList.Add(new { 
                        roomName = room.RoomName, 
                        timeSlot = $"{activeBooking.StartTime:HH:mm} - {activeBooking.EndTime:HH:mm}" 
                    });
                }
                else
                {
                    availableList.Add(new { 
                        roomName = room.RoomName 
                    });
                }
            }

            return new JsonResult(new { inUse = inUseList, available = availableList });
        }
    }
}