using System;
using System.ComponentModel.DataAnnotations;

namespace WhiteboardAPI.Entities
{
    public class ChatDE
    {
        [Key]
        public Guid ChatId { get; set; }
        public Guid LectureId { get; set; }
        public string UserName { get; set; }
        public string Message { get; set; }
        public DateTime DateTime { get; set; }
    }
}
