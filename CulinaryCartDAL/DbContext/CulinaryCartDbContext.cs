namespace CulinaryCart.CulinaryCartDAL.DbContext;

using CulinaryCart.CulinaryCartDAL.Models;
using Microsoft.EntityFrameworkCore;

public class CulinaryCartDbContext : DbContext
{
    public CulinaryCartDbContext(DbContextOptions<CulinaryCartDbContext> options) : base(options) { }

    public DbSet<Menu> Menu { get; set; }
    public DbSet<OrderHistory> OrderHistory { get; set; }
    public DbSet<Category> Category { get; set; }       
    public DbSet<DietaryPreference> DietaryPreference { get; set; }  
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<User> Users { get; set; }   //added for user management
    public DbSet<Address> Address { get; set; }    // Address management
    public DbSet<RevokedToken> RevokedTokens { get; set; }    //logout
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Promocode> Promocode { get; set; } 
    public DbSet<Charge> Charge { get; set; }
    public DbSet<Refund> Refunds { get; set; }  // added for refund management
    public DbSet<RefundItem> RefundItems { get; set; }
    public DbSet<OtpEntry> OtpStore { get; set; }





    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>().ToTable("Categories");
        modelBuilder.Entity<DietaryPreference>().ToTable("DietaryPreference");
        modelBuilder.Entity<Menu>().ToTable("Menu");
        modelBuilder.Entity<OrderHistory>().ToTable("OrderHistory");
        modelBuilder.Entity<CartItem>().ToTable("CartItems");
        modelBuilder.Entity<User>().ToTable("Users");  // added for user management
        modelBuilder.Entity<Address>().ToTable("Address");  //added

        // Defining primary keys:
        modelBuilder.Entity<Category>().HasKey(c => c.CategoryId);
        modelBuilder.Entity<DietaryPreference>().HasKey(d => d.DietId);
        modelBuilder.Entity<Menu>().HasKey(m => m.FoodItemID);
        modelBuilder.Entity<OrderHistory>().HasKey(o => o.HistoryID);
        modelBuilder.Entity<CartItem>().HasKey(c => c.CartItemId);
        modelBuilder.Entity<User>().HasKey(u => u.UserId);  // added for user management
        modelBuilder.Entity<Address>().HasKey(a => a.AddressId);  // added

        // Relationships
        modelBuilder.Entity<Menu>()
            .HasOne(m => m.Category)
            .WithMany(c => c.MenuItems)
            .HasForeignKey(m => m.CategoryId);

        modelBuilder.Entity<Menu>()
            .HasOne(m => m.DietaryPreference)
            .WithMany(d => d.MenuItems)
            .HasForeignKey(m => m.DietId);

        modelBuilder.Entity<Menu>()
            .Property(m => m.RemainingQuantity)
            .HasDefaultValue(50);

        // this things are added 
        modelBuilder.Entity<User>()
              .HasOne(u => u.Address)
              .WithOne(a => a.User)
              .HasForeignKey<Address>(a => a.UserId)
              .OnDelete(DeleteBehavior.Cascade);

        // Unique Email constraint
        modelBuilder.Entity<User>()
            .HasIndex(u => u.EmailId)
            .IsUnique();

        // Order   
        modelBuilder.Entity<Order>()
            .HasMany(o => o.OrderItems)
            .WithOne(i => i.Order)
            .HasForeignKey(i => i.OrderId);
      
        // Charges
        modelBuilder.Entity<Charge>().ToTable("Charges");
        modelBuilder.Entity<Charge>().HasKey(c => c.ChargeId);

        modelBuilder.Entity<Charge>()
            .Property(c => c.ChargeType)
            .IsRequired()
            .HasMaxLength(100);

        modelBuilder.Entity<Charge>()
            .Property(c => c.Value)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        modelBuilder.Entity<Charge>()
            .Property(c => c.IsActive)
            .HasDefaultValue(false);

        // Refunds
        modelBuilder.Entity<Refund>()
               .HasOne(r => r.Order)
               .WithMany() // or WithOne if you want one refund per order
               .HasForeignKey(r => r.OrderId);

        modelBuilder.Entity<Refund>()
            .HasOne(r => r.User)
            .WithMany()
            .HasForeignKey(r => r.UserId);

        modelBuilder.Entity<Refund>()
            .HasMany(r => r.RefundItems)
            .WithOne(ri => ri.Refund)
            .HasForeignKey(ri => ri.RefundId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RefundItem>()
            .HasOne(ri => ri.Menu)
            .WithMany() // if you don’t want a collection of RefundItems inside Menu
            .HasForeignKey(ri => ri.FoodItemID);
    }
}