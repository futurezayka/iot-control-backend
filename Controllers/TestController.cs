using Microsoft.AspNetCore.Mvc;

namespace IotControlService.Controllers;

[ApiController]
public class TestController : ControllerBase
{
    [HttpGet("api/test")]
    public IActionResult Get()
    {
        return Ok("Hello World");
    }
}