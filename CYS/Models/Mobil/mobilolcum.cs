namespace CYS.Models
{
	public class MobilOlcum
	{
		public int Id { get; set; }
		public string Rfid { get; set; }
		public float Weight { get; set; }
		public int CihazId { get; set; }
		public int AmacId { get; set; }
		public string? Amac { set; get; }
		public int HayvanId { get; set; }
		public DateTime Tarih { get; set; }
        public Hayvan? hayvan { get; set; }  // Nullable yapıyoruz
        public OlcumTipi OlcumTipi { get; set; } = OlcumTipi.Normal; // Varsayılan değer Normal olarak ayarlandı
    }
}
