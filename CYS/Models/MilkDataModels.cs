using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CYS.Models
{
    /// <summary>
    /// Süt analiz sonucu DTO - API üzerinden veri transferi için
    /// </summary>
    public class MilkAnalysisResultDto
    {
        [Required]
        public int AnimalId { get; set; }

        [Required]
        public DateTime AnalysisDate { get; set; }

        [StringLength(50)]
        public string? AnalysisMethod { get; set; }

        [Range(0, 14)]
        public double? PhValue { get; set; }

        [Range(0, 10)]
        public double? FatPercentage { get; set; }

        [Range(0, 10)]
        public double? ProteinPercentage { get; set; }

        [Range(0, 10)]
        public double? LactosePercentage { get; set; }

        [Range(0, 5)]
        public double? MineralPercentage { get; set; }

        [Range(0, 100)]
        public double? WaterPercentage { get; set; }

        [Range(1.0, 1.1)]
        public double? Density { get; set; }

        [Range(-10, 50)]
        public double? Temperature { get; set; }

        public double? Viscosity { get; set; }
        public double? Conductivity { get; set; }

        [Range(0, int.MaxValue)]
        public int? TotalBacteriaCount { get; set; }

        [Range(0, int.MaxValue)]
        public int? SomaticCellCount { get; set; }

        public double? FreezingPoint { get; set; }
        public double? AcidityLevel { get; set; }
        public double? PeroxideValue { get; set; }
        public double? UreaContent { get; set; }
        public double? AddedWaterPercentage { get; set; }
        public double? SaltContent { get; set; }

        [Range(0, 3600)]
        public int? AnalysisDurationSeconds { get; set; }

        [StringLength(50)]
        public string? InstrumentId { get; set; }

        public string? ValidationNotes { get; set; }
    }

    /// <summary>
    /// Süt örneki DTO
    /// </summary>
    public class MilkSampleDto
    {
        [Required]
        [StringLength(50)]
        public string SampleId { get; set; } = string.Empty;

        [Required]
        public int AnimalId { get; set; }

        [Required]
        public DateTime CollectedAt { get; set; }

        [StringLength(50)]
        public string? SampleType { get; set; }

        [StringLength(100)]
        public string? SourceLocation { get; set; }

        [Range(-10, 50)]
        public double? CollectionTemperature { get; set; }

        public string? CollectionNotes { get; set; }
    }

    /// <summary>
    /// Hayvan süt istatistikleri
    /// </summary>
    public class AnimalMilkStatistics
    {
        public int AnimalId { get; set; }
        public string? AnimalName { get; set; }
        public string? RfidTag { get; set; }
        public decimal AverageDailyMilkYield { get; set; }
        public decimal TotalMilkYield { get; set; }
        public decimal MaxDailyMilkYield { get; set; }
        public decimal MinDailyMilkYield { get; set; }
        public decimal? AverageFatPercentage { get; set; }
        public decimal? AverageProteinPercentage { get; set; }
        public decimal? AverageLactosePercentage { get; set; }
        public decimal? AveragePhValue { get; set; }
        public double? AverageBacteriaCount { get; set; }
        public double? AverageSomaticCellCount { get; set; }
        public DateTime? FirstAnalysisDate { get; set; }
        public DateTime? LastAnalysisDate { get; set; }
        public int TotalAnalysisCount { get; set; }
        public decimal AverageQualityScore { get; set; }
        public string QualityGrade { get; set; } = string.Empty;
    }

    /// <summary>
    /// Toplu veri senkronizasyon isteği
    /// </summary>
    public class MilkDataSyncRequest
    {
        public List<MilkAnalysisResultDto> AnalysisResults { get; set; } = new();
        public List<MilkSampleDto> Samples { get; set; } = new();
        public DateTime SyncTimestamp { get; set; }
        public string ClientId { get; set; } = string.Empty;
        public string SyncVersion { get; set; } = "2.0";
    }

    /// <summary>
    /// Senkronizasyon sonucu
    /// </summary>
    public class SyncResult
    {
        public bool Success { get; set; }
        public int ProcessedRecords { get; set; }
        public int SuccessfulRecords { get; set; }
        public int FailedRecords { get; set; }
        public List<string> Errors { get; set; } = new();
        public DateTime SyncCompletedAt { get; set; }
        public string SyncId { get; set; } = Guid.NewGuid().ToString();
    }

    /// <summary>
    /// Kalite kontrol isteği
    /// </summary>
    public class QualityCheckRequest
    {
        [Required]
        public List<int> AnalysisResultIds { get; set; } = new();
        
        [StringLength(50)]
        public string QualityStandardName { get; set; } = "TS_1018";
        
        public bool IncludeDetailedReport { get; set; } = true;
    }

    /// <summary>
    /// Kalite kontrol sonucu
    /// </summary>
    public class QualityCheckResult
    {
        public int TotalChecked { get; set; }
        public int PassedCount { get; set; }
        public int FailedCount { get; set; }
        public decimal CompliancePercentage { get; set; }
        public string OverallGrade { get; set; } = string.Empty;
        public List<QualityIssue> Issues { get; set; } = new();
        public DateTime CheckedAt { get; set; } = DateTime.UtcNow;
        public string QualityStandard { get; set; } = string.Empty;
    }

    /// <summary>
    /// Kalite sorunu detayı
    /// </summary>
    public class QualityIssue
    {
        public int AnalysisResultId { get; set; }
        public int AnimalId { get; set; }
        public string IssueType { get; set; } = string.Empty; // composition, microbiological, physical
        public string Parameter { get; set; } = string.Empty;
        public decimal ActualValue { get; set; }
        public decimal ExpectedMin { get; set; }
        public decimal ExpectedMax { get; set; }
        public string Severity { get; set; } = string.Empty; // Low, Medium, High, Critical
        public string Recommendation { get; set; } = string.Empty;
        public DateTime DetectedAt { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// API yanıt wrapper'ı
    /// </summary>
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<string> Errors { get; set; } = new();
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string RequestId { get; set; } = Guid.NewGuid().ToString();
    }

    /// <summary>
    /// Sayfalama için API yanıt
    /// </summary>
    public class PagedApiResponse<T> : ApiResponse<T>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
    }

    /// <summary>
    /// Ağırlık ölçümü DTO
    /// </summary>
    public class WeightMeasurementDto
    {
        [Required]
        public int AnimalId { get; set; }

        [Required]
        [Range(0.1, 2000.0)]
        public double Weight { get; set; }

        [Required]
        public DateTime MeasurementDate { get; set; }

        public bool BluetoothMeasurement { get; set; }
        public int? UserId { get; set; }
        public string? MeasurementNotes { get; set; }
        public string? RequestId { get; set; }
        public string? MeasurementType { get; set; }
        public string? Notes { get; set; }
    }

    /// <summary>
    /// Hayvan arama/filtreleme parametreleri
    /// </summary>
    public class AnimalSearchRequest
    {
        public string? RfidTag { get; set; }
        public string? Name { get; set; }
        public string? Breed { get; set; }
        public string? Status { get; set; }
        public DateTime? BirthDateFrom { get; set; }
        public DateTime? BirthDateTo { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public bool IncludeInactive { get; set; } = false;
    }

    /// <summary>
    /// Hayvan DTO
    /// </summary>
    public class AnimalDto
    {
        public int Id { get; set; }
        public string RfidTag { get; set; } = string.Empty;
        public string? EarTag { get; set; }
        public string? Name { get; set; }
        public string? Breed { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? Gender { get; set; }
        public double? InitialWeight { get; set; }
        public double? CurrentWeight { get; set; }
        public string Status { get; set; } = "active";
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        // İlişkili veriler
        public int TotalWeightMeasurements { get; set; }
        public int TotalMilkAnalyses { get; set; }
        public DateTime? LastWeightMeasurement { get; set; }
        public DateTime? LastMilkAnalysis { get; set; }
    }

    /// <summary>
    /// Sistem durumu bilgisi
    /// </summary>
    public class SystemStatus
    {
        public bool IsOnline { get; set; }
        public DateTime LastSyncTime { get; set; }
        public int PendingUploads { get; set; }
        public int PendingDownloads { get; set; }
        public string ConnectionStatus { get; set; } = string.Empty;
        public string LastError { get; set; } = string.Empty;
        public Dictionary<string, object> Statistics { get; set; } = new();
    }

    /// <summary>
    /// Batch işlem sonucu
    /// </summary>
    public class BatchProcessResult
    {
        public int TotalItems { get; set; }
        public int ProcessedItems { get; set; }
        public int SuccessfulItems { get; set; }
        public int FailedItems { get; set; }
        public List<string> Errors { get; set; } = new();
        public TimeSpan ProcessingTime { get; set; }
        public DateTime CompletedAt { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Veri validasyon sonucu
    /// </summary>
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new();
        public List<string> Warnings { get; set; } = new();
        public Dictionary<string, object> ValidationDetails { get; set; } = new();
    }



    /// <summary>
    /// Enstrüman kalibrasyon bilgisi
    /// </summary>
    public class InstrumentCalibration
    {
        public int Id { get; set; }
        public string InstrumentId { get; set; } = string.Empty;
        public DateTime CalibrationDate { get; set; }
        public DateTime NextCalibrationDue { get; set; }
        public string CalibrationType { get; set; } = string.Empty;
        public bool IsValid { get; set; }
        public string CalibrationResult { get; set; } = string.Empty;
        public string CalibratedBy { get; set; } = string.Empty;
        public Dictionary<string, double> CalibrationValues { get; set; } = new();
        public string Notes { get; set; } = string.Empty;
    }

    /// <summary>
    /// Günlük süt üretim raporu
    /// </summary>
    public class DailyMilkProductionReport
    {
        public DateTime Date { get; set; }
        public int TotalAnimals { get; set; }
        public int MilkedAnimals { get; set; }
        public double TotalMilkVolume { get; set; }
        public double AverageMilkPerAnimal { get; set; }
        public double AverageFatPercentage { get; set; }
        public double AverageProteinPercentage { get; set; }
        public double AverageQualityScore { get; set; }
        public string OverallQualityGrade { get; set; } = string.Empty;
        public int QualityIssuesCount { get; set; }
        public List<string> QualityIssues { get; set; } = new();
        public Dictionary<string, double> ProductionMetrics { get; set; } = new();
    }

    /// <summary>
    /// Süt kalite trend bilgisi
    /// </summary>
    public class MilkQualityTrend
    {
        public int AnimalId { get; set; }
        public string AnimalName { get; set; } = string.Empty;
        public string RfidTag { get; set; } = string.Empty;
        public List<TrendDataPoint> FatPercentageTrend { get; set; } = new();
        public List<TrendDataPoint> ProteinPercentageTrend { get; set; } = new();
        public List<TrendDataPoint> QualityScoreTrend { get; set; } = new();
        public List<TrendDataPoint> BacteriaCountTrend { get; set; } = new();
        public string OverallTrend { get; set; } = string.Empty; // improving, declining, stable
        public double TrendScore { get; set; }
        public DateTime AnalysisPeriodStart { get; set; }
        public DateTime AnalysisPeriodEnd { get; set; }
        public List<string> TrendInsights { get; set; } = new();
    }

    /// <summary>
    /// Trend veri noktası
    /// </summary>
    public class TrendDataPoint
    {
        public DateTime Date { get; set; }
        public double Value { get; set; }
        public string Label { get; set; } = string.Empty;
    }

    /// <summary>
    /// Alert/Alarm bilgisi
    /// </summary>
    public class MilkQualityAlert
    {
        public int Id { get; set; }
        public int AnimalId { get; set; }
        public string AnimalName { get; set; } = string.Empty;
        public string RfidTag { get; set; } = string.Empty;
        public string AlertType { get; set; } = string.Empty; // quality, health, production
        public string Severity { get; set; } = string.Empty; // low, medium, high, critical
        public string Parameter { get; set; } = string.Empty;
        public double CurrentValue { get; set; }
        public double ThresholdValue { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Recommendation { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsResolved { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public string? ResolvedBy { get; set; }
        public string? ResolutionNotes { get; set; }
    }

    /// <summary>
    /// Toplu işlem isteği
    /// </summary>
    public class BulkOperationRequest
    {
        public string OperationType { get; set; } = string.Empty; // sync, update, delete, validate
        public List<int> AnimalIds { get; set; } = new();
        public Dictionary<string, object> Parameters { get; set; } = new();
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
        public string RequestedBy { get; set; } = string.Empty;
        public bool ValidateOnly { get; set; } = false;
    }

    /// <summary>
    /// Toplu işlem sonucu
    /// </summary>
    public class BulkOperationResult
    {
        public string OperationType { get; set; } = string.Empty;
        public int TotalRequested { get; set; }
        public int Processed { get; set; }
        public int Successful { get; set; }
        public int Failed { get; set; }
        public List<string> Errors { get; set; } = new();
        public List<string> Warnings { get; set; } = new();
        public Dictionary<string, object> Results { get; set; } = new();
        public TimeSpan ProcessingTime { get; set; }
        public DateTime CompletedAt { get; set; }
    }

    /// <summary>
    /// Rapor isteği
    /// </summary>
    public class ReportRequest
    {
        public string ReportType { get; set; } = string.Empty; // daily, weekly, monthly, custom
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<int> AnimalIds { get; set; } = new();
        public List<string> ReportSections { get; set; } = new();
        public string Format { get; set; } = "json"; // json, pdf, excel, csv
        public Dictionary<string, object> Parameters { get; set; } = new();
        public bool IncludeCharts { get; set; } = true;
        public string Language { get; set; } = "tr";
    }

    /// <summary>
    /// Rapor sonucu
    /// </summary>
    public class ReportResult
    {
        public string ReportId { get; set; } = Guid.NewGuid().ToString();
        public string ReportType { get; set; } = string.Empty;
        public DateTime GeneratedAt { get; set; }
        public string Format { get; set; } = string.Empty;
        public byte[]? ReportData { get; set; }
        public string? ReportUrl { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new();
        public List<string> Warnings { get; set; } = new();
        public TimeSpan GenerationTime { get; set; }
    }
}
