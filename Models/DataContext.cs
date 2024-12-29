using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IotControlService.Models;

public class DataContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
{
    public DataContext(DbContextOptions<DataContext> opts)
        : base(opts) { }

    public DbSet<Device> Devices => Set<Device>();
    public DbSet<DeviceData> DeviceData => Set<DeviceData>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Device>()
            .HasOne(d => d.User)
            .WithMany(u => u.Devices)
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}