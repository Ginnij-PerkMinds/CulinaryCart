namespace CulinaryCart.CulinaryCartDAL.Models;

    public class User
    {
    public int UserId { get; set; }
    public string Name { get; set; }
    public string EmailId { get; set; }
    public string PasswordHash { get; set; }

    public DateTimeOffset CreatedAt { get; set; }   // default GETDATE()
    public DateTimeOffset? UpdatedAt { get; set; }  // nullable, set on update
    public bool IsActive { get; set; }        // default true
    public bool IsAdmin { get; set; }         // default false

}


