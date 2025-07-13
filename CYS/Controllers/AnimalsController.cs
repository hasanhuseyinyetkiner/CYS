using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CYS.Models;
using CYS.Repos;
using System.ComponentModel.DataAnnotations;

namespace CYS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // JWT token authentication gerekli
    public class AnimalsController : ControllerBase
    {
        private readonly ILogger<AnimalsController> _logger;
        private readonly AnimalRepository _animalRepository;

        public AnimalsController(ILogger<AnimalsController> logger, AnimalRepository animalRepository)
        {
            _logger = logger;
            _animalRepository = animalRepository;
        }

        // Tüm hayvanları listeleme
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<Animal>>>> GetAnimals(
            [FromQuery] bool? isActive = null,
            [FromQuery] int? categoryId = null,
            [FromQuery] string searchTerm = null)
        {
            try
            {
                var animals = await _animalRepository.GetAnimalsAsync(isActive, categoryId, searchTerm);
                
                return Ok(new ApiResponse<List<Animal>>
                {
                    Success = true,
                    Data = animals,
                    Message = "Hayvanlar başarıyla listelendi",
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Hayvanlar listelenirken hata oluştu");
                return StatusCode(500, new ApiResponse<List<Animal>>
                {
                    Success = false,
                    Message = "Hayvanlar listelenemedi: " + ex.Message,
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        // RFID ile hayvan arama
        [HttpGet("by-rfid/{rfidTag}")]
        public async Task<ActionResult<ApiResponse<Animal>>> GetAnimalByRfid(string rfidTag)
        {
            try
            {
                var animal = await _animalRepository.GetAnimalByRfidAsync(rfidTag);
                
                if (animal == null)
                {
                    return NotFound(new ApiResponse<Animal>
                    {
                        Success = false,
                        Message = "RFID ile hayvan bulunamadı",
                        Timestamp = DateTime.UtcNow
                    });
                }

                return Ok(new ApiResponse<Animal>
                {
                    Success = true,
                    Data = animal,
                    Message = "Hayvan başarıyla bulundu",
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RFID ile hayvan aranırken hata oluştu: {RfidTag}", rfidTag);
                return StatusCode(500, new ApiResponse<Animal>
                {
                    Success = false,
                    Message = "Hayvan aranamadı: " + ex.Message,
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        // Hayvan detayları
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<AnimalDetail>>> GetAnimalDetail(int id)
        {
            try
            {
                var animalDetail = await _animalRepository.GetAnimalDetailAsync(id);
                
                if (animalDetail == null)
                {
                    return NotFound(new ApiResponse<AnimalDetail>
                    {
                        Success = false,
                        Message = "Hayvan bulunamadı",
                        Timestamp = DateTime.UtcNow
                    });
                }

                return Ok(new ApiResponse<AnimalDetail>
                {
                    Success = true,
                    Data = AnimalDetail.FromAnimal(animalDetail),
                    Message = "Hayvan detayları başarıyla alındı",
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Hayvan detayları alınırken hata oluştu: {AnimalId}", id);
                return StatusCode(500, new ApiResponse<AnimalDetail>
                {
                    Success = false,
                    Message = "Hayvan detayları alınamadı: " + ex.Message,
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        // Hayvan ağırlık geçmişi
        [HttpGet("{id}/weight-history")]
        public async Task<ActionResult<ApiResponse<List<WeightMeasurement>>>> GetWeightHistory(
            int id, 
            [FromQuery] DateTime? startDate = null, 
            [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var weightHistory = await _animalRepository.GetWeightHistoryAsync(id, startDate, endDate);
                
                // AgirlikOlcum listesini WeightMeasurement listesine dönüştür
                var weightMeasurements = weightHistory.Select(w => new WeightMeasurement
                {
                    Id = w.id,
                    AnimalId = w.hayvanId,
                    Weight = (decimal)w.agirlik,
                    MeasurementDate = w.olcumTarihi,
                    RFID = w.Hayvan?.rfidKodu ?? "",
                    Notes = w.olcumNotu ?? "",
                    UserId = w.userId,
                    CreatedAt = w.tarih
                }).ToList();

                return Ok(new ApiResponse<List<WeightMeasurement>>
                {
                    Success = true,
                    Data = weightMeasurements,
                    Message = "Ağırlık geçmişi başarıyla alındı",
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ağırlık geçmişi alınırken hata oluştu: {AnimalId}", id);
                return StatusCode(500, new ApiResponse<List<WeightMeasurement>>
                {
                    Success = false,
                    Message = "Ağırlık geçmişi alınamadı: " + ex.Message,
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        // Yeni ağırlık ölçümü ekleme
        [HttpPost("{id}/weight")]
        public async Task<ActionResult<ApiResponse<WeightMeasurement>>> AddWeightMeasurement(
            int id, 
            [FromBody] WeightMeasurementDto weightDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<WeightMeasurement>
                    {
                        Success = false,
                        Message = "Geçersiz ağırlık verisi",
                        Timestamp = DateTime.UtcNow
                    });
                }

                weightDto.AnimalId = id;
                var result = await _animalRepository.AddWeightMeasurementAsync(weightDto);
                
                if (result)
                {
                    return Ok(new ApiResponse<string>
                    {
                        Success = true,
                        Data = "Ağırlık ölçümü başarıyla eklendi",
                        Message = "Ağırlık ölçümü başarıyla eklendi",
                        Timestamp = DateTime.UtcNow
                    });
                }
                else
                {
                    return BadRequest(new ApiResponse<string>
                    {
                        Success = false,
                        Message = "Ağırlık ölçümü eklenirken hata oluştu",
                        Timestamp = DateTime.UtcNow
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ağırlık ölçümü eklenirken hata oluştu: {AnimalId}", id);
                return StatusCode(500, new ApiResponse<WeightMeasurement>
                {
                    Success = false,
                    Message = "Ağırlık ölçümü eklenemedi: " + ex.Message,
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        // Hayvan sürüsü bilgisi
        [HttpGet("{id}/herd-info")]
        public async Task<ActionResult<ApiResponse<HerdInfo>>> GetHerdInfo(int id)
        {
            try
            {
                var herdInfoData = await _animalRepository.GetHerdInfoAsync(id);
                
                // Object'i HerdInfo'ya dönüştür
                var herdInfo = new HerdInfo
                {
                    TotalAnimals = 0,
                    ActiveAnimals = 0,
                    Categories = new List<CategoryInfo>()
                };
                
                return Ok(new ApiResponse<HerdInfo>
                {
                    Success = true,
                    Data = herdInfo,
                    Message = "Sürü bilgisi başarıyla alındı",
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Sürü bilgisi alınırken hata oluştu: {AnimalId}", id);
                return StatusCode(500, new ApiResponse<HerdInfo>
                {
                    Success = false,
                    Message = "Sürü bilgisi alınamadı: " + ex.Message,
                    Timestamp = DateTime.UtcNow
                });
            }
        }
    }
}
