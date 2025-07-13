using System;

namespace CYS.Models
{
	public class hayvanmobil
	{
		public int id { get; set; }
		public string hayvanguid { get; set; }
		public string rfid { get; set; }
		public string ilkdogumagirligi { get; set; }
		public string suttenkesimagirligi { get; set; }
		public string mevcutagirlik { get; set; }
		public int kategoriid { get; set; }
		public string cinsiyet { get; set; }
		public string kupeismi { get; set; }
		public int userid { get; set; }
		public DateTime eklenmezamani { get; set; }
		public DateTime? ensonguncelleme { get; set; }
		public int aktif { get; set; }

		// Related entities
		public User user { get; set; }
		public Kategori kategori { get; set; }
		public List<HayvanKriterUnsur> ozellikler { get; set; }
	}
}
