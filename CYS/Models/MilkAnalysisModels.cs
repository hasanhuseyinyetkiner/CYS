using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CYS.Models
{
    /// <summary>
    /// Süt analiz sonuçları modeli - PostgreSQL'den gelen veriler için
    /// </summary>
    [Table("milk_analysis_results")]
    public class MilkAnalysisResult
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("sample_id")]
        public int SampleId { get; set; }

        [Column("animal_id")]
        public int AnimalId { get; set; }

        [Column("analysis_date")]
        public DateTime AnalysisDate { get; set; }

        [Column("analyzed_by")]
        [StringLength(100)]
        public string? AnalyzedBy { get; set; }

        [Column("analysis_method")]
        [StringLength(50)]
        public string? AnalysisMethod { get; set; }

        // Kimyasal bileşim
        [Column("ph_value")]
        public double? PhValue { get; set; }

        [Column("fat_percentage")]
        public double? FatPercentage { get; set; }

        [Column("protein_percentage")]
        public double? ProteinPercentage { get; set; }

        [Column("lactose_percentage")]
        public double? LactosePercentage { get; set; }

        [Column("mineral_percentage")]
        public double? MineralPercentage { get; set; }

        [Column("water_percentage")]
        public double? WaterPercentage { get; set; }

        // Fiziksel özellikler
        [Column("density")]
        public double? Density { get; set; }

        [Column("temperature")]
        public double? Temperature { get; set; }

        [Column("viscosity")]
        public double? Viscosity { get; set; }

        [Column("conductivity")]
        public double? Conductivity { get; set; }

        // Mikrobiyolojik
        [Column("total_bacteria_count")]
        public int? TotalBacteriaCount { get; set; }

        [Column("somatic_cell_count")]
        public int? SomaticCellCount { get; set; }

        // Kalite göstergeleri
        [Column("freezing_point")]
        public double? FreezingPoint { get; set; }

        [Column("acidity_level")]
        public double? AcidityLevel { get; set; }

        [Column("peroxide_value")]
        public double? PeroxideValue { get; set; }

        // Ek parametreler
        [Column("urea_content")]
        public double? UreaContent { get; set; }

        [Column("added_water_percentage")]
        public double? AddedWaterPercentage { get; set; }

        [Column("salt_content")]
        public double? SaltContent { get; set; }

        // Metadata
        [Column("analysis_duration_seconds")]
        public int? AnalysisDurationSeconds { get; set; }

        [Column("instrument_id")]
        [StringLength(50)]
        public string? InstrumentId { get; set; }

        [Column("calibration_date")]
        public DateTime? CalibrationDate { get; set; }

        // Validasyon
        [Column("is_valid")]
        public bool IsValid { get; set; } = true;

        [Column("validation_notes")]
        public string? ValidationNotes { get; set; }

        [Column("reviewed_by")]
        [StringLength(100)]
        public string? ReviewedBy { get; set; }

        [Column("reviewed_at")]
        public DateTime? ReviewedAt { get; set; }

        // Zaman damgaları
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("AnimalId")]
        public virtual Animal? Animal { get; set; }

        [ForeignKey("SampleId")]
        public virtual MilkSample? Sample { get; set; }
    }

    /// <summary>
    /// Süt örnekleri modeli - PostgreSQL'den gelen veriler için
    /// </summary>
    [Table("milk_samples")]
    public class MilkSample
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("sample_id")]
        [StringLength(50)]
        public string SampleId { get; set; } = string.Empty;

        [Column("animal_id")]
        public int AnimalId { get; set; }

        [Column("collected_at")]
        public DateTime CollectedAt { get; set; }

        [Column("collected_by")]
        [StringLength(100)]
        public string? CollectedBy { get; set; }

        [Column("sample_type")]
        [StringLength(50)]
        public string? SampleType { get; set; }

        [Column("source_location")]
        [StringLength(100)]
        public string? SourceLocation { get; set; }

        [Column("collection_temperature")]
        public double? CollectionTemperature { get; set; }

        [Column("collection_notes")]
        public string? CollectionNotes { get; set; }

        [Column("status")]
        [StringLength(20)]
        public string Status { get; set; } = "pending";

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("AnimalId")]
        public virtual Animal? Animal { get; set; }

        public virtual ICollection<MilkAnalysisResult> AnalysisResults { get; set; } = new List<MilkAnalysisResult>();
    }

    /// <summary>
    /// Süt kalite değerlendirme modeli
    /// </summary>
    [Table("milk_quality_assessments")]
    public class MilkQualityAssessment
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("analysis_result_id")]
        public int AnalysisResultId { get; set; }

        [Column("overall_quality_score")]
        public double OverallQualityScore { get; set; }

        [Column("quality_grade")]
        [StringLength(10)]
        public string QualityGrade { get; set; } = string.Empty;

        [Column("composition_score")]
        public double? CompositionScore { get; set; }

        [Column("hygiene_score")]
        public double? HygieneScore { get; set; }

        [Column("freshness_score")]
        public double? FreshnessScore { get; set; }

        [Column("adulteration_detected")]
        public bool AdulterationDetected { get; set; }

        [Column("assessment_notes")]
        public string? AssessmentNotes { get; set; }

        [Column("assessed_by")]
        [StringLength(100)]
        public string? AssessedBy { get; set; }

        [Column("assessed_at")]
        public DateTime AssessedAt { get; set; } = DateTime.Now;

        // Navigation property
        [ForeignKey("AnalysisResultId")]
        public virtual MilkAnalysisResult? AnalysisResult { get; set; }
    }

    /// <summary>
    /// Enstrüman bilgileri modeli
    /// </summary>
    [Table("instruments")]
    public class Instrument
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("instrument_id")]
        [StringLength(50)]
        public string InstrumentId { get; set; } = string.Empty;

        [Column("instrument_name")]
        [StringLength(100)]
        public string InstrumentName { get; set; } = string.Empty;

        [Column("model")]
        [StringLength(50)]
        public string? Model { get; set; }

        [Column("manufacturer")]
        [StringLength(50)]
        public string? Manufacturer { get; set; }

        [Column("serial_number")]
        [StringLength(100)]
        public string? SerialNumber { get; set; }

        [Column("last_calibration_date")]
        public DateTime? LastCalibrationDate { get; set; }

        [Column("next_calibration_due")]
        public DateTime? NextCalibrationDue { get; set; }

        [Column("calibration_interval_days")]
        public int CalibrationIntervalDays { get; set; } = 365;

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("location")]
        [StringLength(100)]
        public string? Location { get; set; }

        [Column("notes")]
        public string? Notes { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual ICollection<MilkAnalysisResult> AnalysisResults { get; set; } = new List<MilkAnalysisResult>();
    }

    /// <summary>
    /// Sync durumu takip modeli
    /// </summary>
    [Table("sync_status")]
    public class SyncStatus
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("table_name")]
        [StringLength(50)]
        public string TableName { get; set; } = string.Empty;

        [Column("last_sync_time")]
        public DateTime LastSyncTime { get; set; }

        [Column("sync_direction")]
        [StringLength(20)]
        public string SyncDirection { get; set; } = string.Empty;

        [Column("records_synced")]
        public int RecordsSynced { get; set; }

        [Column("sync_status")]
        [StringLength(20)]
        public string Status { get; set; } = string.Empty;

        [Column("error_message")]
        public string? ErrorMessage { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
