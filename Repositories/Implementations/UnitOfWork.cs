using IotControlService.Models;
using IotControlService.Repositories.Interfaces;

namespace IotControlService.Repositories.Implementations;

public class UnitOfWork : IUnitOfWork
{
    private readonly DataContext _dbContext;

    private DeviceDataRepository? _deviceDataRepository;
    private DeviceRepository? _deviceRepository;

    public DeviceDataRepository DeviceDataRepository => _deviceDataRepository ??= new DeviceDataRepository(_dbContext);
    public DeviceRepository DeviceRepository => _deviceRepository ??= new DeviceRepository(_dbContext);

    public UnitOfWork(DataContext context) => _dbContext = context;

    public async Task SaveAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}