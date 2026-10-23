using Microsoft.EntityFrameworkCore;
using IF133.Tugas1.Api.Models;

namespace IF133.Tugas1.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<Book> Books { get; set; } = null!;
    public DbSet<Customer> Customers { get; set; } = null!;
    public DbSet<TaskItem> TaskItems { get; set; } = null!;
    public DbSet<Event> Events { get; set; } = null!;
}