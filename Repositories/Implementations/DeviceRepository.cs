using IotControlService.Models;
using Microsoft.EntityFrameworkCore;

namespace IotControlService.Repositories.Implementations;

public class DeviceRepository: Repository<Device>
{
    public DeviceRepository(DataContext context) : base(context)
    {
    }
    
    public async Task<List<Device>> GetAllByUserIdAsync(Guid userId)
    {
        var devices = await DbContext.Devices
            .Include(d => d.DeviceData)
            .Where(d => d.UserId == userId)
            .ToListAsync();
        
        devices.ForEach(d => d.DeviceData = d.DeviceData
            .OrderByDescending(dd => dd.Date)
            .Take(50)
            .ToList());

        return devices;
    }    
}