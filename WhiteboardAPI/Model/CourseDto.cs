using System;
using System.Collections.Generic;

namespace WhiteboardAPI.Model
{
    public class CourseDto
    {
        public Guid CourseId { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public string CourseDescription { get; set; }
        public IEnumerable<Guid> Staff { get; set; }
        public IEnumerable<Guid> Students { get; set; }
        public IEnumerable<Guid> Contents { get; set; }
        public IEnumerable<CourseFolderDto> CourseFolders { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}