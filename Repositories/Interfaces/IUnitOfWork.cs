using IotControlService.Models;
using IotControlService.Repositories.Implementations;

namespace IotControlService.Repositories.Interfaces;

public interface IUnitOfWork
{
    DeviceDataRepository DeviceDataRepository { get; }
    DeviceRepository DeviceRepository { get; }
    
    Task SaveAsync();
}