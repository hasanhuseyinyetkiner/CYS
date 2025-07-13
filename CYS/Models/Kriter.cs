namespace CYS.Models
{
	public class Kriter
	{
		public int id { get; set; }
		public string kriterAdi { get; set; }
		public int isActive { get; set; }
		public string badge { set; get; }
		public List<KriterUnsur> unsurlar { get; set;}
	}
}
