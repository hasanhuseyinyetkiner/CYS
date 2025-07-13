using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CYS.Models
{
    [Table("DogumYavru")]
    public class DogumYavru
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int DogumId { get; set; }
        
        [Required]
        public int HayvanId { get; set; }
        
        [StringLength(10)]
        public string Cinsiyet { get; set; }
        
        public double? DogumAgirligi { get; set; }
        
        [StringLength(100)]
        public string KupeNo { get; set; }
        
        [StringLength(500)]
        public string Notlar { get; set; }
        
        public bool Yasayan { get; set; }
        
        public DateTime KayitTarihi { get; set; }
        
        [ForeignKey("DogumId")]
        public virtual Dogum Dogum { get; set; }
        
        [ForeignKey("HayvanId")]
        public virtual Animal Hayvan { get; set; }
    }
} 