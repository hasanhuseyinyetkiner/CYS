using CYS.Models.DTOs;
using CYS.Repos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CYS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeaningWeightMeasurementController : ControllerBase
    {
        private readonly WeaningWeightMeasurementRepository _repository;

        public WeaningWeightMeasurementController(WeaningWeightMeasurementRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<WeaningWeightMeasurementDTO>>> GetAllWeaningWeightMeasurements()
        {
            try
            {
                return Ok(await _repository.GetAllWeaningWeightMeasurementsAsync());
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<WeaningWeightMeasurementDTO>> GetWeaningWeightMeasurementById(int id)
        {
            try
            {
                var measurement = await _repository.GetWeaningWeightMeasurementByIdAsync(id);
                if (measurement == null)
                {
                    return NotFound($"Weaning weight measurement with ID {id} not found");
                }
                return Ok(measurement);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("animal/{animalId}")]
        public async Task<ActionResult<IEnumerable<WeaningWeightMeasurementDTO>>> GetWeaningWeightMeasurementsByAnimalId(int animalId)
        {
            try
            {
                return Ok(await _repository.GetWeaningWeightMeasurementsByAnimalIdAsync(animalId));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("rfid/{rfid}")]
        public async Task<ActionResult<IEnumerable<WeaningWeightMeasurementDTO>>> GetWeaningWeightMeasurementsByRfid(string rfid)
        {
            try
            {
                return Ok(await _repository.GetWeaningWeightMeasurementsByRfidAsync(rfid));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<int>> AddWeaningWeightMeasurement(WeaningWeightMeasurementDTO measurement)
        {
            try
            {
                var id = await _repository.AddWeaningWeightMeasurementAsync(measurement);
                return CreatedAtAction(nameof(GetWeaningWeightMeasurementById), new { id }, id);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateWeaningWeightMeasurement(int id, WeaningWeightMeasurementDTO measurement)
        {
            try
            {
                var success = await _repository.UpdateWeaningWeightMeasurementAsync(id, measurement);
                if (!success)
                {
                    return NotFound($"Weaning weight measurement with ID {id} not found");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWeaningWeightMeasurement(int id)
        {
            try
            {
                var success = await _repository.DeleteWeaningWeightMeasurementAsync(id);
                if (!success)
                {
                    return NotFound($"Weaning weight measurement with ID {id} not found");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("mobile/weaning-weight")]
        public async Task<IActionResult> AddMobileWeaningWeightMeasurement([FromBody] WeaningWeightMeasurementDTO measurement)
        {
            try
            {
                var id = await _repository.AddWeaningWeightMeasurementAsync(measurement);
                return Ok(new { success = true, message = "Weaning weight measurement saved successfully", id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }
    }
} 