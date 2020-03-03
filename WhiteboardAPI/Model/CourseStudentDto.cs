using System;
using System.ComponentModel.DataAnnotations;

namespace WhiteboardAPI.Model
{
    public class CourseStudentDto
    {
        public Guid Id { get; set; }

        [Required]
        public Guid CourseId { get; set; }

        [Required]
        public Guid StudentId { get; set; }
        public bool IsActive { get; set; }
    }
}
