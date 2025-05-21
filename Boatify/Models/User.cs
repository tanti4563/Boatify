using System;
using System.ComponentModel.DataAnnotations;

namespace ChuyenDI.Models
{
    public class User
    {
        public int UserId { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [StringLength(50)]
        public string FirstName { get; set; }

        [StringLength(50)]
        public string LastName { get; set; }

        [StringLength(15)]
        public string PhoneNumber { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}