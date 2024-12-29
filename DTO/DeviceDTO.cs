using IotControlService.Models;

namespace IotControlService.DTO
{
    public class DeviceDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public DeviceType Type { get; set; }
        public DeviceStatus Status { get; set; }

        public Device ToDevice()
        {
            return new Device
            {
                Id = Id,
                Name = Name,
                Type = Type,
                Status = Status,
            };
        }

        public static DeviceDTO FromDevice(Device device)
        {
            return new DeviceDTO
            {
                Id = device.Id,
                Name = device.Name,
                Type = device.Type,
                Status = device.Status,
            };
        }
    }

}
