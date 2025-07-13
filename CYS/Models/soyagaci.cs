namespace CYS.Models
{
	public class soyagaci
	{
		public int id { set; get; }
		public int ustHayvanId { set; get; }
		public Hayvan ustHayvan { set; get; }
		public int hayvanId { set; get; }
		public Hayvan hayvan { set; get; }

		public int isActive { set; get; }
	}
}
