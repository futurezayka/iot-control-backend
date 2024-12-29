using IotControlService.Models;
using IotControlService.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IotControlService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeviceDataController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeviceDataController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("device/{deviceId}")]
        public async Task<IActionResult> GetAllByDeviceId(Guid deviceId)
        {
            var deviceData = await _unitOfWork.DeviceDataRepository.GetAllByDeviceIdAsync(deviceId);
            return Ok(deviceData);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var data = await _unitOfWork.DeviceDataRepository.GetByIdAsync(id);
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DeviceData data)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            await _unitOfWork.DeviceDataRepository.AddAsync(data);
            await _unitOfWork.SaveAsync();
            return Ok(new { id = data.Id });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] DeviceData data)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var existingData = await _unitOfWork.DeviceDataRepository.GetByIdAsync(id);
            if (existingData == null)
            {
                return NotFound();
            }
            existingData.Telemetry = data.Telemetry;
            existingData.Date = data.Date;
            existingData.DeviceId = data.DeviceId;
            _unitOfWork.DeviceDataRepository.Update(existingData);
            await _unitOfWork.SaveAsync();
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var data = await _unitOfWork.DeviceDataRepository.GetByIdAsync(id);
            if (data == null)
            {
                return NotFound();
            }
            _unitOfWork.DeviceDataRepository.Remove(data);
            await _unitOfWork.SaveAsync();
            return Ok();
        }
    }
}
