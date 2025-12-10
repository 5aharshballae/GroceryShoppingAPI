using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

	public DbSet<User> Users { get; set; }
	public DbSet<Store> Stores { get; set; }
	public DbSet<Category> Categories { get; set; }
	public DbSet<Product> Products { get; set; }
	public DbSet<Cart> Carts { get; set; }
	public DbSet<CartItem> CartItems { get; set; }
	public DbSet<Order> Orders { get; set; }
	public DbSet<OrderItem> OrderItems { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.Entity<User>(entity =>
		{
			entity.HasKey(e => e.Id);
			entity.HasIndex(e => e.Email).IsUnique();
			entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
			entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
			entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
		});

		modelBuilder.Entity<Store>(entity =>
		{
			entity.HasKey(e => e.Id);
			entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
		});

		modelBuilder.Entity<Category>(entity =>
		{
			entity.HasKey(e => e.Id);
			entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
			entity.HasOne(e => e.Store)
				  .WithMany(s => s.Categories)
				  .HasForeignKey(e => e.StoreId)
				  .OnDelete(DeleteBehavior.Cascade);
		});

		modelBuilder.Entity<Product>(entity =>
		{
			entity.HasKey(e => e.Id);
			entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
			entity.Property(e => e.Price).HasPrecision(18, 2);
			entity.HasOne(e => e.Category)
				  .WithMany(c => c.Products)
				  .HasForeignKey(e => e.CategoryId)
				  .OnDelete(DeleteBehavior.Cascade);
		});

		modelBuilder.Entity<Cart>(entity =>
		{
			entity.HasKey(e => e.Id);
			entity.HasOne(e => e.User)
				  .WithOne(u => u.Cart)
				  .HasForeignKey<Cart>(e => e.UserId)
				  .OnDelete(DeleteBehavior.Cascade);
		});

		modelBuilder.Entity<CartItem>(entity =>
		{
			entity.HasKey(e => e.Id);
			entity.Property(e => e.PriceAtAdd).HasPrecision(18, 2);
			entity.HasOne(e => e.Cart)
				  .WithMany(c => c.Items)
				  .HasForeignKey(e => e.CartId)
				  .OnDelete(DeleteBehavior.Cascade);
			entity.HasOne(e => e.Product)
				  .WithMany()
				  .HasForeignKey(e => e.ProductId)
				  .OnDelete(DeleteBehavior.Restrict);
		});

		modelBuilder.Entity<Order>(entity =>
		{
			entity.HasKey(e => e.Id);
			entity.Property(e => e.OrderNumber).IsRequired().HasMaxLength(50);
			entity.HasIndex(e => e.OrderNumber).IsUnique();
			entity.Property(e => e.TotalAmount).HasPrecision(18, 2);
			entity.HasOne(e => e.User)
				  .WithMany(u => u.Orders)
				  .HasForeignKey(e => e.UserId)
				  .OnDelete(DeleteBehavior.Cascade);
		});

		modelBuilder.Entity<OrderItem>(entity =>
		{
			entity.HasKey(e => e.Id);
			entity.Property(e => e.UnitPrice).HasPrecision(18, 2);
			entity.Property(e => e.Subtotal).HasPrecision(18, 2);
			entity.HasOne(e => e.Order)
				  .WithMany(o => o.Items)
				  .HasForeignKey(e => e.OrderId)
				  .OnDelete(DeleteBehavior.Cascade);
		});
	}
}