using System;

namespace CYS.Models
{
	public class DogumAgirlikOlcum
	{
		public int id { get; set; }
		public int hayvanId { get; set; }
		public double agirlik { get; set; }
		public DateTime dogumTarihi { get; set; }
		public DateTime olcumTarihi { get; set; }
		public bool bluetoothOlcum { get; set; }
		public int userId { get; set; }
		public string? olcumNotu { get; set; }
		public string? anneRfid { get; set; }
		public string? dogumYeri { get; set; }
		
		// Request tracking
		public string? requestId { get; set; }
		public string? agirlikOlcumu { get; set; }
		public bool aktif { get; set; } = true;
		public DateTime tarih { get; set; } = DateTime.Now;
		
		// İlişkisel özellik
		public User? user { get; set; }
		public Hayvan? hayvan { get; set; }
	}
} 