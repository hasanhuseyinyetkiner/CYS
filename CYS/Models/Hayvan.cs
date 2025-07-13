namespace CYS.Models
{
	public class Hayvan
	{
		public int id { get; set; }
		public int userId { get; set; }
		public User user { get; set; }
		public string rfidKodu { get; set; }
		public string kupeIsmi { get; set; }
		public string cinsiyet { get; set; }
		public string agirlik { get; set; }
		public int aktif { get; set; }
		public DateTime tarih { get; set; }
        public int kategoriId { get; set; }
		public Kategori kategori { set; get; }
		public List<HayvanKriterUnsur> ozellikler { get; set; }
		public string requestId { set; get; }
		public DateTime sonGuncelleme { get; set; }
		
		// Navigation properties
		public virtual ICollection<AgirlikOlcum> WeightMeasurements { get; set; } = new List<AgirlikOlcum>();
	}
}
