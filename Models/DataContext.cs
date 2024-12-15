using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IotControlService.Models;

public class DataContext : IdentityDbContext<IdentityUser>

{
    public DataContext(DbContextOptions<DataContext> opts) 
        : base(opts) { }
    
    public DbSet<User> Users => Set<User>();
    public DbSet<Device> Devices => Set<Device>();
    public DbSet<DeviceData> DeviceData => Set<DeviceData>();
}