using CYS.Models;
using CYS.Repos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Plugins;

namespace CYS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProcessManagementController : ControllerBase
    {
        private readonly processmanagementCTX _context;

        public ProcessManagementController()
        {
            _context = new processmanagementCTX();
        }

        [HttpGet("{cihazId}")]
        public IActionResult GetProcessManagementByCihazId(int cihazId)
        {
            // Sorguyu cihazId ve isTamamlandi = 0 olan kayıtları kontrol etmek için yazıyoruz
            string sorgu = "SELECT id, mevcutMod, minimumhassasiyetagirlik, cikisbeklemeagirligi FROM processmanagement WHERE cihazId = @cihazId AND isTamamlandi = 0";
            var param = new { cihazId };

            var result = _context.Get(sorgu, param);

            if (result != null)
            {
                // Eğer kayıt varsa, id, mevcut mod ve hassasiyet ağırlıklarını döndürüyoruz
                return Ok(new
                {
                    id = result.id,
                    mevcutMod = result.mevcutmod,
                    girisHassasiyetAgirligi = result.minimumhassasiyetagirlik,
                    cikisHassasiyetAgirligi = result.cikisbeklemeagirligi,
                    kapi1agirlik = result.kapi1agirlik,
					kapi2agirlik = result.kapi2agirlik,
					kapi3agirlik = result.kapi3agirlik,
                    girissonrasibekleme = result.girissonrasibekleme,
                    giriskapikapandiktansonrakibekleme = result.giriskapikapandiktansonrakibekleme,
                    cikiskapisisonrasibekleme = result.cikiskapisisonrasibekleme,
                    kupeokumasonrasibekleme = result.kupeokumasonrasibekleme,
                    sonagirlikbekleme = result.sonagirlikbekleme
                });
            }
            else
            {
                // Eğer isTamamlandi = 0 olan bir kayıt yoksa, -1 geri döndür
                return Ok(new
                {
                    id = -1,
                    message = "İlgili cihazId için isTamamlandi = 0 olan bir kayıt bulunamadı."
                });
            }
        }

        public class AgirlikRequestModel
        {
            public float agirlik { get; set; }
            public int cihazId { get; set; }
        }
		[HttpPost]
		public IActionResult Post([FromBody] AgirlikRequestModel model)
		{
			// İlgili cihazId'deki aktif (isTamamlandi = 0) olan process'i kontrol ediyoruz
			string sorgu = "SELECT * FROM processmanagement WHERE cihazId = @cihazId AND isTamamlandi = 0";
			var param = new { model.cihazId };

			// Aktif process'i alıyoruz
			var result = _context.Get(sorgu, param);

			if (result != null)
			{
				// Eğer mevcut mod 6 ise ağırlık ölçümünü kaydediyoruz
				AgirlikOlcumCTX agirlikCTX = new AgirlikOlcumCTX();
				AgirlikOlcum yeniAgirlik = new AgirlikOlcum()
				{
					agirlikOlcumu = model.agirlik.ToString(),  // Ağırlık değeri string olarak kaydediliyor
					userId = 1,  // Varsayılan kullanıcı ID'si, bu ihtiyaçlarınıza göre güncellenebilir
					tarih = DateTime.Now,  // Kayıt zamanı
					requestId = result.guid  // processmanagement'taki guid, requestId olarak kullanılıyor
				};

				// Ağırlık ölçümünü kaydediyoruz
				agirlikCTX.agirlikOlcumEkle(yeniAgirlik);
				result.sonagirlik = model.agirlik;
				result.sonagirlikalindimi = 1;
				result.mevcutmod = 4;
				result.cikiskapisiacildimi = 1;
				_context.Update(result);

				// Hayvanın ağırlığını güncelle
				HayvanCTX hayvanCtx = new HayvanCTX();
				Hayvan hayvan = hayvanCtx.hayvanTek("select * from Hayvan WHERE id = @id", new { id = result.hayvanid });

				if (hayvan != null)
				{
					hayvan.agirlik = model.agirlik.ToString();
					hayvanCtx.hayvanGuncelle(hayvan);

					// AgirlikHayvan tablosuna yeni bir ağırlık kaydı ekle
					AgirlikHayvanCTX agirlikHayvanCtx = new AgirlikHayvanCTX();
					agirlikHayvan yeniAgirlikHayvan = new agirlikHayvan()
					{
						hayvanId = hayvan.id,
						agirlikId = model.agirlik.ToString(),  // Ağırlık değeri
						requestId = result.guid,  // Process GUID'i
						tarih = DateTime.Now
					};

					// Yeni ağırlık kaydını ekle
					agirlikHayvanCtx.agirlikHayvanEkle(yeniAgirlikHayvan);
				}

				return Ok(new
				{
					id = result.id,
					mevcutMod = result.mevcutmod,
					girisHassasiyetAgirligi = result.minimumhassasiyetagirlik,
					cikisHassasiyetAgirligi = result.cikisbeklemeagirligi
				});
			}
			else
			{
				// Eğer mevcut mod 6 değilse ya da process bulunamazsa hata döndürülür
				return BadRequest(new { message = "Ağırlık ölçümü yalnızca mevcut mod 6 iken yapılabilir." });
			}
		}


		[HttpPatch("ilkkapikapandi/{cihazId}")]
		public IActionResult UpdateIlkKapiKapandiAndHayvanGirdiByCihazId(int cihazId, [FromBody] int ilkkapikapandi)
		{
			// İlgili cihazId'deki processmanagement kaydını buluyoruz
			string sorgu = "SELECT * FROM processmanagement WHERE cihazId = @cihazId AND isTamamlandi = 0";
			var param = new { cihazId };

			var result = _context.Get(sorgu, param);

			if (result != null)
			{
				// ilkkapikapandi değerini güncelliyoruz ve hayvan girdi değerini 1 yapıyoruz
				result.ilkkapikapandi = ilkkapikapandi;
				result.hayvangirdi = 1;

				// Processmanagement tablosundaki kaydı güncelliyoruz
				_context.Update(result);

				// Güncellenen kaydın bilgilerini döndürüyoruz
				return Ok(new
				{
					id = result.id,
					cihazId = result.cihazId,
					ilkkapikapandi = result.ilkkapikapandi,
					hayvangirdi = result.hayvangirdi,
					message = "İlk kapı kapandı ve hayvan girdi olarak işaretlendi."
				});
			}
			else
			{
				// Eğer kayıt bulunamazsa hata döndürüyoruz
				return NotFound(new { message = "İlgili cihazId'ye ait aktif bir process kaydı bulunamadı." });
			}
		}
		public class KupeAtamaModel
		{
			public string kupeRfid { get; set; }
			public int cihazId { get; set; }
		}
		[HttpPost("kupe-atama-guncelle")]
		public IActionResult KupeAtamaGuncelle([FromBody] KupeAtamaModel model)

		{
			string processSorgu = "SELECT * FROM processmanagement WHERE cihazId = @cihazId AND isTamamlandi = 0";
            var processParam = new { model.cihazId };

            var processContext = new processmanagementCTX();
            var process = processContext.Get(processSorgu, processParam);

            // Process mevcut değilse veya mevcut mod 2 değilse hata döndür
            if (process == null )
            {
                return Ok(new
                {
                    message = "mod yanlış veya process bulunamadı"
                });
            }

            // 1. Küpe atanmış mı kontrol ediyoruz
            string sorgu = "SELECT * FROM kupehayvan WHERE kupeId = @kupeRfid";
            var param = new { model.kupeRfid };

            kupehayvanCTX kupeHayvanContext = new kupehayvanCTX();
            var kupeHayvan = kupeHayvanContext.kupehayvanTek(sorgu, param);

            if (kupeHayvan != null)
            {
                // 2. Küpe atanmışsa, processmanagement'taki hayvanId'yi ve diğer bilgileri güncelle
                process.hayvanid = kupeHayvan.hayvanId;
                process.okunankupe = model.kupeRfid;  // Okunan küpeyi güncelliyoruz
                process.kupeokundu = 1;  // Küpe okundu
                process.mevcutmod = 3;  // Mevcut modu 3 olarak güncelliyoruz
                processContext.Update(process);

                return Ok(new
                {
                    message = "Küpe zaten atanmıştı, hayvanId process'te güncellendi ve mevcut mod 3 olarak güncellendi.",
                    hayvanId = kupeHayvan.hayvanId,
                    cihazId = model.cihazId
                });
            }
            else
            {
                // 3. Küpe atanmadıysa yeni hayvan ekle
                HayvanCTX hayvanContext = new HayvanCTX();
                Hayvan yeniHayvan = new Hayvan()
                {
                    rfidKodu = model.kupeRfid,
                    kupeIsmi = "Otomatik-"+DateTime.Now.ToString("dd.MM.yyyy hh:mm"),  // Küpe ismi küpe RFID ile aynı olabilir
                    cinsiyet = "Bilinmiyor",  // Varsayılan olarak bilinmiyor olabilir
                    agirlik = "-1",  // Varsayılan olarak bilinmiyor olabilir
                    userId = 1,  // Kullanıcı ID'si varsayılan olarak belirlenebilir
                    kategoriId = 1,  // Varsayılan kategori
                    requestId = process.guid,  // Unique requestId
                    tarih = DateTime.Now,
                    aktif = 1
                };

                // Yeni hayvanı ekliyoruz
                int eklenenid = hayvanContext.hayvanEkle(yeniHayvan);

                // Yeni hayvanı kupehayvan tablosuna ekliyoruz
                kupehayvan yeniKupeHayvan = new kupehayvan()
                {
                    hayvanId = eklenenid,
                    kupeId = model.kupeRfid,
                    requestId = yeniHayvan.requestId,
                    tarih = DateTime.Now
                };

                kupeHayvanContext.kupehayvanEkle(yeniKupeHayvan);

                // 4. processmanagement'ta hayvan ve okunankupe alanlarını güncelle
                process.hayvanid = eklenenid;
                process.okunankupe = model.kupeRfid;  // Okunan küpeyi güncelliyoruz
                process.kupeokundu = 1;  // Küpe okundu
                process.mevcutmod = 3;  // Mevcut modu 3 olarak güncelliyoruz
                processContext.Update(process);

                return Ok(new
                {
                    message = "Yeni hayvan oluşturuldu, küpe bilgisi güncellendi ve mevcut mod 3 olarak güncellendi.",
                    hayvanId = eklenenid,
                    cihazId = model.cihazId
                });
            }
        }

		[HttpPost("tamamla/{cihazId}")]
		public IActionResult UpdateIsTamamlandiAndTamamlanmaZamaniByCihazId(int cihazId)
		{
			try
			{
				// İlgili cihazId'deki aktif (isTamamlandi = 0) olan processmanagement kaydını buluyoruz
				string sorgu = "SELECT * FROM processmanagement WHERE cihazId = @cihazId AND isTamamlandi = 0";
				var param = new { cihazId };

				var result = _context.Get(sorgu, param);

				if (result == null)
				{
					// Eğer kayıt bulunamazsa hata döndürüyoruz
					return NotFound(new { message = "İlgili cihazId'ye ait aktif bir process kaydı bulunamadı." });
				}

				// isTamamlandi'yi 1 yapıyoruz ve tamamlanma zamanını güncelliyoruz
				result.isTamamlandi = 1;
				result.tamamlanmatarihi = DateTime.Now;

				// Processmanagement tablosundaki kaydı güncelliyoruz
				_context.Update(result);

				// Güncellenen kaydın bilgilerini döndürüyoruz
				return Ok(new
				{
					id = result.id,
					cihazId = result.cihazId,
					isTamamlandi = result.isTamamlandi,
					tamamlanmatarihi = result.tamamlanmatarihi,
					message = "Process başarıyla tamamlandı ve güncellendi."
				});
			}
			catch (Exception ex)
			{
				// Hata durumunda hata mesajı döndürüyoruz
				return StatusCode(500, new { message = "Bir hata oluştu", error = ex.Message });
			}
		}




	}
}
