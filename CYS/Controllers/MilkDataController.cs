using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CYS.Models;
using CYS.Repos;
using CYS.Helper;
using System.Text.Json;
using System.ComponentModel.DataAnnotations;

namespace CYS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // JWT token authentication gerekli
    public class MilkDataController : ControllerBase
    {
        private readonly ILogger<MilkDataController> _logger;
        private readonly MilkDataRepository _milkDataRepository;

        public MilkDataController(ILogger<MilkDataController> logger, MilkDataRepository milkDataRepository)
        {
            _logger = logger;
            _milkDataRepository = milkDataRepository;
        }

        // Süt analiz verilerini alma
        [HttpGet("analysis-results")]
        public async Task<ActionResult<ApiResponse<List<MilkAnalysisResult>>>> GetAnalysisResults(
            [FromQuery] int? animalId = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            try
            {
                var results = await _milkDataRepository.GetAnalysisResultsAsync(animalId, startDate, endDate, page, pageSize);
                
                return Ok(new ApiResponse<List<MilkAnalysisResult>>
                {
                    Success = true,
                    Data = results,
                    Message = "Süt analiz verileri başarıyla alındı",
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Süt analiz verileri alınırken hata oluştu");
                return StatusCode(500, new ApiResponse<List<MilkAnalysisResult>>
                {
                    Success = false,
                    Message = "Veri alınırken hata oluştu: " + ex.Message,
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        // Yeni süt analiz verisi ekleme
        [HttpPost("analysis-results")]
        public async Task<ActionResult<ApiResponse<MilkAnalysisResult>>> AddAnalysisResult([FromBody] MilkAnalysisResultDto analysisDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<MilkAnalysisResult>
                    {
                        Success = false,
                        Message = "Geçersiz veri formatı",
                        Timestamp = DateTime.UtcNow
                    });
                }

                var result = await _milkDataRepository.AddAnalysisResultAsync(analysisDto);
                
                return Ok(new ApiResponse<MilkAnalysisResult>
                {
                    Success = true,
                    Data = result,
                    Message = "Süt analiz verisi başarıyla eklendi",
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Süt analiz verisi eklenirken hata oluştu");
                return StatusCode(500, new ApiResponse<MilkAnalysisResult>
                {
                    Success = false,
                    Message = "Veri eklenirken hata oluştu: " + ex.Message,
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        // Süt örnekleri alma
        [HttpGet("samples")]
        public async Task<ActionResult<ApiResponse<List<MilkSample>>>> GetMilkSamples(
            [FromQuery] int? animalId = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] string status = null)
        {
            try
            {
                var samples = await _milkDataRepository.GetMilkSamplesAsync(animalId, startDate, endDate, status);
                
                return Ok(new ApiResponse<List<MilkSample>>
                {
                    Success = true,
                    Data = samples,
                    Message = "Süt örnekleri başarıyla alındı",
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Süt örnekleri alınırken hata oluştu");
                return StatusCode(500, new ApiResponse<List<MilkSample>>
                {
                    Success = false,
                    Message = "Veri alınırken hata oluştu: " + ex.Message,
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        // Hayvan süt verimi istatistikleri
        [HttpGet("animal-statistics/{animalId}")]
        public async Task<ActionResult<ApiResponse<AnimalMilkStatistics>>> GetAnimalStatistics(int animalId)
        {
            try
            {
                var statistics = await _milkDataRepository.GetAnimalMilkStatisticsAsync(animalId);
                
                if (statistics == null)
                {
                    return NotFound(new ApiResponse<AnimalMilkStatistics>
                    {
                        Success = false,
                        Message = "Hayvan bulunamadı",
                        Timestamp = DateTime.UtcNow
                    });
                }

                return Ok(new ApiResponse<AnimalMilkStatistics>
                {
                    Success = true,
                    Data = statistics,
                    Message = "Hayvan istatistikleri başarıyla alındı",
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Hayvan istatistikleri alınırken hata oluştu: AnimalId={AnimalId}", animalId);
                return StatusCode(500, new ApiResponse<AnimalMilkStatistics>
                {
                    Success = false,
                    Message = "İstatistikler alınırken hata oluştu: " + ex.Message,
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        // Toplu veri senkronizasyonu
        [HttpPost("sync")]
        public async Task<ActionResult<ApiResponse<SyncResult>>> SyncMilkData([FromBody] MilkDataSyncRequest syncRequest)
        {
            try
            {
                var syncResult = await _milkDataRepository.SyncMilkDataAsync(syncRequest);
                
                return Ok(new ApiResponse<SyncResult>
                {
                    Success = true,
                    Data = syncResult,
                    Message = $"Senkronizasyon tamamlandı. {syncResult.ProcessedRecords} kayıt işlendi",
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Veri senkronizasyonu sırasında hata oluştu");
                return StatusCode(500, new ApiResponse<SyncResult>
                {
                    Success = false,
                    Message = "Senkronizasyon hatası: " + ex.Message,
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        // Kalite standartları karşılaştırması
        [HttpPost("quality-check")]
        public async Task<ActionResult<ApiResponse<QualityCheckResult>>> CheckMilkQuality([FromBody] QualityCheckRequest request)
        {
            try
            {
                var qualityResult = await _milkDataRepository.CheckMilkQualityAsync(request);
                
                return Ok(new ApiResponse<QualityCheckResult>
                {
                    Success = true,
                    Data = qualityResult,
                    Message = "Kalite kontrol tamamlandı",
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kalite kontrol sırasında hata oluştu");
                return StatusCode(500, new ApiResponse<QualityCheckResult>
                {
                    Success = false,
                    Message = "Kalite kontrol hatası: " + ex.Message,
                    Timestamp = DateTime.UtcNow
                });
            }
        }
    }
}
