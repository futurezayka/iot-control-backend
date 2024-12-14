using System.ComponentModel.DataAnnotations;

namespace IotControlService.Models;

public class User: BaseEntity
{
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public UserRole Role { get; set; }
}