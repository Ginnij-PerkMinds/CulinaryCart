namespace CulinaryCart.CulinaryCartBAL.Models.DTO
    {
  
    public class UpdateUserDto
        {
            public string Name { get; set; }
            public string EmailId { get; set; }
            public bool IsActive { get; set; }
            public bool IsAdmin { get; set; }
        }

        public class UserDto
        {
            public int UserId { get; set; }
            public string Name { get; set; }
            public string EmailId { get; set; }
            public bool IsActive { get; set; } = true;
            public bool IsAdmin { get; set; } = false;
            public DateTimeOffset CreatedAt { get; set; }
            public DateTimeOffset? UpdatedAt { get; set; }
        }
    }



