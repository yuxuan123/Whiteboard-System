using System;
using System.ComponentModel.DataAnnotations;

namespace WhiteboardAPI.Entities
{
    public class UserDE
    {
        [Key]
        public Guid UserId { get; set; }

        [Required]
        [MaxLength(255)]
        public string UserName { get; set; }

        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }

        [Required]
        [MaxLength(50)]
        public string Email { get; set; }

        [Required]
        [MaxLength(50)]
        public string PhoneNo { get; set; }

        [Required]
        public string Role { get; set; }
    }
}
