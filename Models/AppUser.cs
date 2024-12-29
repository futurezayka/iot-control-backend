using Microsoft.AspNetCore.Identity;

namespace IotControlService.Models
{
    public class AppUser : IdentityUser<Guid>
    {
        public List<Device> Devices { get; set; } = new();
    }
}
