using CYS.Models;
using CYS.Repos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CYS.Controllers.WebApis
{
	[Route("api/[controller]")]
	[ApiController]
	public class HayvanApiController : ControllerBase
	{
		public class postCevap
		{
			public int isSynced { set; get; }
			public int insertUpdate { set; get; }
			public string message { set; get; }
		}
		[HttpGet]
		public IEnumerable<Hayvan> Get(int userId)
		{
			HayvanCTX hctx = new HayvanCTX();
			return hctx.hayvanList("select * from Hayvan where userId = @userId", new {userId = userId});
		}

		[HttpPost]
		public postCevap hayvanEkle(string rfidKodu, string kupeIsmi, string cinsiyet, string agirlik, int userId, int kategoriId, string requestId, DateTime dt )
		{
			postCevap pc = new postCevap();
			HayvanCTX hctx = new HayvanCTX();
			AgirlikHayvanCTX ahctx = new AgirlikHayvanCTX();
			kupehayvanCTX kaCTX = new kupehayvanCTX();
			var hayvanVarMi = hctx.hayvanTekSadece("select * from Hayvan where rfidKodu = @rfidKodu and aktif = 1", new { rfidKodu = rfidKodu });
			if(hayvanVarMi != null)
			{
				if(hayvanVarMi.requestId == requestId || hayvanVarMi.sonGuncelleme > dt)
				{
					pc.isSynced = -1;
					pc.insertUpdate = 0;
					pc.message = "Bu güncelleme zaten mevcut";
					return pc;
				}
				hayvanVarMi.rfidKodu = rfidKodu;
				hayvanVarMi.kupeIsmi = kupeIsmi;
				hayvanVarMi.cinsiyet = cinsiyet;
				hayvanVarMi.agirlik = agirlik;
				hayvanVarMi.userId = userId;
				hayvanVarMi.kategoriId = kategoriId;
				hayvanVarMi.requestId = requestId;
				var guncelle = hctx.hayvanGuncelle(hayvanVarMi);
				if(guncelle == 1)
				{
					pc.isSynced = 1;
					pc.insertUpdate = 2;
					pc.message = "Güncelleme İşlemi tamamlandı";

					agirlikHayvan ah = new agirlikHayvan()
					{
						hayvanId = hayvanVarMi.id,
						agirlikId = hayvanVarMi.agirlik,
						requestId = requestId,
						tarih = DateTime.Now

					};
					ahctx.agirlikHayvanEkle(ah);
					return pc;

				}
				pc.isSynced = 0;
				pc.insertUpdate = -1;
				pc.message = "Bilinmeyen hata oluştu";
				return pc;

			}
			else
			{
				Hayvan hy = new Hayvan()
				{
					rfidKodu = rfidKodu,
					kupeIsmi = kupeIsmi,
					cinsiyet = cinsiyet,
					agirlik = agirlik,
					userId = userId,
					kategoriId = kategoriId,
					requestId = requestId
				};
				var eklendiMi = hctx.hayvanEkle(hy);
				hayvanVarMi = hctx.hayvanTekSadece("select * from Hayvan where requestId = @requestId and aktif = 1", new { requestId = requestId });
				if (eklendiMi == 1)
				{
					pc.isSynced = 1;
					pc.insertUpdate = 1;
					pc.message = "Ekleme İşlemi tamamlandı";

					agirlikHayvan ah = new agirlikHayvan()
					{
						hayvanId = hayvanVarMi.id,
						agirlikId = hayvanVarMi.agirlik,
						requestId = requestId,
						tarih = DateTime.Now

					};
					ahctx.agirlikHayvanEkle(ah);

					kupehayvan kh = new kupehayvan()
					{
						hayvanId = hayvanVarMi.id,
						kupeId = hayvanVarMi.rfidKodu,
						requestId = requestId,
						tarih = DateTime.Now
					};
					kaCTX.kupehayvanEkle(kh);
					return pc;
				}
				pc.isSynced = -1;
				pc.insertUpdate = -1;
				pc.message = "Bilinmeyen hata oluştu";
				return pc;
			}
			return null;

		}

		[HttpGet("GetByRfid")]
		public IActionResult GetByRfid(string rfidKodu)
		{
			// Hayvan context kullanılarak hayvan sorgulaması yapılır.
			HayvanCTX hctx = new HayvanCTX();
			var hayvan = hctx.hayvanTekSadece("select * from Hayvan WHERE rfidKodu = @rfidKodu AND aktif = 1", new { rfidKodu = rfidKodu });

			// Eğer hayvan bulunamazsa NotFound (404) döner.
			if (hayvan == null)
			{
				return NotFound(new { message = "Hayvan bulunamadı." });
			}

			// Hayvan bulunduğunda 200 (OK) ve hayvan bilgisi döner.
			return Ok(hayvan);
		}
	}
}
