namespace CYS.Models
{
	public class olcum
	{
		public int id { set; get; }
		public int olcumSessionId { set; get; }
		public olcumSession olcumSession { set; get; }
		public int adet { set; get; }
		public DateTime sonGuncelleme { set; get; }
	}
}
