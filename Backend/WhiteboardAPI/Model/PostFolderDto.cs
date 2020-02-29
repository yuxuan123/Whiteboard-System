using System;
namespace WhiteboardAPI.Model
{
    public class PostFolderDto
    {
        public Guid PostFolderId { get; set; }
        public Guid PostId { get; set; }
        public Guid CourseFolderId { get; set; }
    }
}
