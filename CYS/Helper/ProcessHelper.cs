using CYS.Models;

public class ProcessHelper
{
	private readonly processmanagementCTX _processCtx;

	public ProcessHelper()
	{
		_processCtx = new processmanagementCTX();
	}

	// Yeni süreç başlatırken girisMax ve cikisMin değerlerini alıyoruz
	public string BaslaVeYeniSurecEkle(float girisMax, float cikisMin, int cihazId, float kapi1Agirlik, float kapi2Agirlik, float kapi3Agirlik, int girisSonrasiBekleme, int giriskapikapandiktansonrakibekleme, int cikisKapisiSonrasiBekleme, int kupeOkumaSonrasiBekleme, int sonAgirlikBekleme)
	{
		// 1. CihazId'si belirtilen tüm kayıtların isTamamlandi değerini 1'e çeker
		string updateQuery = "UPDATE processmanagement SET isTamamlandi = 1 WHERE cihazId = @cihazId AND isTamamlandi = 0";
		_processCtx.GetAll(updateQuery, new { cihazId });

		// 2. Yeni bir processmanagement kaydı oluştur
		var yeniProcess = new processmanagement
		{
			guid = Guid.NewGuid().ToString(),
			hayvangirdi = 0,
			ilkkapikapandi = 0,
			kupeokundu = 0,
			okunankupe = "",
			sonagirlikalindimi = 0,
			sonagirlik = 0,
			cikiskapisiacildimi = 0,
			tarih = DateTime.Now,
			cikisbeklemeagirligi = cikisMin,  // Formdan gelen cikisMin değerini kullanıyoruz
			minimumhassasiyetagirlik = girisMax,  // Formdan gelen girisMax değerini kullanıyoruz
			cihazId = cihazId,  // Yeni süreç için cihaz ID'si
			isTamamlandi = 0,  // Yeni süreç başlatıldığı için tamamlanmadı
			mevcutmod = 1,  // Varsayılan mod
			hayvanid = -1,  // Henüz hayvan atanmadı
			kapi1agirlik = kapi1Agirlik,  // Yön 1 ağırlığı
			kapi2agirlik = kapi2Agirlik,  // Yön 2 ağırlığı
			kapi3agirlik = kapi3Agirlik,   // Yön 3 ağırlığı
										   // Yeni eklenen alanlar
			eklemeguncelleme = -1,
			girissonrasibekleme = girisSonrasiBekleme,
			giriskapikapandiktansonrakibekleme = giriskapikapandiktansonrakibekleme,
			cikiskapisisonrasibekleme = cikisKapisiSonrasiBekleme,
			kupeokumasonrasibekleme = kupeOkumaSonrasiBekleme,
			sonagirlikbekleme = sonAgirlikBekleme
		};

		// 3. Yeni süreci veritabanına ekle
		_processCtx.Add(yeniProcess);

		// 4. Eklenen yeni kaydın guid değerini döndür
		return yeniProcess.guid;
	}
}
