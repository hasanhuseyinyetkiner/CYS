using CYS.Models;
using CYS.Repos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using CYS.Data;

namespace CYS.Controllers.WebApis
{
	[Route("api/[controller]")]
	[ApiController]
	public class AgirlikApiController : ControllerBase
	{
		private readonly WeightMeasurementRepository _repository;
		private readonly HayvanCTX _hayvanCTX;
		private readonly AnimalRepository _animalRepository;
		
		public AgirlikApiController(WeightMeasurementRepository repository, AnimalRepository animalRepository)
		{
			_repository = repository;
			_animalRepository = animalRepository;
			_hayvanCTX = new HayvanCTX();
		}
		
		// GET: api/AgirlikApi
		[HttpGet]
		public IActionResult Get()
		{
			try
			{
				var olcumler = _repository.GetAllWeightMeasurementsAsync().Result;
				return Ok(olcumler);
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}
		
		// GET api/AgirlikApi/5
		[HttpGet("{id}")]
		public IActionResult GetById(int id)
		{
			try
			{
				var olcum = _repository.GetWeightMeasurementByIdAsync(id).Result;
				if (olcum == null)
				{
					return NotFound(new { message = "Ağırlık ölçümü bulunamadı" });
				}
				return Ok(olcum);
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}
		
		// POST ağırlık ölçüm kaydetme
		[HttpPost]
		public IActionResult AddOlcum([FromBody] WeightMeasurement olcum)
		{
			try
			{
				if (olcum.MeasurementDate == DateTime.MinValue)
				{
					olcum.MeasurementDate = DateTime.Now;
				}
				
				var dto = new Models.DTOs.WeightMeasurementDTO 
				{
					AnimalId = olcum.AnimalId,
					Weight = olcum.Weight,
					MeasurementDate = olcum.MeasurementDate,
					RFID = olcum.RFID,
					Notes = olcum.Notes,
					UserId = olcum.UserId
				};
				
				var id = _repository.AddWeightMeasurementAsync(dto).Result;
				olcum.Id = id;
				
				// Hayvanın ağırlık bilgisini de güncelle
				if (olcum.AnimalId > 0)
				{
					var hayvan = _hayvanCTX.hayvanTek("SELECT * FROM Hayvan WHERE id = @id", new { id = olcum.AnimalId });
					if (hayvan != null)
					{
						hayvan.agirlik = olcum.Weight.ToString();
						_hayvanCTX.hayvanGuncelle(hayvan);
					}
				}
				
				return Ok(new { success = true, message = "Ağırlık ölçümü başarıyla kaydedildi", data = olcum });
			}
			catch (Exception ex)
			{
				return BadRequest(new { success = false, message = ex.Message });
			}
		}

		// PUT api/AgirlikApi/5
		[HttpPut("{id}")]
		public IActionResult Update(int id, [FromBody] WeightMeasurement olcum)
		{
			try
			{
				var existingOlcum = _repository.GetWeightMeasurementByIdAsync(id).Result;
				if (existingOlcum == null)
				{
					return NotFound(new { message = "Ağırlık ölçümü bulunamadı" });
				}
				
				var dto = new Models.DTOs.WeightMeasurementDTO 
				{
					Id = id,
					AnimalId = olcum.AnimalId,
					Weight = olcum.Weight,
					MeasurementDate = olcum.MeasurementDate,
					RFID = olcum.RFID,
					Notes = olcum.Notes,
					UserId = olcum.UserId
				};
				
				_repository.UpdateWeightMeasurementAsync(id, dto).Wait();
				
				return Ok(new { success = true, message = "Ağırlık ölçümü başarıyla güncellendi", data = olcum });
			}
			catch (Exception ex)
			{
				return BadRequest(new { success = false, message = ex.Message });
			}
		}

		// DELETE api/AgirlikApi/5
		[HttpDelete("{id}")]
		public IActionResult Delete(int id)
		{
			try
			{
				var existingOlcum = _repository.GetWeightMeasurementByIdAsync(id).Result;
				if (existingOlcum == null)
				{
					return NotFound(new { message = "Ağırlık ölçümü bulunamadı" });
				}
				
				_repository.DeleteWeightMeasurementAsync(id).Wait();
				
				return Ok(new { success = true, message = "Ağırlık ölçümü başarıyla silindi" });
			}
			catch (Exception ex)
			{
				return BadRequest(new { success = false, message = ex.Message });
			}
		}

		[HttpGet("hayvan/{hayvanId}")]
		public IActionResult GetByHayvanId(int hayvanId)
		{
			try
			{
				var olcumler = _repository.GetWeightMeasurementsByAnimalIdAsync(hayvanId).Result;
				return Ok(olcumler);
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}

		// Mobil cihazlardan gelen ağırlık ölçümlerini kaydetmek için endpoint
		[HttpPost("mobile/weight")]
		public IActionResult SaveMobileWeight([FromBody] MobileNormalWeight data)
		{
			try
			{
				var dto = new Models.DTOs.WeightMeasurementDTO
				{
					AnimalId = data.HayvanId,
					Weight = data.Weight,
					MeasurementDate = DateTime.Now,
					RFID = data.RFID ?? "",
					Notes = data.Note ?? "",
					UserId = data.UserId
				};
				
				var id = _repository.AddWeightMeasurementAsync(dto).Result;
				
				// Hayvanın ağırlık bilgisini de güncelle
				if (data.HayvanId > 0)
				{
					var hayvan = _hayvanCTX.hayvanTek("SELECT * FROM Hayvan WHERE id = @id", new { id = data.HayvanId });
					if (hayvan != null)
					{
						hayvan.agirlik = data.Weight.ToString();
						_hayvanCTX.hayvanGuncelle(hayvan);
					}
				}
				
				return Ok(new { success = true, message = "Mobil cihazdan gelen ağırlık ölçümü başarıyla kaydedildi" });
			}
			catch (Exception ex)
			{
				return BadRequest(new { success = false, message = ex.Message });
			}
		}
	}
	
	// DTO for mobile weight data
	public class MobileNormalWeight
	{
		public decimal Weight { get; set; }
		public int HayvanId { get; set; }
		public int UserId { get; set; }
		public string? RFID { get; set; }
		public string? Note { get; set; }
	}
}
