using System;
using System.ComponentModel.DataAnnotations;

namespace WhiteboardAPI.Entities
{
    public class PostFolderDE
    {
        [Key]
        public Guid PostFolderId { get; set; }
        public Guid PostId { get; set; }
        public Guid CourseFolderId { get; set; }
    }
}
