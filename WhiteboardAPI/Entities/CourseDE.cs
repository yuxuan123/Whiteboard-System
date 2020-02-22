using System;
using System.ComponentModel.DataAnnotations;

namespace WhiteboardAPI.Entities
{
    public class CourseDE
    {
        [Key]
        public Guid CourseId { get; set; }

        public string CourseCode { get; set; }

        [Required]
        [MaxLength(255)]
        public string CourseName { get; set; }

        [Required]
        [MaxLength(255)]
        public string CourseDescription { get; set; }

        [Required]
        public Guid CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }


    }
}
