using System;
using System.ComponentModel.DataAnnotations;

namespace WhiteboardAPI.Entities
{
    public class CourseFolderDE
    {
        [Key]
        public Guid CourseFolderId { get; set; }
        public Guid CourseId { get; set; }
        public string Name { get; set; }
    }
}
