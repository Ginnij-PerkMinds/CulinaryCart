using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CulinaryCart.CulinaryCartDAL.Models
{
    [Table("Users")]   
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        [Required, MaxLength(100)]
        public string EmailId { get; set; }

        [Required, MaxLength(256)]
        public string PasswordHash { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }

        public bool IsActive { get; set; }
        public bool IsAdmin { get; set; }

        [MaxLength(15)]
        [Column("PhoneNo")]   
        public string? PhoneNo { get; set; }

        [MaxLength(250)]
        [Column("Profilepic")]  
        public string? ProfilePic { get; set; }

        // Navigation
        public Address Address { get; set; }
    }
}



