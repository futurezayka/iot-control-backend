using IotControlService.Models;
using IotControlService.Repositories.Implementations;

namespace IotControlService.Repositories.Interfaces;

public interface IUnitOfWork
{
    IRepository<User> UserRepository { get; }
    DeviceDataRepository DeviceDataRepository { get; }
    DeviceRepository DeviceRepository { get; }
    
    Task SaveAsync();
}