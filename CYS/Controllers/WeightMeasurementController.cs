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
    public class WeightMeasurementController : ControllerBase
    {
        private readonly WeightMeasurementRepository _repository;

        public WeightMeasurementController(WeightMeasurementRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<WeightMeasurementDTO>>> GetAllWeightMeasurements()
        {
            try
            {
                return Ok(await _repository.GetAllWeightMeasurementsAsync());
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<WeightMeasurementDTO>> GetWeightMeasurementById(int id)
        {
            try
            {
                var measurement = await _repository.GetWeightMeasurementByIdAsync(id);
                if (measurement == null)
                {
                    return NotFound($"Weight measurement with ID {id} not found");
                }
                return Ok(measurement);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("animal/{animalId}")]
        public async Task<ActionResult<IEnumerable<WeightMeasurementDTO>>> GetWeightMeasurementsByAnimalId(int animalId)
        {
            try
            {
                return Ok(await _repository.GetWeightMeasurementsByAnimalIdAsync(animalId));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("rfid/{rfid}")]
        public async Task<ActionResult<IEnumerable<WeightMeasurementDTO>>> GetWeightMeasurementsByRfid(string rfid)
        {
            try
            {
                return Ok(await _repository.GetWeightMeasurementsByRfidAsync(rfid));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<int>> AddWeightMeasurement(WeightMeasurementDTO measurement)
        {
            try
            {
                var id = await _repository.AddWeightMeasurementAsync(measurement);
                return CreatedAtAction(nameof(GetWeightMeasurementById), new { id }, id);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateWeightMeasurement(int id, WeightMeasurementDTO measurement)
        {
            try
            {
                var success = await _repository.UpdateWeightMeasurementAsync(id, measurement);
                if (!success)
                {
                    return NotFound($"Weight measurement with ID {id} not found");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWeightMeasurement(int id)
        {
            try
            {
                var success = await _repository.DeleteWeightMeasurementAsync(id);
                if (!success)
                {
                    return NotFound($"Weight measurement with ID {id} not found");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("mobile/weight")]
        public async Task<IActionResult> AddMobileWeightMeasurement([FromBody] WeightMeasurementDTO measurement)
        {
            try
            {
                var id = await _repository.AddWeightMeasurementAsync(measurement);
                return Ok(new { success = true, message = "Weight measurement saved successfully", id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }
    }
} 