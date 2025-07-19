using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CYS.Models
{
    [Table("TestWeightMeasurements")]
    public class TestWeightMeasurement
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Required]
        public int AnimalId { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Weight { get; set; }
        
        [StringLength(50)]
        public string Rfid { get; set; }
        
        [StringLength(500)]
        public string Notes { get; set; }
        
        [StringLength(100)]
        public string Source { get; set; } // Verinin hangi uygulamadan geldiğini belirtir (C++, Flutter, vb.)
        
        public DateTime MeasurementDate { get; set; } = DateTime.Now;
    }
} 