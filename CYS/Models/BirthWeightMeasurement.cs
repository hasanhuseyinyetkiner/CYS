using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CYS.Models
{
    /// <summary>
    /// Doğum ağırlık ölçümlerini temsil eden model sınıfı
    /// </summary>
    public class BirthWeightMeasurement
    {
        [Key]
        public int Id { get; set; }
        
        public int? AnimalId { get; set; }
        
        [Required]
        public decimal Weight { get; set; }
        
        public DateTime? BirthDate { get; set; }
        
        public string BirthPlace { get; set; }
        
        [Required]
        public DateTime MeasurementDate { get; set; } = DateTime.Now;
        
        public string RFID { get; set; }
        
        public string MotherRFID { get; set; }
        
        public string Notes { get; set; }
        
        public int? UserId { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("AnimalId")]
        public virtual Animal Animal { get; set; }
    }
} 