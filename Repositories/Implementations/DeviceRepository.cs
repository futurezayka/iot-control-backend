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
        return await DbContext.Devices.Where(d => d.UserId == userId).ToListAsync();
    }
    
}