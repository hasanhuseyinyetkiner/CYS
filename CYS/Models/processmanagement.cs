namespace CYS.Models
{
	public class processmanagement
	{
		public int id { get; set; }
		public string guid { get; set; }
		public int hayvangirdi { get; set; }
		public int ilkkapikapandi { get; set; }
		public int kupeokundu { get; set; }
		public string okunankupe { get; set; }
		public int sonagirlikalindimi { get; set; }
		public float sonagirlik { get; set; }
		public int cikiskapisiacildimi { get; set; }
		public DateTime tarih { get; set; }
		public float cikisbeklemeagirligi { get; set; }
		public float minimumhassasiyetagirlik { get; set; }
		public int cihazId { set; get; }
		public int isTamamlandi { set; get; }
		public DateTime	tamamlanmatarihi { set; get; }
		public int mevcutmod { set; get; }
		public int hayvanid { set; get; }
		public float kapi1agirlik { get; set; }
		public float kapi2agirlik { get; set; }
		public float kapi3agirlik { get; set; }
        public int eklemeguncelleme { get; set; }
        public int girissonrasibekleme { get; set; }
        public int giriskapikapandiktansonrakibekleme { get; set; }
        public int cikiskapisisonrasibekleme { get; set; }
        public int kupeokumasonrasibekleme { get; set; }
        public int sonagirlikbekleme { get; set; }


    }
}
