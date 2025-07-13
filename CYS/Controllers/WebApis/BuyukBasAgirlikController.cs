using CYS.Models;
using CYS.Repos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CYS.Controllers.WebApis
{
	[Route("api/[controller]")]
	[ApiController]
	public class BuyukBasAgirlikController : ControllerBase
	{
		[HttpGet]
		public int Post(string eid, string baslangicAgirligi, string sonAgirlik, string hayvangirdi, string hayvancikti)
		{
			
			processsettingCTX processsettingCTX = new processsettingCTX();
			var ps = processsettingCTX.processsettingTek("select * from processsetting where id =1", null);
			agirliksuCTX asuctx = new agirliksuCTX();
			var mevcutguiddeVarMi = asuctx.agirliksuTek("select * from agirliksu where reqestId = @requestId", new { requestId = ps.mevcutRequest } );
			Hayvan ilgiliHayvan = null;
			if (ps.mevcutRequest == "")
				return -1;
			if (hayvangirdi != "-1")
			{
				mevcutguiddeVarMi = new agirliksu()
				{
					hayvanId = -1,
					reqestId = ps.mevcutRequest,
					ilkOlcum = "-1",
					sonOlcum = "-1",
					userId = 1,
					tarih = DateTime.Now,
					hayvangirdi = 1
				};
				asuctx.agirliksuEkle(mevcutguiddeVarMi);
				return 1;
			}

			if (hayvancikti != "-1")
			{
				mevcutguiddeVarMi.hayvancikti = 1;
				asuctx.agirliksuGuncelle(mevcutguiddeVarMi);
				return 1;
			}
			if (eid != "" && baslangicAgirligi == "0" && sonAgirlik == "0")
			{
				HayvanCTX hctx = new HayvanCTX();
				ilgiliHayvan = hctx.hayvanTek("select * from Hayvan where rfidKodu = @rfidKodu and aktif = 1", new { rfidKodu = eid });
				if (ilgiliHayvan == null)
				{
					int toplam = 0;
					var toplamHayvanSayisi = hctx.hayvanListSadece("select id from hayvan", null);
					if (toplamHayvanSayisi != null)
						toplam = toplamHayvanSayisi.Count;
					ilgiliHayvan = new Hayvan()
					{
						cinsiyet = "Bilinmiyor",
						kategoriId = -1,
						kupeIsmi = "TRK-" + DateTime.Now.ToString("dd.MM.yyyy") + "-" + (toplam + 1).ToString(),
						rfidKodu = eid,
						agirlik = "0",
						sonGuncelleme = DateTime.Now,
						requestId = ps.mevcutRequest,
						userId = 1,
						aktif = 1
					};
					hctx.hayvanEkle(ilgiliHayvan);
					mevcutguiddeVarMi.hayvanui = 1;

					ilgiliHayvan = hctx.hayvanTek("select * from Hayvan where rfidKodu = @eid", new { eid = eid });
					kupehayvanCTX kupehayvanCTX = new kupehayvanCTX();
					kupehayvan kh = new kupehayvan()
					{
						hayvanId = ilgiliHayvan.id,
						kupeId = eid,
						requestId = ps.mevcutRequest,
						tarih = DateTime.Now
					};
					kupehayvanCTX.kupehayvanEkle(kh);
				}
				else
				{
					ilgiliHayvan.requestId = ps.mevcutRequest;
					ilgiliHayvan.sonGuncelleme = DateTime.Now;
					mevcutguiddeVarMi.hayvanui = 2;
					hctx.hayvanGuncelle(ilgiliHayvan);

				}
				mevcutguiddeVarMi.hayvanId = ilgiliHayvan.id;
				mevcutguiddeVarMi.ilkOlcum = "-1";
				mevcutguiddeVarMi.sonOlcum = "-1";
				mevcutguiddeVarMi.userId = 1;
				mevcutguiddeVarMi.tarih = DateTime.Now;
				asuctx.agirliksuGuncelle(mevcutguiddeVarMi);
				return 1;
			}
			else if (eid != "" && baslangicAgirligi != "0")
			{
				var varMi = asuctx.agirliksuTek("select * from agirliksu where reqestId = @requestId", new { requestId = ps.mevcutRequest });
				HayvanCTX hctx = new HayvanCTX();
				ilgiliHayvan = hctx.hayvanTek("select * from Hayvan where rfidKodu = @rfidKodu", new { rfidKodu = eid });
				
				if (varMi != null)
				{
					varMi.ilkOlcum = baslangicAgirligi;
					asuctx.agirliksuGuncelle(varMi);
					ilgiliHayvan.agirlik = baslangicAgirligi;
					ilgiliHayvan.sonGuncelleme = DateTime.Now;

					hctx.hayvanGuncelle(ilgiliHayvan);
					AgirlikHayvanCTX agirlikHayvanCTX = new AgirlikHayvanCTX();
					agirlikHayvan ah = new agirlikHayvan()
					{
						agirlikId = baslangicAgirligi,
						hayvanId = ilgiliHayvan.id,
						requestId = ps.mevcutRequest,
						tarih = DateTime.Now
					};
					agirlikHayvanCTX.agirlikHayvanEkle(ah);
					return 1;
				}
			}
			else if (eid != "" && sonAgirlik != "0")
			{
				var varMi = asuctx.agirliksuTek("select * from agirliksu where reqestId = @requestId", new { requestId = ps.mevcutRequest });
				HayvanCTX hctx = new HayvanCTX();
				ilgiliHayvan = hctx.hayvanTek("select * from Hayvan where rfidKodu = @rfidKodu", new { rfidKodu = eid });
				if (varMi != null)
				{
					varMi.sonOlcum = sonAgirlik;
					asuctx.agirliksuGuncelle(varMi);
					ilgiliHayvan.sonGuncelleme = DateTime.Now;
					ilgiliHayvan.agirlik = sonAgirlik;
					hctx.hayvanGuncelle(ilgiliHayvan);
					AgirlikHayvanCTX agirlikHayvanCTX = new AgirlikHayvanCTX();
					agirlikHayvan ah = new agirlikHayvan()
					{
						agirlikId = sonAgirlik,
						hayvanId = ilgiliHayvan.id,
						requestId = ps.mevcutRequest,
						tarih = DateTime.Now
					};
					agirlikHayvanCTX.agirlikHayvanEkle(ah);
					return 1;
				}
			}
			return 0;
		}
	}
}
