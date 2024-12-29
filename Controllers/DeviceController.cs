using System.Security.Claims;
using IotControlService.DTO;
using IotControlService.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IotControlService.Controllers
{
    [Route("api/devices")]
    [Authorize]
    [ApiController]
    public class DeviceController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeviceController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAllByUserId()
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);;
            var devices = await _unitOfWork.DeviceRepository.GetAllByUserIdAsync(userId);
            return Ok(devices);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var device = await _unitOfWork.DeviceRepository.GetByIdAsync(id);
            if (device == null)
            {
                return NotFound();
            }
            return Ok(device);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DeviceDTO deviceDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var device = deviceDTO.ToDevice();
            device.UserId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            await _unitOfWork.DeviceRepository.AddAsync(device);
            await _unitOfWork.SaveAsync();
            return Ok(device);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] DeviceDTO deviceDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingDevice = await _unitOfWork.DeviceRepository.GetByIdAsync(id);

            if (existingDevice == null)
            {
                return NotFound();
            }

            existingDevice.Name = deviceDTO.Name;
            existingDevice.Type = deviceDTO.Type;
            existingDevice.Status = deviceDTO.Status;

            _unitOfWork.DeviceRepository.Update(existingDevice);
            await _unitOfWork.SaveAsync();

            return Ok(existingDevice);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var device = await _unitOfWork.DeviceRepository.GetByIdAsync(id);
            if (device == null)
            {
                return NotFound();
            }
            _unitOfWork.DeviceRepository.Remove(device);
            await _unitOfWork.SaveAsync();
            return Ok(device);
        }
    }
}
