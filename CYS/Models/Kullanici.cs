using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CYS.Models
{
    [Table("Kullanici")]
    public class Kullanici
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string KullaniciAdi { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Email { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Sifre { get; set; }
        
        [StringLength(50)]
        public string Ad { get; set; }
        
        [StringLength(50)]
        public string Soyad { get; set; }
        
        [StringLength(20)]
        public string Telefon { get; set; }
        
        public int? RolId { get; set; }
        
        [StringLength(255)]
        public string ProfilResmi { get; set; }
        
        public DateTime KayitTarihi { get; set; }
        
        public DateTime? SonGirisTarihi { get; set; }
        
        public bool Aktif { get; set; }
        
        [StringLength(50)]
        public string Tema { get; set; }
        
        [StringLength(10)]
        public string Dil { get; set; }
    }
} 