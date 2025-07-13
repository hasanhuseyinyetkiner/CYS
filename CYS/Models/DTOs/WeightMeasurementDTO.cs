using System;

namespace CYS.Models.DTOs
{
    /// <summary>
    /// Normal ağırlık ölçümleri için DTO sınıfı
    /// </summary>
    public class WeightMeasurementDTO
    {
        public int? Id { get; set; }
        
        public int? AnimalId { get; set; }
        
        public decimal Weight { get; set; }
        
        public DateTime MeasurementDate { get; set; } = DateTime.Now;
        
        public string RFID { get; set; }
        
        public string Notes { get; set; }
        
        public int? UserId { get; set; }
        
        // İsteğe bağlı hayvan bilgileri
        public string AnimalName { get; set; }
        public string AnimalEarTag { get; set; }
        public string AnimalType { get; set; }
    }
} 