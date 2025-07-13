namespace CYS.Models
{
	public class HayvanKriterUnsur
	{
		public int id { get; set; }
		public int hayvanId { get; set; }
		public Hayvan hayvan { set; get; }
		public int kriterUnsurId { get; set; }
		public KriterUnsur kriterunsur { set; get; }
		public int isActive { get; set; }

	}
}
