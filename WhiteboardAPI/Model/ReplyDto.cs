using System;
namespace WhiteboardAPI.Model
{
    public class ReplyDto
    {
        public Guid ReplyId { get; set; }
        public Guid PostId { get; set; }
        public string Description { get; set; }
        public string Role { get; set; }
        public string UserName { get; set; }
        public bool isEdited { get; set; }
        public DateTime? CreatedOn { get; set; }
        public Guid CreatedBy { get; set; }
    }
}
