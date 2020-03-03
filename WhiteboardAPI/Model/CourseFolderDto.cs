using System;
namespace WhiteboardAPI.Model
{
    public class CourseFolderDto
    {
        public Guid CourseFolderId { get; set; }
        public Guid CourseId { get; set; }
        public string Name { get; set; }
    }
}
