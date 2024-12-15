namespace IotControlService.Models;

public class DeviceData: BaseEntity
{
    public Device? Device { get; set; }
    public Guid DeviceId { get; set; }
    public double Telemetry { get; set; }
    public DateTime Date { get; set; }
}