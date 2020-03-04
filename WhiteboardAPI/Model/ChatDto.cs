using System;
namespace WhiteboardAPI.Model
{
    public class ChatDto
    {
        public Guid LectureId { get; set; }
        public string UserName { get; set; }
        public string Message { get; set; }
        public DateTime DateTime { get; set; }
    }
}
