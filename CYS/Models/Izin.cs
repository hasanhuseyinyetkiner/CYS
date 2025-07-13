using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CYS.Models
{
    [Table("Izin")]
    public class Izin
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string IzinAdi { get; set; }
        
        [StringLength(100)]
        public string IzinKodu { get; set; }
        
        [StringLength(250)]
        public string Aciklama { get; set; }
        
        [StringLength(50)]
        public string Modul { get; set; }
        
        public bool Aktif { get; set; }
    }
} 