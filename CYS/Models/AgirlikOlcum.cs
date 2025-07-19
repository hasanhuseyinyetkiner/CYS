using System;

namespace CYS.Models
{
	public class AgirlikOlcum
	{
		public int id { get; set; }
		public int hayvanId { get; set; }
		public double agirlik { get; set; }
		public DateTime olcumTarihi { get; set; } = DateTime.Now;
		public bool bluetoothOlcum { get; set; }
		public int userId { get; set; }
		public string? olcumNotu { get; set; }
		
		// HayvanController'da kullanılan eksik alanlar
		public string? requestId { get; set; }
		public string? agirlikOlcumu { get; set; }
		public bool aktif { get; set; } = true; // Varsayılan olarak true
		public DateTime tarih { get; set; } = DateTime.Now; // Varsayılan olarak şu anki zaman
		
		// İlişkisel özellik
		public User? user { get; set; }
		
		// Properties for ApplicationDbContext
		public DateTime OlcumTarihi { get; set; }
		public virtual Animal? Hayvan { get; set; }
        
		// Ensure OlcumTarihi is initialized when the object is created
		public AgirlikOlcum()
		{
			OlcumTarihi = olcumTarihi;
		}
	}
}
