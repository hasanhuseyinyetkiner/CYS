using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CYS.Models;
using CYS.Repos;
using Microsoft.AspNetCore.Cors;

namespace CYS.Controllers.WebApis
{
    [Route("api/test")]
    [ApiController]
    [EnableCors("AllowAll")]
    public class TestApiController : ControllerBase
    {
        private readonly TestWeightRepository _repository;

        public TestApiController(TestWeightRepository repository)
        {
            _repository = repository;
        }

        // GET: api/test/ping
        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok(new { success = true, message = "API çalışıyor", timestamp = DateTime.Now });
        }

        // GET: api/test/weights
        [HttpGet("weights")]
        public async Task<IActionResult> GetWeights()
        {
            var weights = await _repository.GetAllWeightsAsync();
            return Ok(weights);
        }

        // POST: api/test/weights
        [HttpPost("weights")]
        public async Task<IActionResult> AddWeight([FromBody] TestWeightMeasurement measurement)
        {
            try
            {
                measurement.MeasurementDate = DateTime.Now;
                measurement.Source = measurement.Source ?? Request.Headers["User-Agent"].ToString();
                var result = await _repository.AddWeightAsync(measurement);
                return Ok(new { 
                    success = true, 
                    message = "Ölçüm başarıyla kaydedildi", 
                    data = result 
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        // DELETE: api/test/weights/{id}
        [HttpDelete("weights/{id}")]
        public async Task<IActionResult> DeleteWeight(int id)
        {
            var success = await _repository.DeleteWeightAsync(id);
            if (!success)
            {
                return NotFound(new { success = false, message = "Ölçüm bulunamadı" });
            }

            return Ok(new { success = true, message = "Ölçüm silindi" });
        }

        // Tüm verileri temizle
        [HttpDelete("weights")]
        public async Task<IActionResult> ClearWeights()
        {
            await _repository.ClearAllWeightsAsync();
            return Ok(new { success = true, message = "Tüm ölçümler silindi" });
        }
    }
} 