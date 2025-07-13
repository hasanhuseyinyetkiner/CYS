using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CYS.Models
{
    /// <summary>
    /// Normal ağırlık ölçümlerini temsil eden model sınıfı
    /// </summary>
    public class WeightMeasurement
    {
        [Key]
        public int Id { get; set; }
        
        public int? AnimalId { get; set; }
        
        [Required]
        public decimal Weight { get; set; }
        
        [Required]
        public DateTime MeasurementDate { get; set; } = DateTime.Now;
        
        public string RFID { get; set; }
        
        public string Notes { get; set; }
        
        public int? UserId { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("AnimalId")]
        public virtual Animal Animal { get; set; }
    }
} 