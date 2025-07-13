using CYS.Models;
using CYS.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace CYS.Repos
{
    public class MilkDataRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<MilkDataRepository> _logger;

        public MilkDataRepository(ApplicationDbContext context, ILogger<MilkDataRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Süt analiz sonuçlarını alma
        public async Task<List<MilkAnalysisResult>> GetAnalysisResultsAsync(
            int? animalId = null, 
            DateTime? startDate = null, 
            DateTime? endDate = null, 
            int page = 1, 
            int pageSize = 50)
        {
            var query = _context.MilkAnalysisResults
                .Include(m => m.Animal)
                .AsQueryable();

            if (animalId.HasValue)
                query = query.Where(m => m.AnimalId == animalId.Value);

            if (startDate.HasValue)
                query = query.Where(m => m.AnalysisDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(m => m.AnalysisDate <= endDate.Value);

            return await query
                .OrderByDescending(m => m.AnalysisDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        // Yeni süt analiz sonucu ekleme
        public async Task<MilkAnalysisResult> AddAnalysisResultAsync(MilkAnalysisResultDto analysisDto)
        {
            var analysisResult = new MilkAnalysisResult
            {
                AnimalId = analysisDto.AnimalId,
                AnalysisDate = analysisDto.AnalysisDate,
                AnalysisMethod = analysisDto.AnalysisMethod,
                PhValue = analysisDto.PhValue,
                FatPercentage = analysisDto.FatPercentage,
                ProteinPercentage = analysisDto.ProteinPercentage,
                LactosePercentage = analysisDto.LactosePercentage,
                MineralPercentage = analysisDto.MineralPercentage,
                WaterPercentage = analysisDto.WaterPercentage,
                Density = analysisDto.Density,
                Temperature = analysisDto.Temperature,
                Viscosity = analysisDto.Viscosity,
                Conductivity = analysisDto.Conductivity,
                TotalBacteriaCount = analysisDto.TotalBacteriaCount,
                SomaticCellCount = analysisDto.SomaticCellCount,
                FreezingPoint = analysisDto.FreezingPoint,
                AcidityLevel = analysisDto.AcidityLevel,
                PeroxideValue = analysisDto.PeroxideValue,
                UreaContent = analysisDto.UreaContent,
                AddedWaterPercentage = analysisDto.AddedWaterPercentage,
                SaltContent = analysisDto.SaltContent,
                AnalysisDurationSeconds = analysisDto.AnalysisDurationSeconds,
                InstrumentId = analysisDto.InstrumentId,
                ValidationNotes = analysisDto.ValidationNotes,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.MilkAnalysisResults.Add(analysisResult);
            await _context.SaveChangesAsync();

            return analysisResult;
        }

        // Süt örneklerini alma
        public async Task<List<MilkSample>> GetMilkSamplesAsync(
            int? animalId = null, 
            DateTime? startDate = null, 
            DateTime? endDate = null, 
            string status = null)
        {
            var query = _context.MilkSamples
                .Include(s => s.Animal)
                .Include(s => s.AnalysisResults)
                .AsQueryable();

            if (animalId.HasValue)
                query = query.Where(s => s.AnimalId == animalId.Value);

            if (startDate.HasValue)
                query = query.Where(s => s.CollectedAt >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(s => s.CollectedAt <= endDate.Value);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(s => s.Status == status);

            return await query
                .OrderByDescending(s => s.CollectedAt)
                .ToListAsync();
        }

        // Hayvan süt istatistikleri hesaplama
        public async Task<AnimalMilkStatistics> GetAnimalMilkStatisticsAsync(int animalId)
        {
            var animal = await _context.Animals.FindAsync(animalId);
            if (animal == null) return null;

            var analysisResults = await _context.MilkAnalysisResults
                .Where(m => m.AnimalId == animalId && m.IsValid)
                .OrderBy(m => m.AnalysisDate)
                .ToListAsync();

            if (!analysisResults.Any())
            {
                return new AnimalMilkStatistics
                {
                    AnimalId = animalId,
                    AnimalName = animal.Name,
                    RfidTag = animal.RfidTag,
                    TotalAnalysisCount = 0
                };
            }

            // Süt verimi hesaplamaları (günlük ortalama, toplam, min, max)
            var dailyYields = analysisResults
                .GroupBy(r => r.AnalysisDate.Date)
                .Select(g => new { Date = g.Key, DailyYield = g.Sum(r => r.WaterPercentage ?? 0) })
                .ToList();

            var avgFat = analysisResults.Where(r => r.FatPercentage.HasValue).Average(r => r.FatPercentage.Value);
            var avgProtein = analysisResults.Where(r => r.ProteinPercentage.HasValue).Average(r => r.ProteinPercentage.Value);
            var avgLactose = analysisResults.Where(r => r.LactosePercentage.HasValue).Average(r => r.LactosePercentage.Value);
            var avgPh = analysisResults.Where(r => r.PhValue.HasValue).Average(r => r.PhValue.Value);

            // Kalite skoru hesaplama (basit algoritma)
            decimal qualityScore = CalculateQualityScore(avgFat, avgProtein, 
                analysisResults.Where(r => r.TotalBacteriaCount.HasValue).Average(r => r.TotalBacteriaCount.Value));

            return new AnimalMilkStatistics
            {
                AnimalId = animalId,
                AnimalName = animal.Name,
                RfidTag = animal.RfidTag,
                AverageDailyMilkYield = dailyYields.Any() ? (decimal)dailyYields.Average(d => d.DailyYield) : 0,
                TotalMilkYield = dailyYields.Any() ? (decimal)dailyYields.Sum(d => d.DailyYield) : 0,
                MaxDailyMilkYield = dailyYields.Any() ? (decimal)dailyYields.Max(d => d.DailyYield) : 0,
                MinDailyMilkYield = dailyYields.Any() ? (decimal)dailyYields.Min(d => d.DailyYield) : 0,
                AverageFatPercentage = (decimal?)avgFat,
                AverageProteinPercentage = (decimal?)avgProtein,
                AverageLactosePercentage = (decimal?)avgLactose,
                AveragePhValue = (decimal?)avgPh,
                AverageBacteriaCount = analysisResults.Where(r => r.TotalBacteriaCount.HasValue).Average(r => r.TotalBacteriaCount.Value),
                AverageSomaticCellCount = analysisResults.Where(r => r.SomaticCellCount.HasValue).Average(r => r.SomaticCellCount.Value),
                FirstAnalysisDate = analysisResults.Min(r => r.AnalysisDate),
                LastAnalysisDate = analysisResults.Max(r => r.AnalysisDate),
                TotalAnalysisCount = analysisResults.Count,
                AverageQualityScore = qualityScore,
                QualityGrade = GetQualityGrade(qualityScore)
            };
        }

        // Toplu veri senkronizasyonu
        public async Task<SyncResult> SyncMilkDataAsync(MilkDataSyncRequest syncRequest)
        {
            var result = new SyncResult
            {
                SyncCompletedAt = DateTime.UtcNow
            };

            using var transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                int successCount = 0;
                int errorCount = 0;

                // Analiz sonuçlarını senkronize et
                foreach (var analysisDto in syncRequest.AnalysisResults)
                {
                    try
                    {
                        await AddAnalysisResultAsync(analysisDto);
                        successCount++;
                    }
                    catch (Exception ex)
                    {
                        errorCount++;
                        result.Errors.Add($"Analiz verisi hatası: {ex.Message}");
                    }
                }

                // Örnekleri senkronize et
                foreach (var sampleDto in syncRequest.Samples)
                {
                    try
                    {
                        var sample = new MilkSample
                        {
                            SampleId = sampleDto.SampleId,
                            AnimalId = sampleDto.AnimalId,
                            CollectedAt = sampleDto.CollectedAt,
                            SampleType = sampleDto.SampleType,
                            SourceLocation = sampleDto.SourceLocation,
                            CollectionTemperature = sampleDto.CollectionTemperature,
                            CollectionNotes = sampleDto.CollectionNotes,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        };

                        _context.MilkSamples.Add(sample);
                        successCount++;
                    }
                    catch (Exception ex)
                    {
                        errorCount++;
                        result.Errors.Add($"Örnek verisi hatası: {ex.Message}");
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                result.Success = errorCount == 0;
                result.ProcessedRecords = successCount + errorCount;
                result.SuccessfulRecords = successCount;
                result.FailedRecords = errorCount;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                result.Success = false;
                result.Errors.Add($"Senkronizasyon hatası: {ex.Message}");
            }

            return result;
        }

        // Kalite kontrol
        public async Task<QualityCheckResult> CheckMilkQualityAsync(QualityCheckRequest request)
        {
            var analysisResults = await _context.MilkAnalysisResults
                .Where(r => request.AnalysisResultIds.Contains(r.Id))
                .ToListAsync();

            var result = new QualityCheckResult
            {
                TotalChecked = analysisResults.Count
            };

            // TS 1018 standardına göre kalite kontrol
            var qualityStandards = GetQualityStandards(request.QualityStandardName);

            foreach (var analysis in analysisResults)
            {
                var issues = CheckAnalysisQuality(analysis, qualityStandards);
                result.Issues.AddRange(issues);

                if (issues.Any())
                    result.FailedCount++;
                else
                    result.PassedCount++;
            }

            result.CompliancePercentage = result.TotalChecked > 0 
                ? (decimal)result.PassedCount / result.TotalChecked * 100 
                : 0;

            result.OverallGrade = GetOverallGrade(result.CompliancePercentage);

            return result;
        }

        // Yardımcı metodlar
        private decimal CalculateQualityScore(double? fatPercentage, double? proteinPercentage, double? bacteriaCount)
        {
            decimal score = 100;

            // Yağ oranı değerlendirmesi
            if (fatPercentage.HasValue)
            {
                if (fatPercentage < 3.0 || fatPercentage > 5.0)
                    score -= 20;
                else if (fatPercentage < 3.5 || fatPercentage > 4.5)
                    score -= 10;
            }

            // Protein değerlendirmesi
            if (proteinPercentage.HasValue)
            {
                if (proteinPercentage < 2.8)
                    score -= 25;
                else if (proteinPercentage < 3.0)
                    score -= 10;
            }

            // Bakteri sayısı değerlendirmesi
            if (bacteriaCount.HasValue)
            {
                if (bacteriaCount > 100000)
                    score -= 30;
                else if (bacteriaCount > 50000)
                    score -= 15;
            }

            return Math.Max(score, 0);
        }

        private string GetQualityGrade(decimal score)
        {
            if (score >= 90) return "A";
            if (score >= 80) return "B";
            if (score >= 70) return "C";
            if (score >= 60) return "D";
            return "F";
        }

        private Dictionary<string, (decimal min, decimal max)> GetQualityStandards(string standardName)
        {
            // TS 1018 Türk Standardı
            return new Dictionary<string, (decimal, decimal)>
            {
                {"fat_percentage", (3.0m, 6.0m)},
                {"protein_percentage", (2.8m, 4.5m)},
                {"ph_value", (6.6m, 6.8m)},
                {"density", (1.028m, 1.034m)},
                {"bacteria_count", (0m, 100000m)},
                {"somatic_cell_count", (0m, 400000m)}
            };
        }

        private List<QualityIssue> CheckAnalysisQuality(MilkAnalysisResult analysis, Dictionary<string, (decimal min, decimal max)> standards)
        {
            var issues = new List<QualityIssue>();

            // Fat percentage kontrolü
            if (analysis.FatPercentage.HasValue && standards.ContainsKey("fat_percentage"))
            {
                var (min, max) = standards["fat_percentage"];
                if (analysis.FatPercentage < (double)min || analysis.FatPercentage > (double)max)
                {
                    issues.Add(new QualityIssue
                    {
                        AnalysisResultId = analysis.Id,
                        AnimalId = analysis.AnimalId,
                        IssueType = "composition",
                        Parameter = "Yağ Oranı",
                        ActualValue = (decimal)analysis.FatPercentage.Value,
                        ExpectedMin = min,
                        ExpectedMax = max,
                        Severity = GetSeverity((decimal)analysis.FatPercentage.Value, min, max),
                        Recommendation = "Yem içeriği ve hayvan sağlığı kontrol edilmeli"
                    });
                }
            }

            // Protein kontrolü
            if (analysis.ProteinPercentage.HasValue && standards.ContainsKey("protein_percentage"))
            {
                var (min, max) = standards["protein_percentage"];
                if (analysis.ProteinPercentage < (double)min || analysis.ProteinPercentage > (double)max)
                {
                    issues.Add(new QualityIssue
                    {
                        AnalysisResultId = analysis.Id,
                        AnimalId = analysis.AnimalId,
                        IssueType = "composition",
                        Parameter = "Protein Oranı",
                        ActualValue = (decimal)analysis.ProteinPercentage.Value,
                        ExpectedMin = min,
                        ExpectedMax = max,
                        Severity = GetSeverity((decimal)analysis.ProteinPercentage.Value, min, max),
                        Recommendation = "Beslenme programı gözden geçirilmeli"
                    });
                }
            }

            // Bakteri sayısı kontrolü
            if (analysis.TotalBacteriaCount.HasValue && standards.ContainsKey("bacteria_count"))
            {
                var (min, max) = standards["bacteria_count"];
                if (analysis.TotalBacteriaCount > max)
                {
                    issues.Add(new QualityIssue
                    {
                        AnalysisResultId = analysis.Id,
                        AnimalId = analysis.AnimalId,
                        IssueType = "microbiological",
                        Parameter = "Bakteri Sayısı",
                        ActualValue = analysis.TotalBacteriaCount.Value,
                        ExpectedMin = min,
                        ExpectedMax = max,
                        Severity = "High",
                        Recommendation = "Hijyen koşulları iyileştirilmeli, veteriner kontrolü gerekli"
                    });
                }
            }

            return issues;
        }

        private string GetSeverity(decimal actualValue, decimal min, decimal max)
        {
            var range = max - min;
            var deviation = Math.Max(Math.Abs(actualValue - min), Math.Abs(actualValue - max));
            
            if (deviation > range * 0.5m) return "Critical";
            if (deviation > range * 0.3m) return "High";
            if (deviation > range * 0.1m) return "Medium";
            return "Low";
        }

        private string GetOverallGrade(decimal compliancePercentage)
        {
            if (compliancePercentage >= 95) return "Mükemmel";
            if (compliancePercentage >= 85) return "İyi";
            if (compliancePercentage >= 70) return "Orta";
            if (compliancePercentage >= 50) return "Zayıf";
            return "Kritik";
        }
    }
}
