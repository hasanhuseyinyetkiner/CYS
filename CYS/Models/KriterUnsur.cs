namespace CYS.Models
{
	public class KriterUnsur
	{
		public int id { get; set; }
		public int kriterId { get; set; }
		public Kriter kriter { set; get; }
		public string unsurAdi { get; set; }
		public int isActive { get; set; }


	}
}
