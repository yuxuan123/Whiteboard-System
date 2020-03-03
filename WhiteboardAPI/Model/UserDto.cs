using System;
using System.Collections.Generic;

namespace WhiteboardAPI.Model
{
    public class Users
    {
        public IEnumerable<UserDto> UserDtos { get; set; }
            = new List<UserDto>();
        public int TotalCount { get; set; }
    }


    public class UserDto
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNo { get; set; }
        public string Role { get; set; }
        public string Token { get; set; }
        public List<Guid> CourseIds { get; set; }
    }
}
