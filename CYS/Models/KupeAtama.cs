using System.Security.Policy;

namespace CYS.Models
{
	public class KupeAtama
	{
		public int id { get; set; }
		public int userId { get; set; }
		public User user { get; set; }
		public string kupeRfid { get; set; }
		public int aktif { get; set; }
		public string requestId { set; get; }
		public DateTime tarih { get; set; }

	}
}
