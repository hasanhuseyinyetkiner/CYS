using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CYS.Models
{
    [Table("AktiviteLog")]
    public class AktiviteLog
    {
        [Key]
        public int Id { get; set; }
        
        public int KullaniciId { get; set; }
        
        [StringLength(50)]
        public string Islem { get; set; }
        
        [StringLength(50)]
        public string ModulAdi { get; set; }
        
        [StringLength(250)]
        public string IslemDetayi { get; set; }
        
        [StringLength(20)]
        public string IPAdresi { get; set; }
        
        [StringLength(100)]
        public string Tarayici { get; set; }
        
        public DateTime IslemTarihi { get; set; }
        
        [ForeignKey("KullaniciId")]
        public virtual Kullanici Kullanici { get; set; }
    }
} 