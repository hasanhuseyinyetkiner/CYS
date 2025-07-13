namespace CYS.Models
{
	public class process
	{
        public int id { get; set; }
		public int processTypeId { get; set; }
		public string requestId { get; set; }
        public DateTime baslangicZamani { get; set; }
		public DateTime bitisZamani { get; set; }
		public int tamamlandi { get; set; }

	}
}
