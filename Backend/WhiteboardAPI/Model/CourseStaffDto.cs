using System;
using System.ComponentModel.DataAnnotations;

namespace WhiteboardAPI.Model
{
    public class CourseStaffDto
    {
        public Guid Id { get; set; }

        [Required]
        public Guid CourseId { get; set; }

        [Required]
        public Guid StaffId { get; set; }
        public bool IsActive { get; set; }
    }
}
