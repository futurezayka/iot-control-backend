using System.Text.Json.Serialization;

namespace IotControlService.Models;

public class Device: BaseEntity
{
    public string Name { get; set; } = null!;
    public Guid UserId { get; set; }
    public DeviceType Type { get; set; }
    public DeviceStatus Status { get; set; }
    public List<DeviceData> DeviceData { get; set; } = new();

    [JsonIgnore]
    public AppUser User { get; set; } = null!;
}