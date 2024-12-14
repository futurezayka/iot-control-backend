namespace IotControlService.Models;

public class Device: BaseEntity
{
    public string Name { get; set; }
    public User User { get; set; }
    public Guid UserId { get; set; }
    public DeviceType Type { get; set; }
    public DeviceStatus Status { get; set; }
    public List<DeviceData> DeviceData { get; set; }
}