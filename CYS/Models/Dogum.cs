using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace CYS.Models
{
    [Table("Dogum")]
    public class Dogum
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int AnneId { get; set; }
        
        public int? BabaId { get; set; }
        
        [Required]
        public DateTime DogumTarihi { get; set; }
        
        public int YavruSayisi { get; set; }
        
        [StringLength(500)]
        public string Notlar { get; set; }
        
        public bool OluDogum { get; set; }
        
        [StringLength(50)]
        public string DogumYeri { get; set; }
        
        public DateTime KayitTarihi { get; set; }
        
        public int KaydedenKullaniciId { get; set; }
        
        [ForeignKey("AnneId")]
        public virtual Animal Anne { get; set; }
        
        [ForeignKey("BabaId")]
        public virtual Animal Baba { get; set; }
        
        // Doğumla ilişkili yavrular listesi - DogumController için gerekli
        public List<DogumYavru> Yavrular { get; set; } = new List<DogumYavru>();
    }
} 