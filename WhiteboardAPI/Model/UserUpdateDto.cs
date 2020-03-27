using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WhiteboardAPI.Model
{
    public class UserUpdateDto
    {
        [Display(Name = "Username")]
        [StringLength(50, ErrorMessage = "{0} must be at least {2} characters long.", MinimumLength = 2)]
        public string UserName { get; set; }

        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [RegularExpression("^(?!0+$)(\\+\\d{1,3}[- ]?)?(?!0+$)\\d{8}$", ErrorMessage = "Please enter valid phone no. Needs to be at least 8 numbers.")]
        [Phone]
        public string PhoneNo { get; set; }

        public List<Guid> CourseIds { get; set; }
    }

    public class UserUpdatePasswordDto
    {
        public string Username { get; set; }

        public string Email { get; set; }

        public string NewPassword { get; set; }

        public string OldPassword { get; set; }
    }
}
