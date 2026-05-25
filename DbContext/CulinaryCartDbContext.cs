namespace CulinaryCart.DbContext;
using CulinaryCart.Model;
using Microsoft.EntityFrameworkCore;

public class CulinaryCartDbContext : DbContext
{
    public CulinaryCartDbContext(DbContextOptions<CulinaryCartDbContext> options) : base(options) { }

    public DbSet<Menu> Menu { get; set; }
    public DbSet<OrderHistory> OrderHistory { get; set; }
}

