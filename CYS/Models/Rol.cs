using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CYS.Models
{
    [Table("Rol")]
    public class Rol
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string RolAdi { get; set; }
        
        [StringLength(250)]
        public string Aciklama { get; set; }
        
        public DateTime OlusturmaTarihi { get; set; }
        
        public bool Aktif { get; set; }
    }
} 