using System;
namespace WhiteboardAPI.Entities
{
    public class CourseStudentDE
    {
        public Guid Id { get; set; }
        public Guid CourseId { get; set; }
        public Guid StudentId { get; set; }
        public bool IsActive { get; set; }
    }
}
