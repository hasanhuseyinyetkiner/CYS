namespace CYS.Models
{
	public class sureclog
	{
		public int id { get; set; }
		public int processId { get; set; }
		public surec process { get; set; }
		public string fonksiyonAdi { get; set; }
		public string sorguSonucu { get; set; }
		public string sorguCevap{ get; set; }
		public DateTime tarih { get; set; }

	}
}
