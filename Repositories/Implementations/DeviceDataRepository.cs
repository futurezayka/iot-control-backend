using IotControlService.Models;
using Microsoft.EntityFrameworkCore;

namespace IotControlService.Repositories.Implementations;

public class DeviceDataRepository : Repository<DeviceData>
{
    public DeviceDataRepository(DataContext context) : base(context)
    {
    }

    public async Task<List<DeviceData>> GetAllByDeviceIdAsync(Guid deviceId)
    {
        return await DbContext.DeviceData.Where(d => d.DeviceId == deviceId).ToListAsync();
    }
}