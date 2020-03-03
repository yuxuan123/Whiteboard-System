using System;
using System.ComponentModel.DataAnnotations;

namespace WhiteboardAPI.Entities
{
    public class ContentDE
    {
        [Key]
        public Guid ContentId { get; set; }         public Guid CourseId { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Datetime { get; set; }
        public string FileName { get; set; }
        public string url { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid CreatedBy { get; set; }
    }
}
