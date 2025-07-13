namespace CYS.Models
{
	public class processsetting
	{
        public int id { get; set; }
        public string mevcutRequest { get; set; }
		public int oncekiIslem { get; set; }
		public int mevcutIslem { get; set; }
		public DateTime guncellemeZamani { get; set; }
		public int islemModu { set; get; }

	}
}
