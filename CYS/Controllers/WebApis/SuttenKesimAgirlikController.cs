using System;
using Microsoft.AspNetCore.Mvc;
using CYS.Models;
using CYS.Repos;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using CYS.Data;

namespace CYS.Controllers.WebApis
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuttenKesimAgirlikController : ControllerBase
    {
        private readonly WeaningWeightMeasurementRepository _repository;
        private readonly HayvanCTX _hayvanCTX;
        private readonly AnimalRepository _animalRepository;

        public SuttenKesimAgirlikController(WeaningWeightMeasurementRepository repository, AnimalRepository animalRepository)
        {
            _repository = repository;
            _animalRepository = animalRepository;
            _hayvanCTX = new HayvanCTX();
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var olcumler = _repository.GetAllWeaningWeightMeasurementsAsync().Result;
                return Ok(olcumler);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var olcum = _repository.GetWeaningWeightMeasurementByIdAsync(id).Result;
                if (olcum == null)
                {
                    return NotFound(new { message = "Sütten kesim ağırlık ölçümü bulunamadı" });
                }
                return Ok(olcum);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult AddOlcum([FromBody] WeaningWeightMeasurement olcum)
        {
            try
            {
                if (olcum.MeasurementDate == DateTime.MinValue)
                {
                    olcum.MeasurementDate = DateTime.Now;
                }

                var dto = new Models.DTOs.WeaningWeightMeasurementDTO
                {
                    AnimalId = olcum.AnimalId,
                    Weight = olcum.Weight,
                    MeasurementDate = olcum.MeasurementDate,
                    RFID = olcum.RFID,
                    Notes = olcum.Notes,
                    UserId = olcum.UserId,
                    WeaningDate = olcum.WeaningDate,
                    WeaningAge = olcum.WeaningAge,
                    MotherRFID = olcum.MotherRFID
                };

                var id = _repository.AddWeaningWeightMeasurementAsync(dto).Result;
                olcum.Id = id;

                // Hayvanın sütten kesim ağırlık bilgisini de güncelle
                if (olcum.AnimalId > 0)
                {
                    var hayvan = _hayvanCTX.hayvanTek("SELECT * FROM Hayvan WHERE id = @id", new { id = olcum.AnimalId });

                    if (hayvan != null)
                    {
                        // Update with a SQL query 
                        _hayvanCTX.executeNonQuery("UPDATE Hayvan SET suttenKesimAgirlik = @agirlik WHERE id = @id", 
                            new { agirlik = olcum.Weight.ToString(), id = olcum.AnimalId });
                    }
                }

                return Ok(new { success = true, message = "Sütten kesim ağırlık ölçümü başarıyla kaydedildi", data = olcum });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] WeaningWeightMeasurement olcum)
        {
            try
            {
                var existingOlcum = _repository.GetWeaningWeightMeasurementByIdAsync(id).Result;
                if (existingOlcum == null)
                {
                    return NotFound(new { message = "Sütten kesim ağırlık ölçümü bulunamadı" });
                }

                var dto = new Models.DTOs.WeaningWeightMeasurementDTO
                {
                    Id = id,
                    AnimalId = olcum.AnimalId,
                    Weight = olcum.Weight,
                    MeasurementDate = olcum.MeasurementDate,
                    RFID = olcum.RFID,
                    Notes = olcum.Notes,
                    UserId = olcum.UserId,
                    WeaningDate = olcum.WeaningDate,
                    WeaningAge = olcum.WeaningAge,
                    MotherRFID = olcum.MotherRFID
                };

                _repository.UpdateWeaningWeightMeasurementAsync(id, dto).Wait();

                return Ok(new { success = true, message = "Sütten kesim ağırlık ölçümü başarıyla güncellendi", data = olcum });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var existingOlcum = _repository.GetWeaningWeightMeasurementByIdAsync(id).Result;
                if (existingOlcum == null)
                {
                    return NotFound(new { message = "Sütten kesim ağırlık ölçümü bulunamadı" });
                }

                _repository.DeleteWeaningWeightMeasurementAsync(id).Wait();

                return Ok(new { success = true, message = "Sütten kesim ağırlık ölçümü başarıyla silindi" });
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
                var olcumler = _repository.GetWeaningWeightMeasurementsByAnimalIdAsync(hayvanId).Result;
                return Ok(olcumler);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Mobil cihazlardan gelen sütten kesim ağırlık ölçümlerini kaydetmek için endpoint
        [HttpPost("mobile/weaning-weight")]
        public IActionResult SaveMobileWeaningWeight([FromBody] MobileWeaningWeight data)
        {
            try
            {
                var dto = new Models.DTOs.WeaningWeightMeasurementDTO
                {
                    AnimalId = data.HayvanId,
                    Weight = data.Weight,
                    MeasurementDate = DateTime.Now,
                    RFID = data.RFID,
                    Notes = data.Note ?? "",
                    UserId = data.UserId,
                    WeaningDate = data.WeaningDate,
                    WeaningAge = data.WeaningAge,
                    MotherRFID = data.MotherRFID
                };

                var id = _repository.AddWeaningWeightMeasurementAsync(dto).Result;

                // Hayvanın sütten kesim ağırlık bilgisini de güncelle
                if (data.HayvanId > 0)
                {
                    var hayvan = _hayvanCTX.hayvanTek("SELECT * FROM Hayvan WHERE id = @id", new { id = data.HayvanId });

                    if (hayvan != null)
                    {
                        // Update with a SQL query
                        _hayvanCTX.executeNonQuery("UPDATE Hayvan SET suttenKesimAgirlik = @agirlik WHERE id = @id", 
                            new { agirlik = data.Weight.ToString(), id = data.HayvanId });
                    }
                }

                return Ok(new { success = true, message = "Mobil cihazdan gelen sütten kesim ağırlık ölçümü başarıyla kaydedildi" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }

    // DTO for mobile weaning weight data
    public class MobileWeaningWeight
    {
        public decimal Weight { get; set; }
        public int HayvanId { get; set; }
        public int UserId { get; set; }
        public string? RFID { get; set; }
        public string? Note { get; set; }
        public DateTime? WeaningDate { get; set; }
        public int? WeaningAge { get; set; }
        public string? MotherRFID { get; set; }
    }
} 