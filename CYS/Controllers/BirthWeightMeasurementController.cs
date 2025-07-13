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
    public class BirthWeightMeasurementController : ControllerBase
    {
        private readonly BirthWeightMeasurementRepository _repository;

        public BirthWeightMeasurementController(BirthWeightMeasurementRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BirthWeightMeasurementDTO>>> GetAllBirthWeightMeasurements()
        {
            try
            {
                return Ok(await _repository.GetAllBirthWeightMeasurementsAsync());
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BirthWeightMeasurementDTO>> GetBirthWeightMeasurementById(int id)
        {
            try
            {
                var measurement = await _repository.GetBirthWeightMeasurementByIdAsync(id);
                if (measurement == null)
                {
                    return NotFound($"Birth weight measurement with ID {id} not found");
                }
                return Ok(measurement);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("animal/{animalId}")]
        public async Task<ActionResult<IEnumerable<BirthWeightMeasurementDTO>>> GetBirthWeightMeasurementsByAnimalId(int animalId)
        {
            try
            {
                return Ok(await _repository.GetBirthWeightMeasurementsByAnimalIdAsync(animalId));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("rfid/{rfid}")]
        public async Task<ActionResult<IEnumerable<BirthWeightMeasurementDTO>>> GetBirthWeightMeasurementsByRfid(string rfid)
        {
            try
            {
                return Ok(await _repository.GetBirthWeightMeasurementsByRfidAsync(rfid));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<int>> AddBirthWeightMeasurement(BirthWeightMeasurementDTO measurement)
        {
            try
            {
                var id = await _repository.AddBirthWeightMeasurementAsync(measurement);
                return CreatedAtAction(nameof(GetBirthWeightMeasurementById), new { id }, id);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBirthWeightMeasurement(int id, BirthWeightMeasurementDTO measurement)
        {
            try
            {
                var success = await _repository.UpdateBirthWeightMeasurementAsync(id, measurement);
                if (!success)
                {
                    return NotFound($"Birth weight measurement with ID {id} not found");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBirthWeightMeasurement(int id)
        {
            try
            {
                var success = await _repository.DeleteBirthWeightMeasurementAsync(id);
                if (!success)
                {
                    return NotFound($"Birth weight measurement with ID {id} not found");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("mobile/birth-weight")]
        public async Task<IActionResult> AddMobileBirthWeightMeasurement([FromBody] BirthWeightMeasurementDTO measurement)
        {
            try
            {
                var id = await _repository.AddBirthWeightMeasurementAsync(measurement);
                return Ok(new { success = true, message = "Birth weight measurement saved successfully", id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }
    }
} 