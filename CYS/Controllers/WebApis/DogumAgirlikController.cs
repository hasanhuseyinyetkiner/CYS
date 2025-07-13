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
    public class DogumAgirlikController : ControllerBase
    {
        private readonly BirthWeightMeasurementRepository _repository;
        private readonly HayvanCTX _hayvanCTX;
        private readonly AnimalRepository _animalRepository;

        public DogumAgirlikController(BirthWeightMeasurementRepository repository, AnimalRepository animalRepository)
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
                var olcumler = _repository.GetAllBirthWeightMeasurementsAsync().Result;
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
                var olcum = _repository.GetBirthWeightMeasurementByIdAsync(id).Result;
                if (olcum == null)
                {
                    return NotFound(new { message = "Doğum ağırlık ölçümü bulunamadı" });
                }
                return Ok(olcum);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult AddOlcum([FromBody] BirthWeightMeasurement olcum)
        {
            try
            {
                if (olcum.MeasurementDate == DateTime.MinValue)
                {
                    olcum.MeasurementDate = DateTime.Now;
                }

                var dto = new Models.DTOs.BirthWeightMeasurementDTO
                {
                    AnimalId = olcum.AnimalId,
                    Weight = olcum.Weight,
                    MeasurementDate = olcum.MeasurementDate,
                    RFID = olcum.RFID,
                    MotherRFID = olcum.MotherRFID,
                    Notes = olcum.Notes,
                    UserId = olcum.UserId,
                    BirthDate = olcum.BirthDate,
                    BirthPlace = olcum.BirthPlace
                };

                var id = _repository.AddBirthWeightMeasurementAsync(dto).Result;
                olcum.Id = id;

                // Hayvanın doğum ağırlık bilgisini de güncelle
                if (olcum.AnimalId > 0)
                {
                    var hayvan = _hayvanCTX.hayvanTek("SELECT * FROM Hayvan WHERE id = @id", new { id = olcum.AnimalId });

                    if (hayvan != null)
                    {
                        // Update with a SQL query since the property doesn't exist in the class
                        _hayvanCTX.executeNonQuery("UPDATE Hayvan SET dogumAgirlik = @agirlik WHERE id = @id", 
                            new { agirlik = olcum.Weight.ToString(), id = olcum.AnimalId });
                    }
                }

                return Ok(new { success = true, message = "Doğum ağırlık ölçümü başarıyla kaydedildi", data = olcum });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] BirthWeightMeasurement olcum)
        {
            try
            {
                var existingOlcum = _repository.GetBirthWeightMeasurementByIdAsync(id).Result;
                if (existingOlcum == null)
                {
                    return NotFound(new { message = "Doğum ağırlık ölçümü bulunamadı" });
                }

                var dto = new Models.DTOs.BirthWeightMeasurementDTO
                {
                    Id = id,
                    AnimalId = olcum.AnimalId,
                    Weight = olcum.Weight,
                    MeasurementDate = olcum.MeasurementDate,
                    RFID = olcum.RFID,
                    MotherRFID = olcum.MotherRFID,
                    Notes = olcum.Notes,
                    UserId = olcum.UserId,
                    BirthDate = olcum.BirthDate,
                    BirthPlace = olcum.BirthPlace
                };

                _repository.UpdateBirthWeightMeasurementAsync(id, dto).Wait();

                return Ok(new { success = true, message = "Doğum ağırlık ölçümü başarıyla güncellendi", data = olcum });
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
                var existingOlcum = _repository.GetBirthWeightMeasurementByIdAsync(id).Result;
                if (existingOlcum == null)
                {
                    return NotFound(new { message = "Doğum ağırlık ölçümü bulunamadı" });
                }

                _repository.DeleteBirthWeightMeasurementAsync(id).Wait();

                return Ok(new { success = true, message = "Doğum ağırlık ölçümü başarıyla silindi" });
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
                var olcumler = _repository.GetBirthWeightMeasurementsByAnimalIdAsync(hayvanId).Result;
                return Ok(olcumler);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Mobil cihazlardan gelen doğum ağırlık ölçümlerini kaydetmek için endpoint
        [HttpPost("mobile/birth-weight")]
        public IActionResult SaveMobileBirthWeight([FromBody] MobileBirthWeight data)
        {
            try
            {
                var dto = new Models.DTOs.BirthWeightMeasurementDTO
                {
                    AnimalId = data.HayvanId,
                    Weight = data.Weight,
                    MeasurementDate = DateTime.Now,
                    RFID = data.RFID,
                    MotherRFID = data.MotherRFID,
                    Notes = data.Note ?? "",
                    UserId = data.UserId,
                    BirthDate = data.BirthDate,
                    BirthPlace = data.BirthPlace
                };

                var id = _repository.AddBirthWeightMeasurementAsync(dto).Result;

                // Hayvanın doğum ağırlık bilgisini de güncelle
                if (data.HayvanId > 0)
                {
                    var hayvan = _hayvanCTX.hayvanTek("SELECT * FROM Hayvan WHERE id = @id", new { id = data.HayvanId });

                    if (hayvan != null)
                    {
                        // Update with a SQL query since the property doesn't exist in the class
                        _hayvanCTX.executeNonQuery("UPDATE Hayvan SET dogumAgirlik = @agirlik WHERE id = @id", 
                            new { agirlik = data.Weight.ToString(), id = data.HayvanId });
                    }
                }

                return Ok(new { success = true, message = "Mobil cihazdan gelen doğum ağırlık ölçümü başarıyla kaydedildi" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }

    // DTO for mobile birth weight data
    public class MobileBirthWeight
    {
        public decimal Weight { get; set; }
        public int HayvanId { get; set; }
        public int UserId { get; set; }
        public string? RFID { get; set; }
        public string? MotherRFID { get; set; }
        public string? Note { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? BirthPlace { get; set; }
    }
} 