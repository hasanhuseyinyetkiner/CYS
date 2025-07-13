using System;
using Microsoft.AspNetCore.Mvc;
using CYS.Models;
using CYS.Repos;
using Microsoft.Extensions.Configuration;

namespace CYS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AgirlikOlcumController : ControllerBase
    {
        private readonly AgirlikOlcumRepository _repository;

        public AgirlikOlcumController(IConfiguration configuration)
        {
            _repository = new AgirlikOlcumRepository(configuration);
        }

        [HttpPost]
        public IActionResult AddOlcum([FromBody] AgirlikOlcum olcum)
        {
            try
            {
                olcum.olcumTarihi = DateTime.Now;
                _repository.Add(olcum);
                return Ok(olcum);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("hayvan/{hayvanId}")]
        public IActionResult GetHayvanOlcumler(int hayvanId)
        {
            try
            {
                var olcumler = _repository.GetByHayvanId(hayvanId);
                return Ok(olcumler);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("mobile/weight")]
        public IActionResult SaveMobileWeight([FromBody] MobileWeightData data)
        {
            try
            {
                var agirlikOlcum = new AgirlikOlcum
                {
                    hayvanId = data.HayvanId > 0 ? data.HayvanId : 0,
                    agirlik = data.Weight,
                    olcumTarihi = DateTime.Now,
                    bluetoothOlcum = true,
                    userId = data.UserId,
                    olcumNotu = data.Note,
                    requestId = data.RequestId,
                    agirlikOlcumu = data.Weight.ToString()
                };

                _repository.Add(agirlikOlcum);
                return Ok(new { success = true, message = "Ağırlık verisi başarıyla kaydedildi", data = agirlikOlcum });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }

    // DTO for mobile weight data
    public class MobileWeightData
    {
        public double Weight { get; set; }
        public int HayvanId { get; set; }
        public int UserId { get; set; }
        public string? Note { get; set; }
        public string? RequestId { get; set; }
    }
} 