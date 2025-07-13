namespace CYS.Models
{
	public class surec
	{
		public int id { get; set; }
		public string requestId { get; set; }
		public int userId { set; get; }
		public User user { set; get; }
		public DateTime tarih { get; set; }
		public int tamamlandi { get; set; }
		public DateTime tamamlanmaTarihi { get; set; }

	}
}
