using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CulinaryCart.CulinaryCartDAL.Models
{
    [Table("Address")]   // matches DB table name
    public class Address
    {
        [Key]
        public int AddressId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required, MaxLength(50)]
        public string HouseNo { get; set; }

        [Required, MaxLength(100)]
        public string Locality { get; set; }

        [MaxLength(100)]
        public string Landmark { get; set; }

        [Required, MaxLength(100)]
        public string City { get; set; }

        [Required, MaxLength(100)]
        public string District { get; set; }

        [Required, MaxLength(10)]
        public string Pincode { get; set; }

        [Required, MaxLength(100)]
        public string State { get; set; }

        // Navigation back to User
        public User User { get; set; }
    }
}