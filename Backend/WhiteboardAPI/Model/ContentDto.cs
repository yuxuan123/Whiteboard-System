using System;
using System.ComponentModel.DataAnnotations;

namespace WhiteboardAPI.Model
{
    public class ContentDto
    {
        public Guid ContentId { get; set; }

        [Required]
        public Guid CourseId { get; set; }

        [Required]
        public string Type { get; set; }

        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Datetime { get; set; }
        public string FileName { get; set; }
        public string url { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid CreatedBy { get; set; }
    }
}
