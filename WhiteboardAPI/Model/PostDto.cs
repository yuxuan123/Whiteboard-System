using System;
using System.Collections.Generic;

namespace WhiteboardAPI.Model
{
    public class PostDto
    {
        public Guid PostId { get; set; }
        public Guid CourseId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<Guid> LecturerReply { get; set; }
        public List<Guid> StudentReply { get; set; }
        public List<Guid> CourseFolderId { get; set; }
        public string UserName { get; set; }
        public bool isEdited { get; set; }
        public DateTime? CreatedOn { get; set; }
        public Guid CreatedBy { get; set; }
    }
}
