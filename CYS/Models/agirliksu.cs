namespace CYS.Models
{
	public class agirliksu
	{
        public int id { get; set; }
		public int userId { get; set; }
		public User user { set; get; }
		public Hayvan hayvan { set; get; }
		public int hayvanId { get; set; }
        public string ilkOlcum { get; set; }
		public string sonOlcum { get; set; }
		public DateTime tarih { set; get; }
        public string reqestId { get; set; }
		public int hayvangirdi { set; get; }
		public int hayvancikti { set; get; }
		public int hayvanui { set; get; }


	}
}
