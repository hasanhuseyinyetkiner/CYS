using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CYS.Models
{
    [Table("SyncLog")]
    public class SyncLog
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int KullaniciId { get; set; }
        
        [Required]
        public DateTime SyncTarihi { get; set; }
        
        [StringLength(20)]
        public string CihazId { get; set; }
        
        [StringLength(100)]
        public string CihazAdi { get; set; }
        
        [StringLength(20)]
        public string CihazIP { get; set; }
        
        public int YuklenenKayitSayisi { get; set; }
        
        public int IndirilenKayitSayisi { get; set; }
        
        [StringLength(50)]
        public string SyncDurumu { get; set; }
        
        [StringLength(500)]
        public string HataMesaji { get; set; }
        
        [ForeignKey("KullaniciId")]
        public virtual Kullanici Kullanici { get; set; }
    }
} 