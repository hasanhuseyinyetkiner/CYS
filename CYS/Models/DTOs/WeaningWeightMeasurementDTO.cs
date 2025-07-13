using System;

namespace CYS.Models.DTOs
{
    /// <summary>
    /// Sütten kesim ağırlık ölçümleri için DTO sınıfı
    /// </summary>
    public class WeaningWeightMeasurementDTO
    {
        public int? Id { get; set; }
        
        public int? AnimalId { get; set; }
        
        public decimal Weight { get; set; }
        
        public DateTime? WeaningDate { get; set; }
        
        public int? WeaningAge { get; set; }
        
        public DateTime MeasurementDate { get; set; } = DateTime.Now;
        
        public string RFID { get; set; }
        
        public string MotherRFID { get; set; }
        
        public string Notes { get; set; }
        
        public int? UserId { get; set; }
        
        // İsteğe bağlı hayvan bilgileri
        public string AnimalName { get; set; }
        public string AnimalEarTag { get; set; }
        public string AnimalType { get; set; }
        
        // İsteğe bağlı anne hayvan bilgileri
        public string MotherName { get; set; }
        public string MotherEarTag { get; set; }
    }
} 