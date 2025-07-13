using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CYS.Models
{
    [Table("RolIzin")]
    public class RolIzin
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int RolId { get; set; }
        
        [Required]
        public int IzinId { get; set; }
        
        [ForeignKey("RolId")]
        public virtual Rol Rol { get; set; }
        
        [ForeignKey("IzinId")]
        public virtual Izin Izin { get; set; }
    }
} 