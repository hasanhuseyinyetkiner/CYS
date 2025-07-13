namespace CYS.Models
{
	public class cihazrequest
	{
        public int id { get; set; }
		public int cihazId { get; set; }
        public string requestUrl { get; set; }
		public int requestResult { get; set; }
		public DateTime requestTime { set; get; }

	}
}
