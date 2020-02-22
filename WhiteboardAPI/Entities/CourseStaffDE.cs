using System;
namespace WhiteboardAPI.Entities
{
    public class CourseStaffDE
    {
        public Guid Id { get; set; }
        public Guid CourseId { get; set; }
        public Guid StaffId { get; set; }
        public bool IsActive { get; set; }
    }
}
